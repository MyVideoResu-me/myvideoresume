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
using MyVideoResume.Extensions;
using Microsoft.AspNetCore.WebUtilities;
using System.Net.Http.Json;
using MyVideoResume.Client.Pages.App.Admin;
using MyVideoResume.Data.Models;
using MyVideoResume.Data.Models.Account;

namespace MyVideoResume.Client.Pages.App.Account;

public partial class AccountSettings
{
    protected int tabSelected = 0;
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
    protected List<UserCompanyRoleAssociationEntity> users;
    protected RadzenDataGrid<MyVideoResume.Data.Models.ApplicationUser> grid0;


    protected async Task OnChange(int index)
    {
        if (index == 1)
        {
            await GetUsers();
        }
    }

    private async Task GetUsers()
    {

        var result = await Security.GetCompanyUsers();
        if (result.ErrorMessage.HasValue())
        {
            ShowErrorNotification("Error", result.ErrorMessage);
        }
        else
        {
            users = result.Result;
        }
    }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        var uri = NavigationManager.ToAbsoluteUri(NavigationManager.Uri);

        if (QueryHelpers.ParseQuery(uri.Query).TryGetValue("Action", out var action))
        {
            switch (action)
            {
                case "security":
                    tabSelected = 1;
                    break;
                default:
                    tabSelected = 0;
                    break;
            }
        }

        user = await Security.ReadUser($"{Security.User.Id}");
        var result = await Security.GetUserProfile();
        if (!result.ErrorMessage.HasValue())
        {
            userProfile = result.Result;
            //if the RoleSelect is null then they navigated directly here...
            //So we need to set a default and allow them to change it.
            if (userProfile.RoleSelected == null)
            {
                var roleSelected = MyVideoResumeRoles.Recruiter;
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

    protected async Task AddClick()
    {
        await DialogService.OpenAsync<AddApplicationUser>("Add Application User");

        await GetUsers();
    }

    protected async Task ViewProfileDetails(MyVideoResume.Data.Models.Account.UserCompanyRoleAssociationEntity user)
    {
        await DialogService.OpenAsync<ViewProfileDetails>("View User", new Dictionary<string, object> { { "Id", user.Id } });
    }


    protected async Task RowSelect(MyVideoResume.Data.Models.Account.UserCompanyRoleAssociationEntity user)
    {
        await DialogService.OpenAsync<EditApplicationUser>("Edit Application User", new Dictionary<string, object> { { "Id", user.Id } });

        await GetUsers();
    }

    protected async Task DeleteClick(MyVideoResume.Data.Models.Account.UserCompanyRoleAssociationEntity user)
    {
        try
        {
            if (await DialogService.Confirm("Are you sure you want to delete this user?") == true)
            {
                await Security.DeleteUser($"{user.Id}");

                await GetUsers();
            }
        }
        catch (Exception ex)
        {
            errorVisible = true;
            error = ex.Message;
        }
    }

    private async Task HandleValidSubmit()
    {
        // Save the updated user profile data to the server
        await Http.PutAsJsonAsync("api/userprofile", userProfile);
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