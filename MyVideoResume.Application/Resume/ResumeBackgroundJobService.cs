using Azure.Identity;
using Microsoft.AspNetCore.HeaderPropagation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using MyVideoResume.Abstractions.Core;
using MyVideoResume.Abstractions.Match;
using MyVideoResume.Data;
using MyVideoResume.Data.Models.Jobs;
using MyVideoResume.Web.Common;
using Radzen;
using System.Net.Http.Json;

namespace MyVideoResume.Application.Resume;

public class ResumeBackgroundJobService
{
    private readonly ILogger<ResumeBackgroundJobService> _logger;
    private readonly HttpClient _httpClient;
    private readonly string _baseUri;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public ResumeBackgroundJobService(ILogger<ResumeBackgroundJobService> logger, IServiceScopeFactory serviceScopeFactory, IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _baseUri = configuration.GetValue<string>(Constants.BaseUriConfigurationProperty);
        _logger = logger;
        _httpClient = httpClientFactory.CreateClient(Constants.HttpClientFactory);
    }

    public async Task ProcessResumeQueue()
    {
        try
        {
            //Get the Resources (Data)
            var processingSuccessful = false;

            using IServiceScope scope = _serviceScopeFactory.CreateScope();
            var _dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();
            var _matchService = scope.ServiceProvider.GetRequiredService<MatchService>();

            //Query the Resume Queue by taking the first queued item in the Queue with a Status of NotStarted

            var work = _dataContext.QueueForResumes.Include(x => x.ResumeItem).ThenInclude(x=>x.UserProfile).FirstOrDefault(x => x.Status == BatchProcessStatus.NotStarted && x.DeletedDateTime == null);
            if (work != null)
            {
                work.Status = BatchProcessStatus.Processing;
                work.StartBatchProcessDateTime = DateTime.UtcNow;
                _dataContext.QueueForResumes.Update(work);
                _dataContext.SaveChanges();

                //Now start processing
                //Get 5 Jobs that are not already scored for this Job
                try
                {
                    var _dataContextJobs = scope.ServiceProvider.GetRequiredService<DataContext>();
                    var existingRecommendations = _dataContextJobs.ApplicantsToJobs.Where(x => x.ResumeInformationEntityId == work.ResumeItem.Id).AsNoTracking().Select(x=>x.JobItemEntityId);
                    
                    var take5uniqueJobs = _dataContextJobs.Jobs.Where(j => !existingRecommendations.Contains(j.Id) && j.DeletedDateTime == null).Take(5).AsNoTracking();

                    foreach (var item in take5uniqueJobs)
                    {
                        var result = await _matchService.MatchByJobResumeContent(new JobResumeByContentMatchRequest() { JobContent = item.JobSerialized, ResumeContent = work.ResumeItem.ResumeSerialized });
                        if (!result.ErrorMessage.HasValue())
                        {
                            //Create a ApplicationRecommendation and Save it
                            var recommendation = new ApplicantToJobEntity()
                            {
                                JobItemEntityId = item.Id,
                                ResumeInformationEntityId = work.ResumeItem.Id,
                                CreationDateTime = DateTime.UtcNow,
                                JobApplicationStatus = JobApplicationStatus.System,
                                MatchResults = result.Result.SummaryRecommendations,
                                MatchResultsDate = DateTime.UtcNow,
                                MatchScoreRating = result.Result.Score,
                                UserProfileEntityApplyingId = work.ResumeItem.UserProfile.Id
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
                _dataContext.QueueForResumes.Update(work);
                _dataContext.SaveChanges();
                _dataContext.Dispose();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
        }
    }


    public async Task ProcessSemanticScore()
    {
        try
        {
            var uri = new Uri($"{_baseUri}{Paths.AI_API_Sentiment}");
            //Get all the Resumes that don't have a Semantic Score...
            using IServiceScope scope = _serviceScopeFactory.CreateScope();
            var headerPropagationValues = scope.ServiceProvider.GetRequiredService<HeaderPropagationValues>();
            headerPropagationValues.Headers = new Dictionary<string, StringValues>(StringComparer.OrdinalIgnoreCase);
            //eventually set headers coming from other sources (e.g. consuming a queue) 
            headerPropagationValues.Headers.Add("User-Agent", "background-service");
            Console.WriteLine("Initialized Header Propagation Values");
            var _dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();
            var items = _dataContext.ResumeInformation.Where(x => x.SentimentScore.HasValue == false && x.DeletedDateTime == null);
            Console.WriteLine($"Connected to Database. # of Resumes needing Semantic Value: {items.Count()}");
            foreach (var item in items)
            {
                var response = await _httpClient.PostAsJsonAsync<string>(uri, item.ResumeSerialized);
                var value = await response.ReadAsync<float>();
                item.SentimentScore = value;
                item.UpdateDateTime = DateTime.UtcNow;
                Console.WriteLine($"Resume ID: {item.Id} - Semantic Score: {value}");
            }
            _dataContext.SaveChanges();
            _dataContext.Dispose();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            _logger.LogError(ex.Message, ex);
        }
    }
}
