using System.Net.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.Web.Virtualization;
using Microsoft.JSInterop;
using MyVideoResume.Client.Services;
using MyVideoResume.Data.Models.Resume;
using MyVideoResume.Web.Common;
using Radzen;
using Radzen.Blazor;

namespace MyVideoResume.Client.Pages.App.People.Resumes;

public partial class ResumeViewer
{

    [Parameter]
    public String Slug { get; set; }

    [Inject] ILogger<ResumeViewer> Logger { get; set; }

    public ResumeInformationEntity Resume { get; set; } = new ResumeInformationEntity();

    public bool IsResumeDeleted { get; set; }

    public string ResumePageTitle { get; set; }
    public Type ComponentType { get; set; }
    public Dictionary<string, object> ComponentParameters { get; set; }

    [Inject] protected ResumeWebService Service { get; set; }



    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        try
        {
            var tempTitlePart = "View";
            Resume = await Service.GetResume(Slug);
            
            if (Resume == null || Resume.DeletedDateTime.HasValue)
                IsResumeDeleted = true;

            if (IsResumeDeleted)
            {
                tempTitlePart = "Not Available";
            }
            else
            {
                //if Not authenticated show login
                if (Security.IsNotAuthenticated())
                {
                    await ShowUnAuthorizedNoClose($"{Paths.Resume_View}/{Slug}");
                }

                if (Resume.MetaResume != null && Resume.MetaResume.Basics != null)
                    tempTitlePart = Resume.MetaResume.Basics.Name;

                if (Resume.ResumeTemplate != null)
                {
                    ComponentType = ResolveComponent(Resume.ResumeTemplate.TransformerComponentName, Resume.ResumeTemplate.Namespace);
                    ComponentParameters = new Dictionary<string, object>() { { "resume", Resume } };
                }
            }

            ResumePageTitle = $"MyVideoResu.ME - Resume - {tempTitlePart}";
            StateHasChanged();

        }
        catch (Exception ex)
        {
            Logger.LogError(ex.Message, ex);
        }
    }
}