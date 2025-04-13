using Microsoft.AspNetCore.Components;
using Radzen;
using Radzen.Blazor;
using MyVideoResume.Data.Models.Jobs;
using MyVideoResume.Abstractions.Account.Profiles;
using MyVideoResume.Abstractions.Core;
using MyVideoResume.Extensions;
using Microsoft.AspNetCore.WebUtilities;
using System.Net.Http.Json;
using MyVideoResume.Client.Pages.App.Admin;
using MyVideoResume.Data.Models.Account;
using MyVideoResume.Data.Models;
using MyVideoResume.Abstractions.Account;
using MyVideoResume.Client.Services;

namespace MyVideoResume.Client.Pages.App.Account;

public partial class AccountSettings
{
    protected int tabSelected = 0;
    protected string oldPassword = "";
    protected string newPassword = "";
    protected string confirmPassword = "";
    protected bool value;
    protected bool isBusy;
    protected string error;
    protected bool errorVisible;
    protected bool successVisible;
    protected List<UserCompanyRoleAssociationEntity> users;
    protected RadzenDataGrid<ApplicationUser> grid0;
    public AccountSettingsDTO Settings { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        var uri = NavigationManager.ToAbsoluteUri(NavigationManager.Uri);

        if (QueryHelpers.ParseQuery(uri.Query).TryGetValue("Action", out var action))
        {
            switch (action)
            {
                case "security":
                    tabSelected = 2;
                    break;
                default:
                    tabSelected = 0;
                    break;
            }
        }

        var result = await Account.AccountSettingsRead($"{Security.User.Id}");
        if (!result.ErrorMessage.HasValue())
        {
            Settings = result.Result;

            //if the RoleSelect is null then they navigated directly here...
            //So we need to set a default and allow them to change it.
            if (Settings.RoleSelected == null)
            {
                var roleSelected = MyVideoResumeRoles.Recruiter;
                //Account.UserProfileUpdateRole(Settings, roleSelected); //Default to Job Seeker.
                Settings.RoleSelected = roleSelected;
            }
            else if (Settings.RoleSelected.Value == MyVideoResumeRoles.Recruiter)
            {
                value = false;
            }
            else
                value = true;
        }
    }

    protected async Task OnChange(int index)
    {
        if (index == 1)
        {
            await GetUsers();
        }
    }

    private async Task GetUsers()
    {
        var result = await Account.GetCompanyUsers();
        if (result.ErrorMessage.HasValue())
        {
            ShowErrorNotification("Error", result.ErrorMessage);
        }
        else
        {
            users = result.Result;
        }
    }


    protected async Task AddClick()
    {
        await DialogService.OpenAsync<AddApplicationUser>("Add Application User");

        await GetUsers();
    }

    protected async Task ViewProfileDetails(UserCompanyRoleAssociationEntity user)
    {
        await DialogService.OpenAsync<ViewProfileDetails>("View User", new Dictionary<string, object> { { "Id", user.Id } });
    }


    protected async Task RowSelect(UserCompanyRoleAssociationEntity user)
    {
        await DialogService.OpenAsync<EditApplicationUser>("Edit Application User", new Dictionary<string, object> { { "Id", user.Id } });

        await GetUsers();
    }

    protected async Task DeleteClick(UserCompanyRoleAssociationEntity user)
    {
        try
        {
            if (await DialogService.Confirm("Are you sure you want to delete this user?") == true)
            {
                await Security.DeleteApplicationUser($"{user.Id}");

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
        await Http.PutAsJsonAsync("api/userprofile", Settings);
    }
    protected async Task SaveSecurity()
    {
        isBusy = true;
        try
        {
            await Security.PasswordChange(oldPassword, newPassword);
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

            Settings.RoleSelected = roleSelected;
            //var result = await Account.UserProfileUpdateRole(Settings, roleSelected);
            //if (result.ErrorMessage.HasValue())
            //{
            //    ShowErrorNotification("Error", "Error Saving");
            //}
            //else
            //{
            //    ShowSuccessNotification("Success", "Saved Profile");
            //}
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