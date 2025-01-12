using BlazorTemplater;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using MyVideoResume.Abstractions.Resume.Formats.JSONResumeFormat;
using MyVideoResume.Client.Pages.App.Jobs.Template;
using MyVideoResume.Client.Services;
using MyVideoResume.Client.Services.FeatureFlag;
using MyVideoResume.Data.Models.Jobs;
using MyVideoResume.Data.Models.Resume;
using Radzen;
using System.Text.Json;

namespace MyVideoResume.Client.Shared;

public class JobBaseComponent : BaseComponent
{
    [Inject] protected JobWebService JobWebService { get; set; }

    protected async Task DownloadAsHtml(JobItemEntity item)
    {
        var htmlText = new ComponentRenderer<BasicTemplate>()
        .AddService<JobWebService>(JobWebService)
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
        .Set(c => c.Item, item)
        .Render();

        await DownloadHtmlFile(htmlText, "HtmlJob");
    }

    protected async Task DownloadAsJson(JobItemEntity item)
    {
        var json = JsonSerializer.Serialize<JobItemEntity>(item);
        await DownloadJsonFile(json, "JsonJob");
    }
}
