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
using MyVideoResume.Abstractions.Account.Profiles;
using MyVideoResume.Web.Common;
using MyVideoResume.Extensions;

namespace MyVideoResume.Client.Pages.App.Admin;

public partial class ViewProfileDetails
{
    protected UserProfileDTO userProfile;

    [Parameter]
    public string Id { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        var result = await Account.UserProfileReadById($"{Id}");
        if (!result.ErrorMessage.HasValue())
            userProfile = result.Result;
    }
}