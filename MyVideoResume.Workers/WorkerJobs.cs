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
    private readonly JobBackgroundService _jobService;

    public RecurringJobsService(
        [NotNull] IBackgroundJobClient backgroundJobs,
        [NotNull] IRecurringJobManager recurringJobs,
        [NotNull] ILogger<RecurringJobScheduler> logger,
        [NotNull] ResumeBackgroundJobService resumeService,
        [NotNull] JobBackgroundService jobService)
    {
        _backgroundJobs = backgroundJobs ?? throw new ArgumentNullException(nameof(backgroundJobs));
        _recurringJobs = recurringJobs ?? throw new ArgumentNullException(nameof(recurringJobs));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _resumeService = resumeService;
        _jobService = jobService;
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

            //_recurringJobs.AddOrUpdate("Send", () => Console.WriteLine("Hello, seconds!"), "*/15 * * * * *");
            //_recurringJobs.AddOrUpdate("SemanticScoring", () => _resumeService.ProcessSemanticScore(), "0 */8 * * *"); //Every 8 Hours
            _recurringJobs.AddOrUpdate("SemanticScoring", () => _resumeService.ProcessSemanticScore(), "0 */8 * * *"); //Daily 8AM
            //_recurringJobs.AddOrUpdate("CrawlWebsiteCreateJobs-SimplyHired-Developer", () => _jobService.CrawlWebsiteCreateJobs("https://www.simplyhired.com/search?q=Developer&l="), "0 8 * * *"); //Daily 8AM
            _recurringJobs.AddOrUpdate("CrawlWebsiteCreateJobs-Jora-Careers", () => _jobService.CrawlWebsiteCreateJobs("https://us.jora.com/j?sp=search&trigger_source=serp&q=Software+Engineer&l="), "0 */4 * * *"); //Minutely
        

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
        catch (Exception e)
        {
            _logger.LogError(e, "An exception occurred while creating recurring jobs.");
        }

        return Task.CompletedTask;
    }
}