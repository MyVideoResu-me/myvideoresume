using Hangfire;
using Hangfire.Client;
using Hangfire.Common;
using Hangfire.Server;
using Hangfire.States;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using MyVideoResume.Web.Common;

namespace MyVideoResume.Workers;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddWorkers(this IServiceCollection services, string workerConnectionString)
    {

        services.TryAddSingleton<IBackgroundJobFactory>(x => new CustomBackgroundJobFactory(
                        new BackgroundJobFactory(x.GetRequiredService<IJobFilterProvider>())));

        services.TryAddSingleton<IBackgroundJobPerformer>(x => new CustomBackgroundJobPerformer(
            new BackgroundJobPerformer(
                x.GetRequiredService<IJobFilterProvider>(),
                x.GetRequiredService<JobActivator>(),
                TaskScheduler.Default)));

        services.TryAddSingleton<IBackgroundJobStateChanger>(x => new CustomBackgroundJobStateChanger(
                new BackgroundJobStateChanger(x.GetRequiredService<IJobFilterProvider>())));

        
        services.AddHangfire(configuration => configuration
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseSqlServerStorage(workerConnectionString));

        services.AddHostedService<RecurringJobsService>();
        services.AddHangfireServer();

        return services;
    }

    public static IApplicationBuilder UseWorkers(this IApplicationBuilder app)
    {
        app.UseHangfireDashboard(Paths.Admin_BackgroundJobsPortal, new DashboardOptions
        {
            Authorization = new[] { new MyAuthorizationFilter() }
        });

        return app;
    }
}


