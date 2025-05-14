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

    #region Check / Is in...
    public bool IsAuthenticated()
    {
        return Principal?.Identity.IsAuthenticated == true;
    }
    public bool IsNotAuthenticated()
    {
        return Principal?.Identity.IsAuthenticated == false;
    }
    public bool IsInRole(params string[] roles)
    {
        var result = Task.Run(async () => await IsInRoleAsync(roles)).Result;
        return result;
    }
    public bool IsJobSeeker()
    {
        var result = Task.Run(async () => await IsInRoleAsync(Constants.JobSeeker)).Result;
        return result;
    }
    public bool IsRecruiter()
    {
        var result = Task.Run(async () => await IsInRoleAsync(Constants.Recruiter)).Result;
        return result;
    }
    public bool IsAdmin()
    {
        var result = Task.Run(async () => await IsInRoleAsync(Constants.Admin)).Result;
        return result;
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
    #endregion

    #region State
    public async Task<ApplicationAuthenticationState> GetAuthenticationStateAsync()
    {
        var uri = new Uri($"{_navigationManager.BaseUri}Account/CurrentUser");

        var response = await _httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Post, uri));

        return await response.ReadAsync<ApplicationAuthenticationState>();
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
            User = await ReadApplicationUser(userId);
        }

        return IsAuthenticated();
    }
    #endregion

    #region Roles
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
    #endregion

    #region ApplicationUser
    public async Task<IEnumerable<ApplicationUser>> GetApplicationUsers()
    {
        var uri = new Uri($"{_navigationManager.BaseUri}odata/Identity/ApplicationUsers");


        uri = uri.GetODataUri();

        var response = await _httpClient.GetAsync(uri);

        var result = await response.ReadAsync<ODataServiceResult<ApplicationUser>>();

        return result.Value;
    }

    public async Task<ApplicationUser> CreateApplicationUser(ApplicationUser user)
    {
        var uri = new Uri($"{_navigationManager.BaseUri}odata/Identity/ApplicationUsers");

        var content = new StringContent(JsonSerializer.Serialize(user), Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync(uri, content);

        return await response.ReadAsync<ApplicationUser>();
    }

    public async Task<HttpResponseMessage> DeleteApplicationUser(string id)
    {
        var uri = new Uri($"{_navigationManager.BaseUri}odata/Identity/ApplicationUsers('{id}')");

        return await _httpClient.DeleteAsync(uri);
    }

    public async Task<ApplicationUser> ReadApplicationUser(string id)
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

    public async Task<ApplicationUser> UpdateApplicationUser(string id, ApplicationUser user)
    {
        var uri = new Uri($"{_navigationManager.BaseUri}odata/Identity/ApplicationUsers('{id}')");

        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Patch, uri)
        {
            Content = new StringContent(JsonSerializer.Serialize(user), Encoding.UTF8, "application/json")
        };

        var response = await _httpClient.SendAsync(httpRequestMessage);

        return await response.ReadAsync<ApplicationUser>();
    }
    #endregion

    #region Account (Register / Password Reset & Change / Captcha / Login & Logout)
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
    public async Task PasswordReset(string userName)
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
    public async Task PasswordChange(string oldPassword, string newPassword)
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
    public async Task<RecaptchaResponse> VerifyRecaptcha(string token)
    {
        return await recaptchaService.Verify(token);
    }
    public void Logout()
    {
        _navigationManager.NavigateTo("Account/Logout", true);
    }
    public void Login()
    {
        _navigationManager.NavigateTo("Login", true);
    }
    #endregion
}