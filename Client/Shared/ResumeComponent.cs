using BlazorTemplater;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using MyVideoResume.Abstractions.Resume.Formats.JSONResumeFormat;
using MyVideoResume.Client.Pages.App.People.Resumes.Templates;
using MyVideoResume.Client.Services;
using MyVideoResume.Client.Services.FeatureFlag;
using MyVideoResume.Data.Models.Resume;
using Radzen;
using System.Text.Json;

namespace MyVideoResume.Client.Shared;

public class ResumeComponent : BasicTemplate
{
    [Inject] protected ResumeWebService ResumeWebService { get; set; }

    protected async Task DownloadAsHtml(ResumeInformationEntity resume)
    {
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
        .Set(c => c.Resume, resume)
        .Render();

        await DownloadHtmlFile(resumeText, "HtmlResume");
    }

    protected async Task DownloadAsJson(ResumeInformationEntity resume)
    {
        var metaResume = resume.MetaResume;
        var jsonResume = JsonSerializer.Serialize<JSONResume>(metaResume);
        await DownloadJsonFile(jsonResume, "JsonResume");
    }
}
