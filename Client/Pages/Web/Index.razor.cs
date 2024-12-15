using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.Web.Virtualization;
using Microsoft.JSInterop;
using MyVideoResume.Abstractions.Resume;
using MyVideoResume.Client.Services;
using Radzen;
using Radzen.Blazor;
using System.Net.Http.Json;

namespace MyVideoResume.Client.Pages.Web;

public partial class Index
{
    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
    }
}