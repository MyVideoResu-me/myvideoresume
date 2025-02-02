using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Blazored.LocalStorage;

namespace MyVideoResume.Client.Pages.Shared.ML;

public partial class ResumeToJSONConverterTool
{
    [Inject]
    protected ILogger<ResumeToJSONConverterTool> Logger { get; set; }

    [Inject]
    protected ILocalStorageService localStorage { get; set; }

    public bool DisableDownload { get; set; } = true;
    public string Result { get; set; } = "Upload";


    private async Task DownloadFile()
    {
        var temp = Result;
        temp = temp.Replace("```json", "").Replace("```", "");
        await JSRuntime.InvokeVoidAsync("saveTextAsFile", temp, $"JsonResume-{DateTime.Now.ToString("yyyy-MM-dd")}.json");
    }

    private async Task UploadCompletedHandler(string result)
    {
        var temp = "```json";
        temp += result;
        temp += "```";
        Result = temp;
        DisableDownload = false;
    }

}
