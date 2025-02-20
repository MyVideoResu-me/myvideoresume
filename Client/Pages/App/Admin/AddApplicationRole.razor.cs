using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;
using Radzen.Blazor;
using MyVideoResume.Client.Services;
using MyVideoResume.Data.Models.Account;

namespace MyVideoResume.Client.Pages.App.Admin;

public partial class AddApplicationRole
{
    protected ApplicationRole role;
    protected string error;
    protected bool errorVisible;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        role = new ApplicationRole();
    }

    protected async Task FormSubmit(ApplicationRole role)
    {
        try
        {
            await Security.CreateRole(role);

            DialogService.Close(null);
        }
        catch (Exception ex)
        {
            errorVisible = true;
            error = ex.Message;
        }
    }

    protected async Task CancelClick()
    {
        DialogService.Close(null);
    }
}