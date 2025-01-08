using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;
using Radzen.Blazor;

namespace MyVideoResume.Client.Pages.Web;

public partial class ResetPassword
{
    protected MyVideoResume.Data.Models.ApplicationUser user;
    protected bool isBusy;
    protected bool errorVisible;
    protected string error;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        user = new MyVideoResume.Data.Models.ApplicationUser();
    }

    protected async Task FormSubmit()
    {
        try
        {
            isBusy = true;

            await Security.ResetPassword(user.Email);

            DialogService.Close(true);
        }
        catch (Exception ex)
        {
            errorVisible = true;
            error = ex.Message;
        }

        isBusy = false;
    }

    protected async Task CancelClick()
    {
        DialogService.Close(false);
    }
}