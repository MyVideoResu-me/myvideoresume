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

public class SplitFeatureFlagService : FeatureFlagService
{
    public SplitFeatureFlagService(IConfiguration configuration, ILogger<AccountService> logger) : base(configuration, logger) { }

    public override Dictionary<string, bool> FeatureFlags()
    {
        var result = new Dictionary<string, bool>();
        var featureFlagKey = configuration.GetValue<string>("FeatureFlags");
        var factory = new SplitFactory(featureFlagKey);

        var splitClient = factory.Client();
        try
        {
            splitClient.BlockUntilReady(10000);
            var splitResult = splitClient.GetTreatments("KEY", FeatureFlagNames);

            result = splitResult.Select((x) =>
            {
                var enabled = false;
                if (x.Value == "on")
                    enabled = true;
                var item = new KeyValuePair<string, bool>(x.Key, enabled) { };

                return item;
            }).ToDictionary();
            return result;
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message, ex);
            //log
            result = FeatureFlagNames.Select(x => new KeyValuePair<string, bool>(x, false)).ToDictionary();

        }
        finally
        {
            splitClient.Destroy();
        }

        return result;
    }

}
