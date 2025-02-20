using System.Net.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.Web.Virtualization;
using Microsoft.JSInterop;
using MyVideoResume.Abstractions.Core;
using MyVideoResume.Abstractions.Job;
using MyVideoResume.Abstractions.Resume;
using MyVideoResume.Client.Services;
using MyVideoResume.Data.Models.Jobs;
using MyVideoResume.Extensions;
using MyVideoResume.Web.Common;
using Radzen;
using Radzen.Blazor;

namespace MyVideoResume.Client.Pages.App.Jobs;

public partial class JobList
{
    [Inject] JobWebService Service { get; set; }
    public bool ShowGrid { get; set; } = true;
    public bool ShowPreview { get { return !ShowGrid; } }
    List<JobItemDTO> Items { get; set; } = new List<JobItemDTO>();
    IList<JobItemDTO> SelectedItems { get; set; } = new List<JobItemDTO>();

    public bool DisplayItem(JobItemDTO item)
    {
        var result = false;
        result = item.UserId == Security.User.Id;
        return result;
    }

    protected async Task Delete(JobItemDTO item)
    {
        var result = new ResponseResult();
        result = await Service.Delete(item.Id);
        await ShowNotification(result);
    }

    private async Task ShowNotification(ResponseResult result)
    {
        if (result.ErrorMessage.HasValue())
        {
            ShowErrorNotification("Failed to Delete", string.Empty);
        }
        else
        {
            ShowSuccessNotification("Job Deleted", string.Empty);
            Items = await Service.GetJobSummaryItems();
        }
    }

    async Task DeleteCompletedHandler(ResponseResult result)
    {
        await ShowNotification(result);
    }


    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        if (Security.IsAuthenticated())
            Items = await Service.GetJobSummaryItems();
    }

    protected async Task JobCreated(string result)
    {
        if (result.HasValue())
            Items = await Service.GetJobSummaryItems();
    }
}