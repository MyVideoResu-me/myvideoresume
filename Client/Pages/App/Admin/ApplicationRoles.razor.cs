using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;
using Radzen.Blazor;

namespace MyVideoResume.Client.Pages.App.Admin;

public partial class ApplicationRoles
{
    protected IEnumerable<MyVideoResume.Data.Models.ApplicationRole> roles;
    protected RadzenDataGrid<MyVideoResume.Data.Models.ApplicationRole> grid0;
    protected string error;
    protected bool errorVisible;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        roles = await Security.GetRoles();
    }

    protected async Task AddClick()
    {
        await DialogService.OpenAsync<AddApplicationRole>("Add Application Role");

        roles = await Security.GetRoles();
    }

    protected async Task DeleteClick(MyVideoResume.Data.Models.ApplicationRole role)
    {
        try
        {
            if (await DialogService.Confirm("Are you sure you want to delete this role?") == true)
            {
                await Security.DeleteRole($"{role.Id}");

                roles = await Security.GetRoles();
            }
        }
        catch (Exception ex)
        {
            errorVisible = true;
            error = ex.Message;
        }
    }
}