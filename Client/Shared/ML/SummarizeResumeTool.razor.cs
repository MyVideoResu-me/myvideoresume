using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Radzen;
using System.Net.Http.Json;
using Blazored.LocalStorage;
using MyVideoResume.Abstractions.Core;
using MyVideoResume.Client.Services;
using MyVideoResume.Web.Common;
using Markdig;

namespace MyVideoResume.Client.Shared.ML;

public partial class SummarizeResumeTool
{
    [Inject]
    protected ILogger<SummarizeResumeTool> Logger { get; set; }

    [Inject] ResumeWebService Service { get; set; }

    [Inject]
    protected ILocalStorageService localStorage { get; set; }

    public string Result { get; set; } = "";
    public bool Busy { get; set; }
    public string Resume { get; set; }

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
                var r = await Service.Summarize(Resume);
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
            Resume = await localStorage.GetItemAsync<string>("textresume");
        }
        catch (InvalidOperationException ex)
        {
            Logger.LogError(ex.Message, ex);
        }
    }
}
