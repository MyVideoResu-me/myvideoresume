using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;
using Radzen.Blazor;

namespace MyVideoResume.Client.Pages.App.Account;

public partial class AccountSettings
{

    protected string oldPassword = "";
    protected string newPassword = "";
    protected string confirmPassword = "";
    protected Data.Models.ApplicationUser user;
    protected Data.Models.JobPreferencesEntity jobPreferences;
    protected string error;
    protected bool errorVisible;
    protected bool successVisible;


    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        user = await Security.GetUserById($"{Security.User.Id}");
        jobPreferences = new Data.Models.JobPreferencesEntity();
    }

    protected async Task FormSubmit()
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

    protected async Task SavePreferences()
    {
        try { }
        catch (Exception ex)
        {

        }
    }
}