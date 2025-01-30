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
    protected MyVideoResumeRoles roleSelected;
    protected string error;
    protected bool errorVisible;
    protected bool successVisible;


    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        user = await Security.GetUserById($"{Security.User.Id}");
        var result = await Security.GetUserProfile();
        if (!result.ErrorMessage.HasValue())
            userProfile = result.Result;
        jobPreferences = new JobPreferencesEntity();
    }

    protected async Task SaveSecurity()
    {
        try
        {
            await Security.ChangePassword(oldPassword, newPassword);
            successVisible = true;
        }
        catch (Exception ex)
        {
            errorVisible = true;
            error = ex.Message;
        }
    }

    protected async Task SaveUserProfile()
    {
        try
        {
            await Security.UpdateUserProfile(userProfile, roleSelected);
            successVisible = true;
        }
        catch (Exception ex)
        {
            errorVisible = true;
            error = ex.Message;
        }
    }

    protected async Task SavePreferences()
    {
        try { }
        catch (Exception ex)
        {

        }
    }
}