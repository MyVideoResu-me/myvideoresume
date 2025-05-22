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

/// <summary>
/// API endpoints for managing user accounts, settings, preferences, and profiles.
/// </summary>
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

    /// <summary>
    /// Gets the account settings for the currently authenticated user.
    /// </summary>
    /// <remarks>
    /// Requires authentication.
    /// </remarks>
    /// <returns>The account settings for the current user.</returns>
    /// <response code="200">Returns the account settings.</response>
    /// <response code="401">Unauthorized. Authentication is required.</response>
    [EnableQuery]
    [Authorize]
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

    /// <summary>
    /// Gets the job preferences for the currently authenticated user.
    /// </summary>
    /// <remarks>
    /// Requires authentication.
    /// </remarks>
    /// <returns>The job preferences for the current user.</returns>
    /// <response code="200">Returns the job preferences.</response>
    /// <response code="401">Unauthorized. Authentication is required.</response>
    [Authorize]
    [HttpGet("preferences/job")]
    public JobPreferencesEntity GetJobPreferences()
    {
        var id = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return null;
    }

    /// <summary>
    /// Saves the job preferences for the currently authenticated user.
    /// </summary>
    /// <remarks>
    /// Requires authentication.
    /// </remarks>
    /// <param name="preferences">The job preferences to save.</param>
    /// <returns>The saved job preferences.</returns>
    /// <response code="200">Returns the saved job preferences.</response>
    /// <response code="401">Unauthorized. Authentication is required.</response>
    [Authorize]
    [HttpPost("preferences/job")]
    public JobPreferencesEntity SaveJobPreferences([FromBody] JobPreferences preferences)
    {
        var id = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return null;
    }

    /// <summary>
    /// Gets the job preferences for a specific user.
    /// </summary>
    /// <param name="userId">The user ID.</param>
    /// <returns>The job preferences for the specified user.</returns>
    /// <response code="200">Returns the job preferences.</response>
    /// <response code="401">Unauthorized. Authentication is required.</response>
    [Authorize]
    [HttpGet("preferences/job/{userId}")]
    public JobPreferencesEntity GetJobPreferences(string userId)
    {
        return null;
    }

    /// <summary>
    /// Saves the job preferences for a specific user.
    /// </summary>
    /// <param name="userId">The user ID.</param>
    /// <param name="preferences">The job preferences to save.</param>
    /// <returns>The saved job preferences.</returns>
    /// <response code="200">Returns the saved job preferences.</response>
    /// <response code="401">Unauthorized. Authentication is required.</response>
    [Authorize]
    [HttpPost("preferences/job/{userId}")]
    public JobPreferencesEntity SaveJobPreferences(string userId, [FromBody] JobPreferences preferences)
    {
        return null;
    }
    #endregion

    #region Account 

    /// <summary>
    /// Gets a list of users associated with the current account.
    /// </summary>
    /// <remarks>
    /// Requires authentication.
    /// </remarks>
    /// <returns>A list of user-company role associations.</returns>
    /// <response code="200">Returns the list of users.</response>
    /// <response code="401">Unauthorized. Authentication is required.</response>
    [EnableQuery]
    [Authorize]
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

    /// <summary>
    /// Gets the roles for the currently authenticated user.
    /// </summary>
    /// <remarks>
    /// Requires authentication.
    /// </remarks>
    /// <returns>A list of roles for the current user.</returns>
    /// <response code="200">Returns the list of roles.</response>
    /// <response code="401">Unauthorized. Authentication is required.</response>
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

    /// <summary>
    /// Gets the profile for the currently authenticated user.
    /// </summary>
    /// <remarks>
    /// Requires authentication.
    /// </remarks>
    /// <returns>The user profile.</returns>
    /// <response code="200">Returns the user profile.</response>
    /// <response code="401">Unauthorized. Authentication is required.</response>
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

    /// <summary>
    /// Updates the profile for the currently authenticated user.
    /// </summary>
    /// <remarks>
    /// Requires authentication.
    /// </remarks>
    /// <param name="request">The user profile data to update.</param>
    /// <returns>The updated user profile.</returns>
    /// <response code="200">Returns the updated user profile.</response>
    /// <response code="401">Unauthorized. Authentication is required.</response>
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

    /// <summary>
    /// Gets the profile for a specific user.
    /// </summary>
    /// <remarks>
    /// Requires authentication.
    /// </remarks>
    /// <param name="userId">The user ID.</param>
    /// <returns>The user profile for the specified user.</returns>
    /// <response code="200">Returns the user profile.</response>
    /// <response code="401">Unauthorized. Authentication is required.</response>
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
