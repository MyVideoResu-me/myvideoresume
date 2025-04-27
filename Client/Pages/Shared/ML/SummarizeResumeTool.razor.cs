using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Radzen;
using System.Net.Http.Json;
using Blazored.LocalStorage;
using MyVideoResume.Abstractions.Core;
using MyVideoResume.Client.Services;
using MyVideoResume.Web.Common;
using Markdig;

namespace MyVideoResume.Client.Pages.Shared.ML;

public partial class SummarizeResumeTool
{
    [Inject]
    protected ILogger<SummarizeResumeTool> Logger { get; set; }

    [Inject] ResumeWebService Service { get; set; }

    [Inject]
    protected ILocalStorageService localStorage { get; set; }

    public string Result { get; set; } = "";
    public bool Busy { get; set; }
    public string ResumeText { get; set; }

    private async Task SummarizeAsync()
    {
        Busy = true;
        try
        {
            if (Security.IsNotAuthenticated())
            {
                await ShowUnAuthorized(Paths.Tools_SummarizeResume);
            }
            else
            {
                var r = await Service.Summarize(ResumeText);
                Result = Markdown.ToHtml(r.Result);
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
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        try
        {
            ResumeText = await localStorage.GetItemAsync<string>("textresume");
        }
        catch (InvalidOperationException ex)
        {
            Logger.LogError(ex.Message, ex);
        }
    }
}
