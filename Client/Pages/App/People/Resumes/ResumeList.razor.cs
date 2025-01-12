using System.Net.Http;
using System.Security.Policy;
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
using MyVideoResume.Client.Shared.Resume;
using MyVideoResume.Data.Models.Resume;
using MyVideoResume.Web.Common;
using Radzen;
using Radzen.Blazor;
using static MyVideoResume.Client.Shared.Resume.ResumePreviewComponent;

namespace MyVideoResume.Client.Pages.App.People.Resumes;

public partial class ResumeList
{
    public bool ShowGrid { get; set; } = true;
    public bool ShowPreview { get { return !ShowGrid; } }
    List<ResumeSummaryItem> ResumeItems { get; set; } = new List<ResumeSummaryItem>();
    IList<ResumeSummaryItem> SelectedResumeItems { get; set; } = new List<ResumeSummaryItem>();

    public bool DisplayItem(ResumeSummaryItem item)
    {
        var result = false;
        result = item.UserId == Security.User.Id;
        return result;
    }



    protected async Task Delete(ResumeSummaryItem item)
    {
        var result = new ResponseResult();
        result = await ResumeWebService.Delete(item.Id);
        if (result.ErrorMessage.HasValue())
        {
            ShowErrorNotification("Failed to Delete", string.Empty);
        }
        else
        {
            ShowSuccessNotification("Resume Deleted", string.Empty);
            ResumeItems = await ResumeWebService.GetResumeSummaryItems();
        }
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
            ResumeItems = await ResumeWebService.GetResumeSummaryItems();
        }
    }


    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        if (Security.IsAuthenticated())
            ResumeItems = await ResumeWebService.GetResumeSummaryItems();
    }

    protected async Task ResumeCreated(ResponseResult<ResumeInformationEntity> result)
    {
        if (!result.ErrorMessage.HasValue())
            ResumeItems = await ResumeWebService.GetResumeSummaryItems();
    }

    async Task OpenAITools(RadzenSplitButtonItem args, ResumeSummaryItem item)
    {
        if (args != null)
            switch (args.Value)
            {
                case "sentiment":
                    await OpenSentimentAnalysis(item);
                    break;
                case "jobmatch":
                    await OpenJobMatchAnalysis(item);
                    break;
                default:
                    break;
            }
        else
            await OpenSentimentAnalysis(item);
    }

}