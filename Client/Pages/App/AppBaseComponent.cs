using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MyVideoResume.Client.Services;
using Radzen;
using Microsoft.AspNetCore.Components.Authorization;
using System.ComponentModel;
using System.Linq;
using MyVideoResume.Web.Common;
using MyVideoResume.Client.Services.FeatureFlag;
using MyVideoResume.Data.Models.Resume;
using System.Text.Json;
using MyVideoResume.Abstractions.Resume.Formats.JSONResumeFormat;
using AgeCalculator.Extensions;
using MyVideoResume.Client.Pages.App.People.Resumes.Templates;
using BlazorTemplater;
using static System.Net.WebRequestMethods;
using MyVideoResume.Client.Shared.Security;
using MyVideoResume.Client.Shared;
using MyVideoResume.Client.Pages.App.People.Resumes;
using MyVideoResume.Abstractions.Profiles;
using System.IO;

namespace MyVideoResume.Client.Pages.App;


public class AppBaseComponent : BaseComponent
{
    public UserProfileDTO Profile { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        if (Security.IsAuthenticated())
        {
            var response = await Security.GetUserProfile();
            if (response.ErrorMessage.HasValue())
                ShowErrorNotification("Error", "Error Loading Profile");
            else
            {
                Profile = response.Result;
                StateHasChanged();
            }
        }
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            if (Profile != null && (Profile.IsRoleSelected == null || Profile.IsRoleSelected == false))
            {
                //TODO: Show the Welcome Message and Select a Role. Clean UP the Resume.
                //await ShowUnAuthorized("Tes");
            }
        }
    }

}