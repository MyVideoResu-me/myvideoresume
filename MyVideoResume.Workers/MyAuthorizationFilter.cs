using Hangfire.Client;
using Hangfire;
using Hangfire.Dashboard;
using Hangfire.Server;
using Hangfire.States;
using System.Diagnostics.CodeAnalysis;

namespace MyVideoResume.Workers;

public class MyAuthorizationFilter : IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext context)
    {
        var allowedAccess = false;
        var httpContext = context.GetHttpContext();

        // Allow all authenticated users to see the Dashboard (potentially dangerous).
        allowedAccess = httpContext.User.Identity?.IsAuthenticated ?? false && httpContext.User.IsInRole("Admin");

        return allowedAccess;
    }
}