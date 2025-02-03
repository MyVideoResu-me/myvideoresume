using Hangfire.Client;
using Hangfire.Server;
using Hangfire.States;
using Hangfire;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Components;
using static System.Net.WebRequestMethods;
using Hangfire.Common;
using MyVideoResume.Application.Resume;
using Hangfire.Storage;
using MyVideoResume.Application.Job.BackgroundProcessing;
using Microsoft.Extensions.Configuration;

namespace MyVideoResume.Workers;

public class CustomBackgroundJobFactory : IBackgroundJobFactory
{
    private readonly IBackgroundJobFactory _inner;

    public CustomBackgroundJobFactory([NotNull] IBackgroundJobFactory inner)
    {
        _inner = inner ?? throw new ArgumentNullException(nameof(inner));
    }

    public IStateMachine StateMachine => _inner.StateMachine;

    public BackgroundJob Create(CreateContext context)
    {
        Console.WriteLine($"Create: {context.Job.Type.FullName}.{context.Job.Method.Name} in {context.InitialState?.Name} state");
        return _inner.Create(context);
    }
}

public class CustomBackgroundJobPerformer : IBackgroundJobPerformer
{
    private readonly IBackgroundJobPerformer _inner;

    public CustomBackgroundJobPerformer([NotNull] IBackgroundJobPerformer inner)
    {
        _inner = inner ?? throw new ArgumentNullException(nameof(inner));
    }

    public object Perform(PerformContext context)
    {
        Console.WriteLine($"Perform {context.BackgroundJob.Id} ({context.BackgroundJob.Job.Type.FullName}.{context.BackgroundJob.Job.Method.Name})");
        return _inner.Perform(context);
    }
}

public class CustomBackgroundJobStateChanger : IBackgroundJobStateChanger
{
    private readonly IBackgroundJobStateChanger _inner;

    public CustomBackgroundJobStateChanger([NotNull] IBackgroundJobStateChanger inner)
    {
        _inner = inner ?? throw new ArgumentNullException(nameof(inner));
    }

    public IState ChangeState(StateChangeContext context)
    {
        Console.WriteLine($"ChangeState {context.BackgroundJobId} to {context.NewState}");
        return _inner.ChangeState(context);
    }
}

public class RecurringJobsService : BackgroundService
{
    private readonly IBackgroundJobClient _backgroundJobs;
    private readonly IRecurringJobManager _recurringJobs;
    private readonly ILogger<RecurringJobScheduler> _logger;
    private readonly ResumeBackgroundJobService _resumeService;
    private readonly JobQueueProcessor _jobQueueProcessor;
    private readonly JobRecommendationProcessor _jobRecommendationProcessor;
    private readonly JobWebsiteProcessor _jobWebsiteProcessor;
    private readonly IConfiguration _configuration;

    public RecurringJobsService(
        [NotNull] IBackgroundJobClient backgroundJobs,
        [NotNull] IRecurringJobManager recurringJobs,
        [NotNull] ILogger<RecurringJobScheduler> logger,
        [NotNull] ResumeBackgroundJobService resumeService,
        [NotNull] JobQueueProcessor jobQueueProcessor,
        [NotNull] JobRecommendationProcessor jobRecommendationProcessor,
        [NotNull] JobWebsiteProcessor jobWebsiteProcessor,
        [NotNull] IConfiguration configuration)
    {
        _backgroundJobs = backgroundJobs ?? throw new ArgumentNullException(nameof(backgroundJobs));
        _recurringJobs = recurringJobs ?? throw new ArgumentNullException(nameof(recurringJobs));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _resumeService = resumeService;
        _configuration = configuration;
        _jobQueueProcessor = jobQueueProcessor;
        _jobRecommendationProcessor = jobRecommendationProcessor;
        _jobWebsiteProcessor = jobWebsiteProcessor;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            using (var connection = JobStorage.Current.GetConnection())
            {
                foreach (var recurringJob in connection.GetRecurringJobs())
                {
                    RecurringJob.RemoveIfExists(recurringJob.Id);
                }
            }

            var backgroundJobsEnabled = _configuration.GetValue<bool>("BackgroundJobsEnabled");

            if (backgroundJobsEnabled)
            {

                _recurringJobs.AddOrUpdate("SemanticScoring", () => _resumeService.ProcessSemanticScore(), "0 */8 * * *"); //Daily 8AM

                _recurringJobs.AddOrUpdate("Process-Queue-ResumeToJob", () => _resumeService.ProcessResumeQueue(), "*/5 * * * *"); //Every 5 Minutes

                _recurringJobs.AddOrUpdate("Process-Queue-JobToResume", () => _jobRecommendationProcessor.ProcessJobQueue(), "*/10 * * * *"); //Every 10 Minutes

#if DEBUG
                _recurringJobs.AddOrUpdate("Process-CrawlWebsiteQueueJobs", () => _jobWebsiteProcessor.CrawlWebsiteQueueJobs(), "*/5 * * * *"); //Every 5 Minutes
#else
            _recurringJobs.AddOrUpdate("Process-CrawlWebsiteQueueJobs", () => _jobWebsiteProcessor.CrawlWebsiteQueueJobs(), "0 * * * *"); //Every 1 hour
#endif

#if DEBUG
                _recurringJobs.AddOrUpdate("Process-Queue-ProcessJobFromJobQueue", () => _jobQueueProcessor.ProcessJobFromJobQueue(), "*/5 * * * *"); //Every 5 Minutes
#else
            _recurringJobs.AddOrUpdate("Process-Queue-ProcessJobFromJobQueue", () => _jobQueueProcessor.ProcessJobFromJobQueue(), "*/30 * * * *"); //Every 30 Min
#endif

                //_recurringJobs.AddOrUpdate("Send", () => Console.WriteLine("Hello, seconds!"), "*/15 * * * * *");
                //_recurringJobs.AddOrUpdate("SemanticScoring", () => _resumeService.ProcessSemanticScore(), "0 */8 * * *"); //Every 8 Hours
                //_recurringJobs.AddOrUpdate("SemanticScoring", () => _resumeService.ProcessSemanticScore(), Cron.Minutely); //Minutely
                //_recurringJobs.AddOrUpdate("SemanticScoring", () => _resumeService.ProcessSemanticScore(), "25 15 * * *"); //Hourly
                //_recurringJobs.AddOrUpdate("neverfires", () => Console.WriteLine("Can only be triggered"), "0 0 31 2 *");
                //_recurringJobs.AddOrUpdate("Hawaiian", () => Console.WriteLine("Hawaiian"), "15 08 * * *", new RecurringJobOptions
                //{
                //    TimeZone = TimeZoneInfo.FindSystemTimeZoneById("Hawaiian Standard Time")
                //});
                //_recurringJobs.AddOrUpdate("UTC", () => Console.WriteLine("UTC"), "15 18 * * *");
                //_recurringJobs.AddOrUpdate("Russian", () => Console.WriteLine("Russian"), "15 21 * * *", new RecurringJobOptions
                //{
                //    TimeZone = TimeZoneInfo.Local
                //});
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An exception occurred while creating recurring jobs.");
        }

        return Task.CompletedTask;
    }
}