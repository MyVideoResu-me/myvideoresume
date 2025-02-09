using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Radzen;
using System.Net.Http.Json;
using Blazored.LocalStorage;
using MyVideoResume.Abstractions.Job;
using MyVideoResume.Abstractions.Core;
using MyVideoResume.Client.Services;
using Markdig;
using MyVideoResume.Web.Common;
using BlazorTemplater;
using Microsoft.AspNetCore.Components.Authorization;
using MyVideoResume.Client.Services.FeatureFlag;
using MyVideoResume.Data.Models.Jobs;
using System.Text.Json;
using MyVideoResume.Client.Pages.App.People.Resumes.Templates;
using MyVideoResume.Data.Models.Resume;

namespace MyVideoResume.Client.Pages.Shared.ML;

public partial class JobResumeBestResumeTool
{
    [Inject]
    protected ILogger<JobResumeBestResumeTool> Logger { get; set; }

    [Inject]
    protected ResumeWebService Service { get; set; }

    [Inject]
    protected MatchWebService MatchService { get; set; }

    [Inject]
    protected ILocalStorageService localStorage { get; set; }

    public string Result { get; set; } = "Resume";
    public string Recommendations { get; set; } = "Recommendations";
    public float Score { get; set; }
    public bool Busy { get; set; } = false;
    public string Resume { get; set; } = "Copy & Paste Resume";
    public string JobDescription { get; set; } = "Copy & Paste Job Description";

    private async Task JobResumeMatchAsync()
    {
        Busy = true;
        try
        {
            if (Security.IsNotAuthenticated())
            {
                await ShowUnAuthorized(Paths.Tools_BestResume);
            }
            else
            {
                var r = await MatchService.BestResumeByJobResumeContent(JobDescription, Resume);
                var matchResult = r.Result;
                if (matchResult != null)
                {
                    Recommendations = Markdown.ToHtml(matchResult.SummaryRecommendations);
                    Score = matchResult.Score;

                    var resumeSerialized = JsonSerializer.Serialize(matchResult.Resume, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
                    resumeSerialized = resumeSerialized.Replace("\"Id\":null", $"\"Id\":\"{Guid.NewGuid().ToString()}\"");

                    var metaResume = JsonSerializer.Deserialize<MetaResumeEntity>(resumeSerialized);
                    var resumeInformationEntity = new ResumeInformationEntity() { ResumeSerialized = resumeSerialized, MetaResume = metaResume };

                    var resumeText = new ComponentRenderer<BasicTemplate>()
                    .AddService<FeatureFlagClientService>(FeatureFlagService)
                    .AddService<MenuService>(MenuService)
                    .AddService<NavigationManager>(NavigationManager)
                    .AddService<AuthenticationStateProvider>(AuthenticationStateProvider)
                    .AddService<HttpClient>(Http)
                    .AddService<IJSRuntime>(JSRuntime)
                    .AddService<DialogService>(DialogService)
                    .AddService<TooltipService>(TooltipService)
                    .AddService<ContextMenuService>(ContextMenuService)
                    .AddService<NotificationService>(NotificationService)
                    .AddService<SecurityWebService>(Security)
                    .Set(c => c.Resume, resumeInformationEntity)
                    .Render();

                    Result = resumeText;
                }
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex.Message, ex);
            Result = "Coming Soon";
        }
        Busy = false;
    }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        try
        {
            Resume = await localStorage.GetItemAsync<string>("textresume");
        }
        catch (InvalidOperationException ex)
        {
            Logger.LogError(ex.Message, ex);
        }
    }
}
