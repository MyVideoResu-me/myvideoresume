using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MyVideoResume.Application.Account;
using Splitio.Services.Client.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyVideoResume.Application.FeatureFlag;

public interface IFeatureFlagService
{
    Dictionary<string, bool> FeatureFlags();
}

public abstract class FeatureFlagService : IFeatureFlagService
{
    protected readonly IConfiguration configuration;
    protected readonly ILogger<AccountService> logger;
    protected List<string> FeatureFlagNames { get; } = new List<string> { "resumebuilder" };

    public FeatureFlagService(IConfiguration configuration, ILogger<AccountService> logger)
    {
        this.configuration = configuration;
        this.logger = logger;
    }

    public abstract Dictionary<string, bool> FeatureFlags();
}
