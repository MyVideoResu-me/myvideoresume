using AutoMapper;
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

namespace MyVideoResume.Client.Pages.App.People.Resumes;

public class ResumeComponent : BasicTemplate
{
    [Inject] protected ResumeWebService ResumeWebService { get; set; }
    [Inject] protected IMapper Mapper { get; set; }

    protected async Task DownloadAsHtml(ResumeInformationEntity resume)
    {
        var resumeText = new ComponentRenderer<BasicTemplate>()
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
        .Set(c => c.Resume, resume)
        .Render();

        await DownloadHtmlFile(resumeText, "HtmlResume");
    }

    protected async Task DownloadAsJson(ResumeInformationEntity resume)
    {
        var metaResume = resume.MetaResume as JSONResume;
        var vm = Mapper.Map<JSONResume, ExportJSONResume>(metaResume);
        var jsonResume = JsonSerializer.Serialize<ExportJSONResume>(vm);
        await DownloadJsonFile(jsonResume, "JsonResume");
    }
}
