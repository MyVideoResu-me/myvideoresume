using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Radzen;
using System.Net.Http.Json;
using Blazored.LocalStorage;
using MyVideoResume.Abstractions.Core;
using MyVideoResume.Client.Services;

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
        try
        {
            Busy = true;
            var r = await Service.Summarize(Resume);
            Result = r.Result;
            Busy = false;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex.Message, ex);
            Result = "Coming Soon";
        }
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
