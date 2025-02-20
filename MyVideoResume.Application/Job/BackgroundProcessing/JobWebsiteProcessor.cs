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
using MyVideoResume.Abstractions.Match;
using MyVideoResume.Application.Job;
using MyVideoResume.Application.Resume;
using MyVideoResume.Data;
using MyVideoResume.Data.Models.Jobs;
using MyVideoResume.Data.Models.Queues;
using MyVideoResume.Web.Common;
using PuppeteerSharp;
using Radzen;
using System.Net.Http.Json;
using System.Security.Policy;

namespace MyVideoResume.Application.Job.BackgroundProcessing;

public class JobWebsiteProcessor
{
    private readonly ILogger<JobQueueProcessor> _logger;
    private readonly HttpClient _httpClient;
    private readonly string _baseUri;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly IJobPromptEngine _engine;


    public JobWebsiteProcessor(ILogger<JobQueueProcessor> logger, IServiceScopeFactory serviceScopeFactory, IJobPromptEngine engine, IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _baseUri = configuration.GetValue<string>(Constants.BaseUriConfigurationProperty);
        _logger = logger;
        _httpClient = httpClientFactory.CreateClient(Constants.HttpClientFactory);
        _engine = engine;
    }

    public async Task CrawlWebsiteQueueJobs()
    {
        try
        {
            using (IServiceScope scope = _serviceScopeFactory.CreateScope())
            {
                using (var _dataContext = scope.ServiceProvider.GetRequiredService<DataContext>())
                {

                    //Get all Existing IT Jobs that are not deleted and came from an external source (so we don't try to import them again)
                    var getExistingITJobs = await _dataContext.Jobs.Where(x => x.DeletedDateTime == null && x.OriginalWebsiteUrl != null && x.Industry != null && x.Industry.FirstOrDefault(y => y == Industry.IT) != null).AsNoTracking().Select(y => y.OriginalWebsiteUrl).ToHashSetAsync();
                    var getExistingJobsQueued = _dataContext.QueueForJobsToProcess.Select(x => x.Url).AsNoTracking().ToHashSet();

                    //UNION so we have all the URLS
                    getExistingITJobs.UnionWith(getExistingJobsQueued);

                    //get all the URLs to Crawl
                    var sitesToCrawl = _dataContext.JobWebsites.Where(x => x.Active == true && x.DeletedDateTime == null).ToList();

                    foreach (var site in sitesToCrawl)
                    {
                        try
                        {
                            var urlsFound = new List<string>();

                            // Download the Chromium revision if it does not already exist
                            await new BrowserFetcher().DownloadAsync();
                            using (var browser = await Puppeteer.LaunchAsync(new LaunchOptions { Headless = true, AcceptInsecureCerts = true, Args = new string[] { "--no-sandbox", "--disable-web-security" } }))
                            {
                                using (var page = await browser.NewPageAsync())
                                {
                                    var res = await page.GoToAsync(site.Url);
                                    if (res != null)
                                    {
                                        var jsSelectAllAnchors = @"Array.from(document.querySelectorAll('a')).map(a => a.href);";

                                        urlsFound = await page.EvaluateExpressionAsync<List<string>>(jsSelectAllAnchors);

                                        urlsFound = urlsFound.Where(x => x.Contains(site.ParsingRegularExpression)).ToList();

                                        if (res.Status == System.Net.HttpStatusCode.Forbidden)
                                        {
                                            site.LastProcessingStatus = "Failed to Load Site.";
                                        }
                                        else
                                        {
                                            urlsFound.RemoveAll(x => getExistingITJobs.Contains(x));
                                        }
                                    }
                                }

                                if (urlsFound.Count > 0)
                                {
                                    var uniqueUrls = new HashSet<string>(urlsFound); //removes duplicates
                                    foreach (var url in uniqueUrls)
                                    {
                                        //Save to Job Queue
                                        var jobToQueue = new QueueJobToProcessEntity() { Attempts = 0, CreationDateTime = DateTime.UtcNow, Status = BatchProcessStatus.NotStarted, Url = url, Website = site };
                                        _dataContext.QueueForJobsToProcess.Add(jobToQueue);
                                    }
                                }
                            }
                            await _dataContext.SaveChangesAsync();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                            _logger.LogError(ex.Message, ex);
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
