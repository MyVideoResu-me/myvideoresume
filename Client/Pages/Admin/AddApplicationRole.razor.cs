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

namespace MyVideoResume.Client.Pages.Admin;

public partial class AddApplicationRole
{
    protected MyVideoResume.Data.Models.ApplicationRole role;
    protected string error;
    protected bool errorVisible;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        role = new MyVideoResume.Data.Models.ApplicationRole();
    }

    protected async Task FormSubmit(MyVideoResume.Data.Models.ApplicationRole role)
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