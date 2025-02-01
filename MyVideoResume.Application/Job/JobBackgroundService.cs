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

    public async Task ProcessJobQueue()
    {
        try
        {
            //Get the Resources (Data)
            var processingSuccessful = false;

            using IServiceScope scope = _serviceScopeFactory.CreateScope();
            var _dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();
            var _matchService = scope.ServiceProvider.GetRequiredService<MatchService>();

            //Query the Job Queue by taking the first queued item in the Queue with a Status of NotStarted

            var work = _dataContext.QueueForJobs.Include(x => x.Job).FirstOrDefault(x => x.Status == BatchProcessStatus.NotStarted && x.DeletedDateTime == null);
            if (work != null)
            {
                work.Status = BatchProcessStatus.Processing;
                work.StartBatchProcessDateTime = DateTime.UtcNow;
                _dataContext.QueueForJobs.Update(work);
                _dataContext.SaveChanges();

                //Now start processing
                //Get 5 Jobs that are not already scored for this Job
                try
                {
                    var _dataContextJobs = scope.ServiceProvider.GetRequiredService<DataContext>();
                    var existingRecommendations = _dataContextJobs.ApplicantsToJobs.Where(x => x.JobItemEntityId == work.Job.Id).AsNoTracking().Select(x => x.ResumeInformationEntityId);
                    //ToDO: Get the full count
                    //If the # is greater than 25 to queue other work jobs for this resume to complete the ranking for the jobs.
                    var takeSomeResumes = _dataContextJobs.Resumes.Include(x => x.ResumeInformation).ThenInclude(y => y.UserProfile).Where(j => !existingRecommendations.Contains(j.Id) && j.DeletedDateTime == null).Take(25).AsNoTracking();

                    foreach (var item in takeSomeResumes)
                    {
                        var result = await _matchService.MatchByJobResumeContent(new JobResumeByContentMatchRequest() { JobContent = work.Job.JobSerialized, ResumeContent = item.ResumeInformation.ResumeSerialized });
                        if (!result.ErrorMessage.HasValue())
                        {
                            //Create a ApplicationRecommendation and Save it
                            var recommendation = new ApplicantToJobEntity()
                            {
                                JobItemEntityId = work.Job.Id,
                                ResumeInformationEntityId = item.ResumeInformation.Id,
                                CreationDateTime = DateTime.UtcNow,
                                JobApplicationStatus = JobApplicationStatus.System,
                                MatchResults = result.Result.SummaryRecommendations,
                                MatchResultsDate = DateTime.UtcNow,
                                MatchScoreRating = result.Result.Score,
                                UserProfileEntityApplyingId = item.ResumeInformation.UserProfile.Id
                            };
                            _dataContextJobs.ApplicantsToJobs.Add(recommendation);
                        }
                    }
                    processingSuccessful = true;
                    _dataContextJobs.SaveChanges();

                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ex.Message);
                }

                if (processingSuccessful)
                {
                    work.Status = BatchProcessStatus.Completed;
                }
                else
                {
                    work.Status = BatchProcessStatus.Failed;
                }

                work.EndBatchProcessDateTime = DateTime.UtcNow;
                work.UpdateDateTime = DateTime.UtcNow;
                _dataContext.QueueForJobs.Update(work);
                _dataContext.SaveChanges();
                _dataContext.Dispose();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
        }

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
                    urlsFound = urlsFound.Where(x => x.Contains("/job/")).ToList();
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
                                    tempJob.CreationDateTime = DateTime.UtcNow;
                                    _dataContext.Jobs.Add(tempJob);
                                    _dataContext.SaveChanges();
                                }
                            }
                        }
                    }
                }
            }
            _dataContext.Dispose();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            _logger.LogError(ex.Message, ex);
        }
    }
}
