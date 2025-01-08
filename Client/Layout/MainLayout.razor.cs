using System.Net.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.Web.Virtualization;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.JSInterop;
using MyVideoResume.Client.Shared;
using Radzen;
using Radzen.Blazor;

namespace MyVideoResume.Client.Layout;

public partial class MainLayout 
{
    protected void ProfileMenuClick(RadzenProfileMenuItem args)
    {
        if (args.Value == "Logout")
        {
            Security.Logout();
        }
    }

    public bool ShowLogin
    {
        get
        {
            return !Security.IsAuthenticated();
        }
    }
}
