using BlazorTemplater;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using MyVideoResume.Abstractions.Resume.Formats.JSONResumeFormat;
using MyVideoResume.Client.Pages.App.Jobs.Template;
using MyVideoResume.Client.Services;
using MyVideoResume.Client.Services.FeatureFlag;
using MyVideoResume.Client.Shared;
using MyVideoResume.Data.Models.Jobs;
using MyVideoResume.Data.Models.Resume;
using Radzen;
using System.Text.Json;

namespace MyVideoResume.Client.Pages.App.Jobs;

public class JobBaseComponent : BaseComponent
{
    [Inject] protected JobWebService JobWebService { get; set; }

    protected async Task DownloadAsHtml(JobItemEntity item)
    {
        var htmlText = new ComponentRenderer<BasicTemplate>()
        .AddService(JobWebService)
        .AddService(FeatureFlagService)
        .AddService(MenuService)
        .AddService(NavigationManager)
        .AddService(AuthenticationStateProvider)
        .AddService(Http)
        .AddService(JSRuntime)
        .AddService(DialogService)
        .AddService(TooltipService)
        .AddService(ContextMenuService)
        .AddService(NotificationService)
        .AddService(Security)
        .Set(c => c.Item, item)
        .Render();

        await DownloadHtmlFile(htmlText, "HtmlJob");
    }

    protected async Task DownloadAsJson(JobItemEntity item)
    {
        var json = JsonSerializer.Serialize(item);
        await DownloadJsonFile(json, "JsonJob");
    }
}
