using System;
using System.Web;
using System.Linq;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text;
using System.Text.Json;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Radzen;
using MyVideoResume.Data.Models;
using MyVideoResume.Client.Shared.Security.Recaptcha;

namespace MyVideoResume.Client.Services;

public partial class MenuService
{
    public bool SidebarExpanded { get; set; } = true;

    public void SidebarToggleClick()
    {
        SidebarExpanded = !SidebarExpanded;
    }
}