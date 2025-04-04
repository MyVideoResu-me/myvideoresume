using System.Text;
using System.Text.Json;
using System.Security.Claims;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Radzen;
using MyVideoResume.Data.Models;
using MyVideoResume.Web.Common;
using MyVideoResume.Abstractions.Core;
using MyVideoResume.Extensions;
using Microsoft.Extensions.Caching.Hybrid;
using MyVideoResume.Data.Models.Account;
using MyVideoResume.Abstractions.Account.Profiles;
using MyVideoResume.Client.Pages.Shared.Security.Recaptcha;

namespace MyVideoResume.Client.Services;

public class BaseWebService
{
    protected HybridCache _cache;
    protected readonly HttpClient _httpClient;
    protected readonly NavigationManager _navigationManager;

    public BaseWebService(HybridCache cache, IHttpClientFactory factory, NavigationManager navigationManager)
    {
        this._httpClient = factory.CreateClient(Constants.HttpClientFactory);
        this._navigationManager = navigationManager;
        this._cache = cache;
    }
}

public partial class SecurityWebService : BaseWebService
{
    private readonly ILogger<SecurityWebService> _logger;
    private readonly RecaptchaService recaptchaService;

    public ApplicationUser User { get; private set; } = new ApplicationUser { Name = "Anonymous" };

    public ClaimsPrincipal Principal { get; private set; }

    public SecurityWebService(HybridCache cache, NavigationManager navigationManager, IHttpClientFactory factory, ILogger<SecurityWebService> logger, RecaptchaService recaptchaService) : base(cache, factory, navigationManager)
    {
        this._logger = logger;
        this.recaptchaService = recaptchaService;
    }

    public async Task<bool> InitializeAsync(AuthenticationState result)
    {
        Principal = result.User;
#if DEBUG
        if (Principal.Identity.Name == "admin")
        {
            User = new ApplicationUser { Name = "Admin" };

            return true;
        }
#endif
        var userId = Principal?.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userId != null && User?.Id != userId)
        {
            User = await ReadUser(userId);
        }

