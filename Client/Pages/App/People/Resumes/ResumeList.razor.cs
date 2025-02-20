using MyVideoResume.Abstractions.Core;
using MyVideoResume.Abstractions.Resume;
using MyVideoResume.Data.Models.Resume;
using MyVideoResume.Extensions;
using MyVideoResume.Web.Common;
using Radzen.Blazor;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MyVideoResume.Client.Pages.App.People.Resumes;

public partial class ResumeList
{
    public bool ShowGrid { get; set; } = true;
    public bool ShowPreview { get { return !ShowGrid; } }
    List<ResumeInformationSummaryDTO> ResumeItems { get; set; } = new List<ResumeInformationSummaryDTO>();
    IList<ResumeInformationSummaryDTO> SelectedResumeItems { get; set; } = new List<ResumeInformationSummaryDTO>();





    async Task DeleteCompletedHandler(ResponseResult result)
    {
        if (result.ErrorMessage.HasValue())
        {
            ShowErrorNotification("Error Deleting Resume", string.Empty);
        }
        else
        {
            ShowSuccessNotification("Resume Deleted", string.Empty);
            ResumeItems = await ResumeWebService.GetResumesOwnedbyAuthUser();
        }
    }


    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        if (Security.IsAuthenticated())
            ResumeItems = await ResumeWebService.GetResumesOwnedbyAuthUser();
    }

    protected async Task ResumeCreated(ResponseResult<ResumeInformationDTO> result)
    {
        if (!result.ErrorMessage.HasValue())
            ResumeItems = await ResumeWebService.GetResumesOwnedbyAuthUser();
    }


}
