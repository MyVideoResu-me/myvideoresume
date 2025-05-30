using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;
using Radzen.Blazor;
using MyVideoResume.Data.Models.Account;

namespace MyVideoResume.Client.Pages.App.Admin;

public partial class AddApplicationUser
{
    protected IEnumerable<ApplicationRole> roles;
    protected MyVideoResume.Data.Models.ApplicationUser user;
    protected IEnumerable<string> userRoles = Enumerable.Empty<string>();
    protected string error;
    protected bool errorVisible;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        user = new MyVideoResume.Data.Models.ApplicationUser();

        roles = await Security.GetRoles();
    }

    protected async Task FormSubmit(MyVideoResume.Data.Models.ApplicationUser user)
    {
        try
        {
            user.Roles = roles.Where(role => userRoles.Contains(role.Id)).ToList();
            await Security.CreateApplicationUser(user);
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