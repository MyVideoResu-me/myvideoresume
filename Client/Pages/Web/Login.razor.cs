using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;
using Radzen.Blazor;
using MyVideoResume.Web.Common;

namespace MyVideoResume.Client.Pages.Web;

public partial class Login
{
    protected string redirectUrl;
    protected string error;
    protected string info;
    protected bool errorVisible;
    protected bool infoVisible;


    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        var query = System.Web.HttpUtility.ParseQueryString(new Uri(NavigationManager.ToAbsoluteUri(NavigationManager.Uri).ToString()).Query);

        error = query.Get("error");

        info = query.Get("info");

        redirectUrl = query.Get("redirectUrl");
        if (!redirectUrl.HasValue())
            redirectUrl = Paths.Dashboard_View;

        errorVisible = !string.IsNullOrEmpty(error);

        infoVisible = !string.IsNullOrEmpty(info);

    }

    protected async Task Register()
    {
        var result = await DialogService.OpenAsync<RegisterApplicationUser>("Create Account");

        if (result == true)
        {
            infoVisible = true;

            info = "Registration accepted. Please check your email for further instructions.";

            ShowSuccessNotification(info, string.Empty);

        }
    }

    protected async Task ResetPassword()
    {
        var result = await DialogService.OpenAsync<ResetPassword>("Reset password");

        if (result == true)
        {
            infoVisible = true;

            info = "Password reset successfully. Please check your email for further instructions.";

            ShowSuccessNotification(info, string.Empty);
        }
    }
}