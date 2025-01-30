using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyVideoResume.Abstractions.Core;
using MyVideoResume.Abstractions.Job;
using MyVideoResume.Abstractions.Profiles;
using MyVideoResume.Application;
using MyVideoResume.Application.Job;
using MyVideoResume.Application.Resume;
using MyVideoResume.Data.Models;
using MyVideoResume.Data.Models.Jobs;
using MyVideoResume.Data.Models.Resume;
using MyVideoResume.Documents;
using MyVideoResume.Web.Common;
using System.Security.Claims;

namespace MyVideoResume.Server.Controllers;

[Route("api/account")]
[ApiController]
public partial class AccountApiController : ControllerBase
{
    private readonly ILogger<AccountApiController> _logger;
    private readonly JobService _service;
    private readonly AccountService _accountService;
    public AccountApiController(IJobPromptEngine engine, ILogger<AccountApiController> logger, AccountService accountService)
    {
        _logger = logger;
        _accountService = accountService;
    }

    [Authorize]
    [HttpGet("userprofile")]
    public async Task<ActionResult<ResponseResult<UserProfileDTO>>> Userprofile()
    {
        var result = new ResponseResult<UserProfileDTO>();
        try
        {
            var id = User.FindFirstValue(ClaimTypes.NameIdentifier);
            result = await _accountService.GetUserProfile(id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            result.ErrorMessage = ex.Message;
        }
        return result;
    }
}
