using DocumentFormat.OpenXml.InkML;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Results;
using MyVideoResume.Abstractions.Account;
using MyVideoResume.Abstractions.Account.Preferences;
using MyVideoResume.Abstractions.Account.Profiles;
using MyVideoResume.Abstractions.Core;
using MyVideoResume.Application.Account;
using MyVideoResume.Application.Job;
using MyVideoResume.Data.Models;
using MyVideoResume.Data.Models.Account;
using MyVideoResume.Data.Models.Account.Preferences;
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

    #region Settings
    [EnableQuery]
    [HttpGet("settings")]
    public async Task<ResponseResult<AccountSettingsDTO>> GetAccountSettings()
    {
        var result = new ResponseResult<AccountSettingsDTO>();
        try
        {
            var loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            result = await _accountService.AccountSettingsRead(loggedInUserId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
        }
        return result;
    }
    #endregion

    #region Preferences
    [HttpGet("preferences/job")]
    public JobPreferencesEntity GetJobPreferences()
    {
        var id = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return null;
    }

    [HttpPost("preferences/job")]
    public JobPreferencesEntity SaveJobPreferences([FromBody] JobPreferences preferences)
    {
        var id = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return null;
    }

    [HttpGet("preferences/job/{userId}")]
    public JobPreferencesEntity GetJobPreferences(string userId)
    {
        return null;
    }

    [HttpPost("preferences/job/{userId}")]
    public JobPreferencesEntity SaveJobPreferences(string userId, [FromBody] JobPreferences preferences)
    {
        return null;
    }
    #endregion

    #region Account 
    [EnableQuery]
    [HttpGet("users")]
    public async Task<ResponseResult<List<UserCompanyRoleAssociationEntity>>> AccountUsers()
    {
        var result = new ResponseResult<List<UserCompanyRoleAssociationEntity>>();
        try
        {
            var loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            result = await _accountService.AccountUsers(loggedInUserId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
        }
        return result;
    }
    #endregion

    #region User
    [Authorize]
    [HttpGet("user/roles")]
    public async Task<ActionResult<List<string>>> UserRoles()
    {
        var result = new List<string>();
        try
        {
            var id = User.FindFirstValue(ClaimTypes.NameIdentifier);
            result = await _accountService.UserRolesRead(id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
        }
        return result;
    }

    [Authorize]
    [HttpGet("user/profile")]
    public async Task<ActionResult<ResponseResult<UserProfileDTO>>> Userprofile()
    {
        var result = new ResponseResult<UserProfileDTO>();
        try
        {
            var id = User.FindFirstValue(ClaimTypes.NameIdentifier);
            result = await _accountService.UserProfileRead(id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            result.ErrorMessage = ex.Message;
        }
        return result;
    }

    [Authorize]
    [HttpPost("user/profile")]
    public async Task<ActionResult<ResponseResult<UserProfileDTO>>> UserProfileUpdate([FromBody] UserProfileDTO request)
    {
        var result = new ResponseResult<UserProfileDTO>();
        try
        {
            var id = User.FindFirstValue(ClaimTypes.NameIdentifier);
            result = await _accountService.UserProfileUpdate(request, id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            result.ErrorMessage = ex.Message;
        }
        return result;
    }


    [Authorize]
    [HttpGet("user/profile/{userId}")]
    public async Task<ActionResult<ResponseResult<UserProfileDTO>>> Userprofile(string userId)
    {
        var result = new ResponseResult<UserProfileDTO>();
        try
        {
            result = await _accountService.UserProfileRead(userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            result.ErrorMessage = ex.Message;
        }
        return result;
    }
    #endregion
}
