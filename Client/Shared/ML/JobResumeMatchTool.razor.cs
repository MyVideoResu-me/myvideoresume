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

namespace MyVideoResume.Client.Shared.ML;

public partial class JobResumeMatchTool
{
    [Inject]
    protected ILogger<SummarizeResumeTool> Logger { get; set; }

    [Inject]
    protected ResumeWebService Service { get; set; }

    [Inject]
    protected MatchWebService MatchService { get; set; }

    [Inject]
    protected ILocalStorageService localStorage { get; set; }

    public string Result { get; set; } = "Results";
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
                await ShowUnAuthorized(Paths.Tools_JobMatch);
            }
            else
            {
                var r = await MatchService.MatchByJobResumeContent(JobDescription, Resume);
                var matchResult = r.Result;
                if (matchResult != null)
                {
                    Result = Markdown.ToHtml(matchResult.SummaryRecommendations);
                    Score = matchResult.Score;
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
