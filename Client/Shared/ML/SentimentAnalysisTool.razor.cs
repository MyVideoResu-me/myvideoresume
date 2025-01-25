using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.Web.Virtualization;
using Microsoft.JSInterop;
using Radzen;
using Radzen.Blazor;
using System.Net.Http.Json;
using Blazored.LocalStorage;
using MyVideoResume.Web.Common;

namespace MyVideoResume.Client.Shared.ML;

public partial class SentimentAnalysisTool
{
    [Inject]
    protected ILogger<SentimentAnalysisTool> Logger { get; set; }

    [Inject]
    protected ILocalStorageService localStorage { get; set; }

    [Parameter]
    public float happiness { get; set; } = 50; // 0=worst, 100=best

    public string resume { get; set; }

    private async Task UpdateScoreAsync(ChangeEventArgs e)
    {
        string targetText = (string)e.Value;
        // Make a real call to Sentiment service
        happiness = await PredictSentimentAsync(targetText);
    }

    private async Task<float> PredictSentimentAsync(string targetText)
    {
        var textResume = await localStorage.GetItemAsync<string>("textresume");
        if (string.IsNullOrEmpty(targetText) && !string.IsNullOrWhiteSpace(textResume))
        {
            resume = textResume;
        }
        else
        {
            resume = targetText;
        }

        float percentage = 0;
        try
        {
            percentage = await ResumeWebService.GetSentimentAnalysisByText(resume);
            await localStorage.SetItemAsync("textresume", resume);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex.Message, ex);
        }
        
        StateHasChanged();

        return percentage;
    }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        try
        {
            resume = await localStorage.GetItemAsync<string>("textresume");
            if (!string.IsNullOrEmpty(resume))
            {
                happiness = await PredictSentimentAsync(resume);
            }
        }
        catch (InvalidOperationException ex)
        {
            Logger.LogError(ex.Message, ex);
        }
    }
}