        return IsAuthenticated();
    }

    public async Task<RecaptchaResponse> VerifyRecaptcha(string token)
    {
        return await recaptchaService.Verify(token);
    }

    public bool IsInRole(params string[] roles)
    {
        return Task.Run(async () => await IsInRoleAsync(roles)).Result;
    }

    public bool IsJobSeeker()
    {
        return Task.Run(async () => await IsInRoleAsync(Constants.JobSeeker)).Result;
    }
    public bool IsRecruiter()
    {
        return Task.Run(async () => await IsInRoleAsync(Constants.Recruiter)).Result;
    }
    public bool IsAdmin()
    {
        return Task.Run(async () => await IsInRoleAsync(Constants.Admin)).Result;
    }

    public async Task<bool> IsInRoleAsync(params string[] roles)
    {
        var result = false;
        try
        {
#if DEBUG
            if (User?.Name == "admin")
            {
                return true;
            }
#endif

            if (IsNotAuthenticated())
            {
                return false;
            }
            var id = Principal.FindFirstValue(ClaimTypes.NameIdentifier);
            var key = $"{id}_{CacheKeys.UserRoles}";
            var rolesFound = await _cache.GetOrCreateAsync(key, async (x) =>
            {
                var uri = new Uri($"{_navigationManager.BaseUri}api/account/user/roles");
                var response = await _httpClient.GetAsync(uri);
                var roles = await response.ReadAsync<List<string>>();

                return roles.ToHashSet();
            });

            result = roles.Any(role => rolesFound.Contains(role));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
        }

        return result;
    }

    public bool IsAuthenticated()
    {
        return Principal?.Identity.IsAuthenticated == true;
    }

    public bool IsNotAuthenticated()
    {
        return Principal?.Identity.IsAuthenticated == false;
    }



    //GET USER PROFILE
    public async Task<ResponseResult<UserProfileDTO>> GetUserProfile()
    {
        var result = new ResponseResult<UserProfileDTO>();
        try
        {
            var id = Principal.FindFirstValue(ClaimTypes.NameIdentifier);
            var profile = await _cache.GetOrCreateAsync(id, async (x) =>
            {
                var uri = new Uri($"{_navigationManager.BaseUri}api/account/userprofile");
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

    public async Task<ResponseResult<UserProfileDTO>> GetSpecificUserProfile(string userId)
    {
        var result = new ResponseResult<UserProfileDTO>();
        try
        {
            var profile = await _cache.GetOrCreateAsync(userId, async (x) =>
            {
                var uri = new Uri($"{_navigationManager.BaseUri}api/account/userprofile/{userId}");
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

    public async Task<ResponseResult<UserProfileDTO>> UpdateUserProfileRole(UserProfileDTO profile, MyVideoResumeRoles role)
    {
        var result = new ResponseResult<UserProfileDTO>();
        try
        {
            var id = Principal.FindFirstValue(ClaimTypes.NameIdentifier);


            await _cache.RemoveAsync(id);
            await _cache.RemoveAsync(CacheKeys.UserRoles);
            var userProfileResult = await _cache.GetOrCreateAsync(id, async (x) =>
            {
                //Call API 
                var uri = new Uri($"{_navigationManager.BaseUri}api/account/userprofile/updaterole");
                profile.RoleSelected = role;
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


    //TODO: GET COMPANY PROFILE
    public async Task<ResponseResult<List<UserCompanyRoleAssociationEntity>>> GetCompanyUsers()
    {
        var result = new ResponseResult<List<UserCompanyRoleAssociationEntity>>();
        try
        {
            var id = Principal.FindFirstValue(ClaimTypes.NameIdentifier);
            var key = $"{id}_CompanyUsers";
            var profile = await _cache.GetOrCreateAsync(key, async (x) =>
            {
                var uri = new Uri($"{_navigationManager.BaseUri}api/account/CompanyUsers");
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


    public async Task<ApplicationAuthenticationState> GetAuthenticationStateAsync()
    {
        var uri = new Uri($"{_navigationManager.BaseUri}Account/CurrentUser");

        var response = await _httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Post, uri));

        return await response.ReadAsync<ApplicationAuthenticationState>();
    }

    public void Logout()
    {
        _navigationManager.NavigateTo("Account/Logout", true);
    }

    public void Login()
    {
        _navigationManager.NavigateTo("Login", true);
    }

    public async Task<IEnumerable<ApplicationRole>> GetRoles()
    {
        var uri = new Uri($"{_navigationManager.BaseUri}odata/Identity/ApplicationRoles");

        uri = uri.GetODataUri();

        var response = await _httpClient.GetAsync(uri);

        var result = await response.ReadAsync<ODataServiceResult<ApplicationRole>>();

        return result.Value;
    }

    public async Task<ApplicationRole> CreateRole(ApplicationRole role)
    {
        var uri = new Uri($"{_navigationManager.BaseUri}odata/Identity/ApplicationRoles");

        var content = new StringContent(ODataJsonSerializer.Serialize(role), Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync(uri, content);

        return await response.ReadAsync<ApplicationRole>();
    }

    public async Task<HttpResponseMessage> DeleteRole(string id)
    {
        var uri = new Uri($"{_navigationManager.BaseUri}odata/Identity/ApplicationRoles('{id}')");

        return await _httpClient.DeleteAsync(uri);
    }

    public async Task<IEnumerable<ApplicationUser>> GetUsers()
    {
        var uri = new Uri($"{_navigationManager.BaseUri}odata/Identity/ApplicationUsers");


        uri = uri.GetODataUri();

        var response = await _httpClient.GetAsync(uri);

        var result = await response.ReadAsync<ODataServiceResult<ApplicationUser>>();

        return result.Value;
    }

    public async Task<ApplicationUser> CreateUser(ApplicationUser user)
    {
        var uri = new Uri($"{_navigationManager.BaseUri}odata/Identity/ApplicationUsers");

        var content = new StringContent(JsonSerializer.Serialize(user), Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync(uri, content);

        return await response.ReadAsync<ApplicationUser>();
    }

    public async Task<HttpResponseMessage> DeleteUser(string id)
    {
        var uri = new Uri($"{_navigationManager.BaseUri}odata/Identity/ApplicationUsers('{id}')");

        return await _httpClient.DeleteAsync(uri);
    }

    public async Task<ApplicationUser> ReadUser(string id)
    {
        try
        {
            var uri = new Uri($"{_navigationManager.BaseUri}odata/Identity/ApplicationUsers('{id}')?$expand=Roles");

            var response = await _httpClient.GetAsync(uri);

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }

            return await response.ReadAsync<ApplicationUser>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
        }

        return null;
    }

    public async Task<ApplicationUser> UpdateUser(string id, ApplicationUser user)
    {
        var uri = new Uri($"{_navigationManager.BaseUri}odata/Identity/ApplicationUsers('{id}')");

        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Patch, uri)
        {
            Content = new StringContent(JsonSerializer.Serialize(user), Encoding.UTF8, "application/json")
        };

        var response = await _httpClient.SendAsync(httpRequestMessage);

        return await response.ReadAsync<ApplicationUser>();
    }

    public async Task ChangePassword(string oldPassword, string newPassword)
    {
        var uri = new Uri($"{_navigationManager.BaseUri}Account/ChangePassword");

        var content = new FormUrlEncodedContent(new Dictionary<string, string> {
            { "oldPassword", oldPassword },
            { "newPassword", newPassword }
        });

        var response = await _httpClient.PostAsync(uri, content);

        if (!response.IsSuccessStatusCode)
        {
            var message = await response.Content.ReadAsStringAsync();

            throw new ApplicationException(message);
        }
    }

    public async Task Register(string userName, string password)
    {
        var uri = new Uri($"{_navigationManager.BaseUri}Account/Register");

        var content = new FormUrlEncodedContent(new Dictionary<string, string> {
            { "userName", userName },
            { "password", password }
        });

        var response = await _httpClient.PostAsync(uri, content);

        if (!response.IsSuccessStatusCode)
        {
            var message = await response.Content.ReadAsStringAsync();

            throw new ApplicationException(message);
        }
    }

    public async Task ResetPassword(string userName)
    {
        var uri = new Uri($"{_navigationManager.BaseUri}Account/ResetPassword");

        var content = new FormUrlEncodedContent(new Dictionary<string, string> {
            { "userName", userName }
        });

        var response = await _httpClient.PostAsync(uri, content);

        if (!response.IsSuccessStatusCode)
        {
            var message = await response.Content.ReadAsStringAsync();

            throw new ApplicationException(message);
        }
    }
}