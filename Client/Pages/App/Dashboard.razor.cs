using System;
using System.Net.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.Web.Virtualization;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;
using MyVideoResume.Abstractions.Core;
using MyVideoResume.Abstractions.Resume;
using MyVideoResume.Client.Services;
using MyVideoResume.Client.Shared.Resume;
using MyVideoResume.Data;
using MyVideoResume.Data.Models.Resume;
using MyVideoResume.Web.Common;
using Radzen;
using Radzen.Blazor;

namespace MyVideoResume.Client.Pages.App;

public partial class Dashboard
{
    [Inject]
    protected DashboardWebService Service { get; set; }

    [Inject]
    protected ILogger<Dashboard> Console { get; set; }

    List<ResumeSummaryItem> ResumeList { get; set; } = new List<ResumeSummaryItem>();

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        if (Security.IsAuthenticated())
            ResumeList = await Service.GetResumeSummaries();
    }

    async Task DeleteCompletedHandler(ResponseResult result)
    {
        if (result.ErrorMessage.HasValue())
        {
            ShowErrorNotification("Error Deleting Resume", string.Empty);
        }
        else
        {
            ShowSuccessNotification("Resume Deleted", string.Empty);
            ResumeList = await Service.GetResumeSummaries();
            StateHasChanged();
        }
    }

    async Task UploadCompletedHandler(string result)
    {
        if (!result.HasValue())
        {
            ShowErrorNotification("Failed Creating Resume", string.Empty);
        }
        else
        {
            ShowSuccessNotification("Resume Created", string.Empty);
        }

        ResumeList = await Service.GetResumeSummaries();
    }
}