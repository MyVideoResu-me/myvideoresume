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
using MyVideoResume.Extensions;
using MyVideoResume.Web.Common;
using PuppeteerSharp;
using Radzen;
using System.Net.Http.Json;
using System.Security.Policy;

namespace MyVideoResume.Application.Job.BackgroundProcessing;

public class JobRecommendationProcessor
{
    private readonly ILogger<JobQueueProcessor> _logger;
    private readonly HttpClient _httpClient;
    private readonly string _baseUri;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly IJobPromptEngine _engine;


    public JobRecommendationProcessor(ILogger<JobQueueProcessor> logger, IServiceScopeFactory serviceScopeFactory, IJobPromptEngine engine, IHttpClientFactory httpClientFactory, IConfiguration configuration)
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

            using (IServiceScope scope = _serviceScopeFactory.CreateScope())
            {
                using (var _dataContext = scope.ServiceProvider.GetRequiredService<DataContext>())
                {
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
                            var existingRecommendations = _dataContext.ApplicantsToJobs.Where(x => x.JobItemEntityId == work.Job.Id).AsNoTracking().Select(x => x.ResumeInformationEntityId).ToList();
                            //ToDO: Get the full count
                            //If the # is greater than 25 to queue other work jobs for this resume to complete the ranking for the jobs.
                            var takeSomeResumes = _dataContext.Resumes.Include(x => x.ResumeInformation).ThenInclude(y => y.UserProfile).Where(j => !existingRecommendations.Contains(j.Id) && j.DeletedDateTime == null).Take(25).AsNoTracking().ToList();

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
                                    _dataContext.ApplicantsToJobs.Add(recommendation);
                                }
                            }
                            processingSuccessful = true;
                            _dataContext.SaveChanges();

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
                    }
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
        }

    }

}
