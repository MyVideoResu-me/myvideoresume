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

public partial class EditApplicationUser
{
    protected IEnumerable<ApplicationRole> roles;
    protected MyVideoResume.Data.Models.ApplicationUser user;
    protected IEnumerable<string> userRoles;
    protected string error;
    protected bool errorVisible;

    [Parameter]
    public string Id { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        user = await Security.GetUserById($"{Id}");

        userRoles = user.Roles.Select(role => role.Id);

        roles = await Security.GetRoles();
    }

    protected async Task FormSubmit(MyVideoResume.Data.Models.ApplicationUser user)
    {
        try
        {
            user.Roles = roles.Where(role => userRoles.Contains(role.Id)).ToList();
            await Security.UpdateUser($"{Id}", user);
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