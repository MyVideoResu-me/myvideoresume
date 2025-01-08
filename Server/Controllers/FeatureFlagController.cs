using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyVideoResume.Data.Models;
using MyVideoResume.Application;
using MyVideoResume.Services;
using DocumentFormat.OpenXml.EMMA;
using MyVideoResume.Application.Resume;
using MyVideoResume.Documents;
using MyVideoResume.Application.FeatureFlag;
using MyVideoResume.Abstractions.Resume;

namespace MyVideoResume.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public partial class FeatureFlagController : ControllerBase
{
    private readonly IFeatureFlagService _engine;
    private readonly ILogger<FeatureFlagController> _logger;

    public FeatureFlagController(IFeatureFlagService engine, ILogger<FeatureFlagController> logger)
    {
        _logger = logger;
        _engine = engine;
    }

    #region Flags
    [HttpGet()]
    public async Task<ActionResult<Dictionary<string,bool>>> Get()
    {
        var result = _engine.FeatureFlags();
        return result;
    }
    #endregion

}
