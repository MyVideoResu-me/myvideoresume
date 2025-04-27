using Microsoft.AspNetCore.Components;
using Radzen;
using MyVideoResume.Abstractions.Core;
using MyVideoResume.Web.Common;
using System.Net.Http.Json;
using MyVideoResume.Abstractions.Account.Profiles;
using MyVideoResume.Data.Models.Account;
using System.Security.Claims;
using MyVideoResume.Abstractions.Account;
using Microsoft.Extensions.Caching.Hybrid;
using MyVideoResume.Extensions;

namespace MyVideoResume.Client.Services;

public partial class AccountWebService : BaseWebService
{
    private readonly ILogger<AccountWebService> _logger;
    private readonly SecurityWebService _securityWebService;

    public AccountWebService(HybridCache cache, NavigationManager navigationManager, IHttpClientFactory factory, SecurityWebService securityWebService, ILogger<AccountWebService> logger) : base(cache, factory, navigationManager)
    {
        this._logger = logger;
        this._securityWebService = securityWebService;
    }

    #region Settings
    public async Task<ResponseResult<AccountSettingsDTO>> AccountSettingsRead()
    {
        var result = new ResponseResult<AccountSettingsDTO>();
        var id = _securityWebService.Principal.FindFirstValue(ClaimTypes.NameIdentifier);
        var uri = new Uri($"{_navigationManager.BaseUri}api/account/settings");
        var response = await _httpClient.GetAsync(uri);
        result = await response.ReadAsync<ResponseResult<AccountSettingsDTO>>();

        if (result.ErrorMessage.HasValue() || result.Result == null)
            throw new NullReferenceException();

        return result;
    }
    #endregion

    #region User Profile
    public async Task<ResponseResult<UserProfileDTO>> UserProfileRead()
    {
        var result = new ResponseResult<UserProfileDTO>();
        try
        {
            var id = _securityWebService.Principal.FindFirstValue(ClaimTypes.NameIdentifier);
            var profile = await _cache.GetOrCreateAsync(id, async (x) =>
            {
                var uri = new Uri($"{_navigationManager.BaseUri}api/account/user/profile");
                var response = await _httpClient.GetAsync(uri);
                result = await response.ReadAsync<ResponseResult<UserProfileDTO>>();

                if (result.ErrorMessage.HasValue() || result.Result == null)
                    throw new NullReferenceException();

                return result;
            });

            result = profile;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            result.ErrorMessage = ex.Message;
        }

        return result;
    }

    public async Task<ResponseResult<UserProfileDTO>> UserProfileReadById(string userId)
    {
        var result = new ResponseResult<UserProfileDTO>();
        try
        {
            var profile = await _cache.GetOrCreateAsync(userId, async (x) =>
            {
                var uri = new Uri($"{_navigationManager.BaseUri}api/account/user/profile/{userId}");
                var response = await _httpClient.GetAsync(uri);
                result = await response.ReadAsync<ResponseResult<UserProfileDTO>>();

                if (result.ErrorMessage.HasValue() || result.Result == null)
                    throw new NullReferenceException();

                return result;
            });

            result = profile;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            result.ErrorMessage = ex.Message;
        }
        return result;
    }

    public async Task<ResponseResult<UserProfileDTO>> UserProfileUpdate(UserProfileDTO profile)
    {
        var result = new ResponseResult<UserProfileDTO>();
        try
        {
            var id = _securityWebService.Principal.FindFirstValue(ClaimTypes.NameIdentifier);
            await _cache.RemoveAsync(id);
            await _cache.RemoveAsync(CacheKeys.UserRoles);
            var userProfileResult = await _cache.GetOrCreateAsync(id, async (x) =>
            {
                //Call API 
                var uri = $"{_navigationManager.BaseUri}api/account/user/profile";
                var response = await _httpClient.PostAsJsonAsync<UserProfileDTO>(uri, profile);
                result = await response.ReadAsync<ResponseResult<UserProfileDTO>>();
                if (result.ErrorMessage.HasValue() || result.Result == null)
                    throw new NullReferenceException();
                return result;
            });
            result = userProfileResult;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            result.ErrorMessage = ex.Message;
        }
        return result;
    }
    #endregion

    public async Task<ResponseResult<List<UserCompanyRoleAssociationEntity>>> AccountUsers()
    {
        var result = new ResponseResult<List<UserCompanyRoleAssociationEntity>>();
        try
        {
            var id = _securityWebService.Principal.FindFirstValue(ClaimTypes.NameIdentifier);
            var key = $"{id}_CompanyUsers";
            var profile = await _cache.GetOrCreateAsync(key, async (x) =>
            {
                var uri = new Uri($"{_navigationManager.BaseUri}api/account/users");
                var response = await _httpClient.GetAsync(uri);
                result = await response.ReadAsync<ResponseResult<List<UserCompanyRoleAssociationEntity>>>();

                if (result.ErrorMessage.HasValue() || result.Result == null)
                    throw new NullReferenceException();

                return result;
            });

            result = profile;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            result.ErrorMessage = ex.Message;
        }
        return result;
    }

    public async Task<ResponseResult> Delete(string id)
    {
        var result = new ResponseResult();
        try
        {
            var uri = new Uri($"{_navigationManager.BaseUri}{Paths.Tasks_API_View}/{id}");
            var content = new FormUrlEncodedContent(new Dictionary<string, string> { { "id", id } });
            var response = await _httpClient.PostAsync(uri, content);
            result = await response.ReadAsync<ResponseResult>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            result.ErrorMessage = "Error Deleting.";
        }
        return result;
    }
}