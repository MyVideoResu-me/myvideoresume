using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;
using Radzen.Blazor;
using MyVideoResume.Data.Models.Jobs;
using MyVideoResume.Abstractions.Account.Profiles;
using MyVideoResume.Web.Common;
using MyVideoResume.Abstractions.Core;

namespace MyVideoResume.Client.Pages.App.Account;

public partial class AccountSettings
{

    protected string oldPassword = "";
    protected string newPassword = "";
    protected string confirmPassword = "";
    protected Data.Models.ApplicationUser user;
    protected UserProfileDTO userProfile;
    protected JobPreferencesEntity jobPreferences;
    protected bool value;
    protected bool isBusy;
    protected string error;
    protected bool errorVisible;
    protected bool successVisible;


    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        user = await Security.GetUserById($"{Security.User.Id}");
        var result = await Security.GetUserProfile();
        if (!result.ErrorMessage.HasValue())
        {
            userProfile = result.Result;
            //if the RoleSelect is null then they navigated directly here...
            //So we need to set a default and allow them to change it.
            if (userProfile.RoleSelected == null)
            {
                var roleSelected = MyVideoResumeRoles.JobSeeker;
                Security.UpdateUserProfileRole(userProfile, roleSelected); //Default to Job Seeker.
                userProfile.RoleSelected = roleSelected;
            }
            else if (userProfile.RoleSelected.Value == MyVideoResumeRoles.Recruiter)
            {
                value = false;
            }
            else
                value = true;
        }
        jobPreferences = new JobPreferencesEntity();
    }

    protected async Task SaveSecurity()
    {
        isBusy = true;
        try
        {
            await Security.ChangePassword(oldPassword, newPassword);
            ShowSuccessNotification("Success", "Password Updated");
            successVisible = true;
        }
        catch (Exception ex)
        {
            ShowErrorNotification("Error", ex.Message);
        }
        isBusy = false;
    }

    protected async Task SaveUserProfile()
    {
        isBusy = true;
        try
        {
            var roleSelected = MyVideoResumeRoles.JobSeeker;
            if (!value)
                roleSelected = MyVideoResumeRoles.Recruiter;

            userProfile.RoleSelected = roleSelected;
            var result = await Security.UpdateUserProfileRole(userProfile, roleSelected);
            if (result.ErrorMessage.HasValue())
            {
                ShowErrorNotification("Error", "Error Saving");
            }
            else
            {
                ShowSuccessNotification("Success", "Saved Profile");
            }
        }
        catch (Exception ex)
        {
            ShowErrorNotification("Error", ex.Message);
        }
        isBusy = false;
    }

    protected async Task SavePreferences()
    {
        isBusy = true;
        try { }
        catch (Exception ex)
        {

        }
        isBusy = false;
    }
}