using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.Web.Virtualization;
using Microsoft.JSInterop;
using Radzen;
using Radzen.Blazor;
using System.Net.Http.Json;

namespace MyVideoResume.Client.Pages.Web;

public partial class WorkWithUs
{
    [Inject] protected NavigationManager Navigation { get; set; }
    protected override void OnInitialized()
    {
        // External URL to redirect to
        string externalUrl = "https://myvideoresu.me/contact-us";
        Navigation.NavigateTo(externalUrl, true); // true forces the full page reload
    }
}