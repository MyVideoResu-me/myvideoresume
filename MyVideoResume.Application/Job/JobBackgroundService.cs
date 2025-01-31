using HtmlAgilityPack;
using HtmlAgilityPack.CssSelectors.NetCore;
using Microsoft.AspNetCore.HeaderPropagation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using MyVideoResume.Abstractions.Core;
using MyVideoResume.Abstractions.Job;
using MyVideoResume.Application.Job;
using MyVideoResume.Data;
using MyVideoResume.Data.Models.Jobs;
using MyVideoResume.Web.Common;
using PuppeteerSharp;
using Radzen;
using System.Net.Http.Json;
using System.Security.Policy;

namespace MyVideoResume.Application.Resume;

public class JobBackgroundService
{
    private readonly ILogger<JobBackgroundService> _logger;
    private readonly HttpClient _httpClient;
    private readonly string _baseUri;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly IJobPromptEngine _engine;


    public JobBackgroundService(ILogger<JobBackgroundService> logger, IServiceScopeFactory serviceScopeFactory, IJobPromptEngine engine, IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _baseUri = configuration.GetValue<string>(Constants.BaseUriConfigurationProperty);
        _logger = logger;
        _httpClient = httpClientFactory.CreateClient(Constants.HttpClientFactory);
        _engine = engine;
    }

    public async Task CrawlWebsiteCreateJobs(string urlToCrawl)
    {
        try
        {
            //var uri = new Uri($"{_baseUri}{Paths.AI_API_Sentiment}");
            //Get all the Resumes that don't have a Semantic Score...
            using IServiceScope scope = _serviceScopeFactory.CreateScope();
            //var headerPropagationValues = scope.ServiceProvider.GetRequiredService<HeaderPropagationValues>();
            //headerPropagationValues.Headers = new Dictionary<string, StringValues>(StringComparer.OrdinalIgnoreCase);
            //eventually set headers coming from other sources (e.g. consuming a queue) 
            //headerPropagationValues.Headers.Add("User-Agent", "background-service");
            //Console.WriteLine("Initialized Header Propagation Values");
            var _dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();

            //TODO: Create URL Crawler Log Table (BatchID, URL, Status) to track the urls which became jobs); Don't query the Jobs Table.
            //Get all Existing IT Jobs that are not deleted and came from an external source (so we don't try to import them again)
            var getExistingITJobs = await _dataContext.Jobs.Where(x => x.DeletedDateTime == null && x.OriginalWebsiteUrl != null && x.Industry != null && x.Industry.FirstOrDefault(y => y == Industry.IT) != null).AsNoTracking().Select(y => y.OriginalWebsiteUrl).ToHashSetAsync();

            //AGILITY PACK doesn't work...
            //var web = new HtmlWeb();
            //var rootDocument = web.Load(urlToCrawl);
            //var allLinks = rootDocument.DocumentNode.QuerySelectorAll("a").ToList();

            var urlsFound = new List<string>();
            var result = new ResponseResult<JobItemEntity>();
            // Download the Chromium revision if it does not already exist
            await new BrowserFetcher().DownloadAsync();
            using (var browser = await Puppeteer.LaunchAsync(new LaunchOptions { Headless = true, AcceptInsecureCerts = true, Args = new string[] { "--no-sandbox", "--disable-web-security" } }))
            {
                using (var page = await browser.NewPageAsync())
                {
                    var res = await page.GoToAsync(urlToCrawl);
                    var jsSelectAllAnchors = @"Array.from(document.querySelectorAll('a')).map(a => a.href);";
                    urlsFound = await page.EvaluateExpressionAsync<List<string>>(jsSelectAllAnchors);
                    urlsFound = urlsFound.Where(x => x.Contains("/job")).ToList();
                    if (res.Status == System.Net.HttpStatusCode.Forbidden)
                    {
                        result.ErrorMessage = "Failed to Load Job.";
                    }
                    else
                    {
                        urlsFound.RemoveAll(x => getExistingITJobs.Contains(x));
                    }
                }

                if (urlsFound.Count > 0)
                {
                    urlsFound = urlsFound.Distinct().ToList();
                    foreach (var url in urlsFound)
                    {
                        using (var page = await browser.NewPageAsync())
                        {
                            var res = await page.GoToAsync(url);
                            var content = await res.TextAsync();
                            //var uri = new Uri($"{_baseUri}{Paths.Jobs_API_CreateFromHtml}");
                            //var httpresponse = await _httpClient.PostAsJsonAsync<string>(uri, content);
                            //var value = await httpresponse.ReadAsync<JobItemEntity>();
                            result = await _engine.ExtractJob(content);

                            if (!result.ErrorMessage.HasValue())
                            {
                                //Examine the Job
                                var tempJob = result.Result;

                                if (tempJob.Description.HasValue() && tempJob.Responsibilities?.Count > 0 && tempJob.Requirements?.Count > 0)
                                {
                                    //Validate the apply URL is a URL... if it's not; then its probably a relative path. Extract the Domain and build the url.
                                    Uri uri;
                                    if (!Uri.TryCreate(tempJob.ATSApplyUrl, UriKind.Absolute, out uri))
                                    {
                                        var uriBuilder = new UriBuilder(url);
                                        uriBuilder.Path = tempJob.ATSApplyUrl;
                                        uriBuilder.Query = string.Empty;
                                        var applyUrl = uriBuilder.Uri.AbsoluteUri;
                                        tempJob.ATSApplyUrl = applyUrl;
                                    }
                                    tempJob.Origin = JobOrigin.Crawler;
                                    tempJob.OriginalWebsiteUrl = url;
                                    tempJob.Industry = new() { Industry.IT };
                                    tempJob.UserId = string.Empty;
                                    _dataContext.Jobs.Add(tempJob);
                                    _dataContext.SaveChanges();
                                }
                            }
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            _logger.LogError(ex.Message, ex);
        }
    }
}
