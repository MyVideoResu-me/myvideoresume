namespace MyVideoResume.Client.Services.FeatureFlag;

public partial class FeatureFlagClientService
{
    protected FeatureFlagWebService Service { get; set; }
    protected Dictionary<string, bool> FeatureFlags { get; set; }
    public bool IsResumeBuilderEnabled = false;
    public bool IsJobBuilderEnabled = false;

    public FeatureFlagClientService(FeatureFlagWebService service)
    {
        Service = service;
        Init();
    }
    protected async Task Init()
    {
        FeatureFlags = await Service.GetFeatureFlags();
        FeatureFlags.TryGetValue("resumebuilder", out IsResumeBuilderEnabled);
        FeatureFlags.TryGetValue("jobbuilder", out IsJobBuilderEnabled);
    }
}