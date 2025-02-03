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

public class JobQueueProcessor
{
    private readonly ILogger<JobQueueProcessor> _logger;
    private readonly HttpClient _httpClient;
    private readonly string _baseUri;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly IJobPromptEngine _engine;


    public JobQueueProcessor(ILogger<JobQueueProcessor> logger, IServiceScopeFactory serviceScopeFactory, IJobPromptEngine engine, IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _baseUri = configuration.GetValue<string>(Constants.BaseUriConfigurationProperty);
        _logger = logger;
        _httpClient = httpClientFactory.CreateClient(Constants.HttpClientFactory);
        _engine = engine;
    }

    public async Task ProcessJobFromJobQueue()
    {
        try
        {
            using (IServiceScope scope = _serviceScopeFactory.CreateScope())
            {
                using (var _dataContext = scope.ServiceProvider.GetRequiredService<DataContext>())
                {
                    var jobsToProcess = _dataContext.QueueForJobsToProcess.Where(x => x.DeletedDateTime == null && x.Attempts <= 3 && x.JobProcessed == null && x.EndBatchProcessDateTime == null && (x.Status != BatchProcessStatus.Completed || x.Status != BatchProcessStatus.Processing)).ToList();

                    await new BrowserFetcher().DownloadAsync();
                    using (var browser = await Puppeteer.LaunchAsync(new LaunchOptions { Headless = true, AcceptInsecureCerts = true, Args = new string[] { "--no-sandbox", "--disable-web-security" } }))
                    {
                        foreach (var job in jobsToProcess)
                        {
                            try
                            {
                                job.Status = BatchProcessStatus.Processing;
                                job.Attempts = job.Attempts++;
                                job.UpdateDateTime = DateTime.UtcNow;
                                job.AttemptLastDateTime = DateTime.UtcNow;
                                _dataContext.SaveChanges();

                                using (var page = await browser.NewPageAsync())
                                {
                                    var res = await page.GoToAsync(job.Url);
                                    if (res != null)
                                    {
                                        var content = await res.TextAsync();
                                        var result = await _engine.ExtractJob(content);

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
                                                    var uriBuilder = new UriBuilder(job.Url);
                                                    uriBuilder.Path = tempJob.ATSApplyUrl;
                                                    uriBuilder.Query = string.Empty;
                                                    var applyUrl = uriBuilder.Uri.AbsoluteUri;
                                                    tempJob.ATSApplyUrl = applyUrl;
                                                }
                                                tempJob.Origin = JobOrigin.Crawler;
                                                tempJob.OriginalWebsiteUrl = job.Url;
                                                tempJob.Industry = new() { Industry.IT };
                                                tempJob.UserId = string.Empty;
                                                tempJob.CreationDateTime = DateTime.UtcNow;
                                                _dataContext.Jobs.Add(tempJob);
                                                job.JobProcessed = tempJob;
                                                job.Status = BatchProcessStatus.Completed;
                                                job.EndBatchProcessDateTime = DateTime.UtcNow;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (job.Attempts >= 3)
                                        {
                                            job.Status = BatchProcessStatus.Completed;
                                            job.EndBatchProcessDateTime = DateTime.UtcNow;
                                        }
                                        else
                                            job.Status = BatchProcessStatus.Failed;
                                        job.AttemptLastDateTime = DateTime.UtcNow;
                                        job.FailedAttemptMessage = "Failed to Process job Url";
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                if (job.Attempts >= 3)
                                {
                                    job.Status = BatchProcessStatus.Completed;
                                    job.EndBatchProcessDateTime = DateTime.UtcNow;
                                }
                                else
                                    job.Status = BatchProcessStatus.Failed;
                                job.AttemptLastDateTime = DateTime.UtcNow;
                                job.FailedAttemptMessage = ex.Message;
                            }
                            _dataContext.SaveChanges();
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
