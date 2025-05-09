using Microsoft.AspNetCore.Components;
using Radzen;
using Radzen.Blazor;
using MyVideoResume.Abstractions.Core;
using MyVideoResume.Extensions;
using Microsoft.AspNetCore.WebUtilities;
using System.Net.Http.Json;
using MyVideoResume.Client.Pages.App.Admin;
using MyVideoResume.Data.Models.Account;
using MyVideoResume.Data.Models;
using MyVideoResume.Abstractions.Account;

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
    public AccountSettingsDTO Settings { get; set; } = new AccountSettingsDTO();
    SortedList<string, string> DisplayPrivacyOptions = DisplayPrivacy.ToPublic.ToSortedList();
    public string DisplayPrivacyOptionSelected { get; set; } = DisplayPrivacy.ToPublic.ToString();
    public string DisplayPrivacyOptionContactDetailsSelected { get; set; } = DisplayPrivacy.ToPublic.ToString();


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

        var result = await Account.AccountSettingsRead();
        if (!result.ErrorMessage.HasValue())
        {
            Settings = result.Result;

            DisplayPrivacyOptionSelected = Settings.Privacy_ShowProfile.Value.ToString();
            DisplayPrivacyOptionContactDetailsSelected = Settings.Privacy_ShowProfileContactDetails.Value.ToString();

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
        var result = await Account.AccountUsers();
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

            Settings.Privacy_ShowProfile = Enum.Parse<DisplayPrivacy>(DisplayPrivacyOptionSelected);
            Settings.Privacy_ShowProfileContactDetails = Enum.Parse<DisplayPrivacy>(DisplayPrivacyOptionContactDetailsSelected);

            var result = await Account.UserProfileUpdate(Settings.CreateUserProfile());
            if (result.ErrorMessage.HasValue())
            {
                ShowErrorNotification("Error", "Error Saving");
            }
            else
            {
                ShowSuccessNotification("Success", "Saved Account Settings");
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
        try
        {
            await SaveUserProfile();
        }
        catch (Exception ex)
        {

        }
        isBusy = false;
    }
}