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

namespace MyVideoResume.Client.Services.FeatureFlag;

public partial class FeatureFlagClientService
{
    protected FeatureFlagWebService Service { get; set; }
    protected Dictionary<string, bool> FeatureFlags { get; set; }
    public bool IsResumeBuilderEnabled = false;
    public bool IsJobBuilderEnabled = false;

    public FeatureFlagClientService(FeatureFlagWebService service)
    {
        Service = service;
        Init();
    }
    protected async Task Init()
    {
        FeatureFlags = await Service.GetFeatureFlags();
        FeatureFlags.TryGetValue("resumebuilder", out IsResumeBuilderEnabled);
        FeatureFlags.TryGetValue("jobbuilder", out IsJobBuilderEnabled);
    }
}