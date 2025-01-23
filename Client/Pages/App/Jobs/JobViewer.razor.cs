using System.Net.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.Web.Virtualization;
using Microsoft.JSInterop;
using MyVideoResume.Abstractions.Job;
using MyVideoResume.Client.Services;
using MyVideoResume.Data.Models.Jobs;
using MyVideoResume.Data.Models.Resume;
using Radzen;
using Radzen.Blazor;

namespace MyVideoResume.Client.Pages.App.Jobs;

public partial class JobViewer
{

    [Parameter]
    public String Slug { get; set; }

    [Inject] ILogger<JobViewer> Logger { get; set; }

    public JobItemEntity Item { get; set; } = new JobItemEntity();

    public bool IsDeleted { get; set; }

    public string PageTitle { get; set; }

    [Inject] protected JobWebService Service { get; set; }



    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        try
        {
            var tempTitlePart = "View";
            Item = await Service.GetJob(Slug);

            if (Item == null || Item.DeletedDateTime.HasValue)
                IsDeleted = true;

            if (IsDeleted)
            {
                tempTitlePart = "Not Available";
            }
            else
            {
                tempTitlePart = Item.Title;
            }

            PageTitle = $"MyVideoResu.ME - Job - {tempTitlePart}";
            StateHasChanged();

        }
        catch (Exception ex)
        {
            Logger.LogError(ex.Message, ex);
        }
    }
}