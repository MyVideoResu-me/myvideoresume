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
using MyVideoResume.Web.Common;

namespace MyVideoResume.Client.Services;

public partial class SecurityWebService
{

    private readonly HttpClient httpClient;


    private readonly NavigationManager navigationManager;

    private readonly RecaptchaService recaptchaService;

    public ApplicationUser User { get; private set; } = new ApplicationUser { Name = "Anonymous" };

    public ClaimsPrincipal Principal { get; private set; }

    private readonly ILogger<SecurityWebService> _logger;

    public SecurityWebService(NavigationManager navigationManager, IHttpClientFactory factory, ILogger<SecurityWebService> logger, RecaptchaService recaptchaService)
    {
        this.httpClient = factory.CreateClient(Constants.HttpClientFactory);
        this.navigationManager = navigationManager;
        this._logger = logger;
        this.recaptchaService = recaptchaService;
    }

    public async Task<RecaptchaResponse> VerifyRecaptcha(string token)
    {
        return await recaptchaService.Verify(token);
    }


    public bool IsInRole(params string[] roles)
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

        return roles.Any(role => Principal.IsInRole(role));
    }

    public bool IsAuthenticated()
    {
        return Principal?.Identity.IsAuthenticated == true;
    }

    public bool IsNotAuthenticated()
    {
        return Principal?.Identity.IsAuthenticated == false;
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
            User = await GetUserById(userId);
        }

        return IsAuthenticated();
    }


    public async Task<ApplicationAuthenticationState> GetAuthenticationStateAsync()
    {
        var uri = new Uri($"{navigationManager.BaseUri}Account/CurrentUser");

        var response = await httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Post, uri));

        return await response.ReadAsync<ApplicationAuthenticationState>();
    }

    public void Logout()
    {
        navigationManager.NavigateTo("Account/Logout", true);
    }

    public void Login()
    {
        navigationManager.NavigateTo("Login", true);
    }

    public async Task<IEnumerable<ApplicationRole>> GetRoles()
    {
        var uri = new Uri($"{navigationManager.BaseUri}odata/Identity/ApplicationRoles");

        uri = uri.GetODataUri();

        var response = await httpClient.GetAsync(uri);

        var result = await response.ReadAsync<ODataServiceResult<ApplicationRole>>();

        return result.Value;
    }

    public async Task<ApplicationRole> CreateRole(ApplicationRole role)
    {
        var uri = new Uri($"{navigationManager.BaseUri}odata/Identity/ApplicationRoles");

        var content = new StringContent(ODataJsonSerializer.Serialize(role), Encoding.UTF8, "application/json");

        var response = await httpClient.PostAsync(uri, content);

        return await response.ReadAsync<ApplicationRole>();
    }

    public async Task<HttpResponseMessage> DeleteRole(string id)
    {
        var uri = new Uri($"{navigationManager.BaseUri}odata/Identity/ApplicationRoles('{id}')");

        return await httpClient.DeleteAsync(uri);
    }

    public async Task<IEnumerable<ApplicationUser>> GetUsers()
    {
        var uri = new Uri($"{navigationManager.BaseUri}odata/Identity/ApplicationUsers");


        uri = uri.GetODataUri();

        var response = await httpClient.GetAsync(uri);

        var result = await response.ReadAsync<ODataServiceResult<ApplicationUser>>();

        return result.Value;
    }

    public async Task<ApplicationUser> CreateUser(ApplicationUser user)
    {
        var uri = new Uri($"{navigationManager.BaseUri}odata/Identity/ApplicationUsers");

        var content = new StringContent(JsonSerializer.Serialize(user), Encoding.UTF8, "application/json");

        var response = await httpClient.PostAsync(uri, content);

        return await response.ReadAsync<ApplicationUser>();
    }

    public async Task<HttpResponseMessage> DeleteUser(string id)
    {
        var uri = new Uri($"{navigationManager.BaseUri}odata/Identity/ApplicationUsers('{id}')");

        return await httpClient.DeleteAsync(uri);
    }

    public async Task<ApplicationUser> GetUserById(string id)
    {
        try
        {
            var uri = new Uri($"{navigationManager.BaseUri}odata/Identity/ApplicationUsers('{id}')?$expand=Roles");

            var response = await httpClient.GetAsync(uri);

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
        var uri = new Uri($"{navigationManager.BaseUri}odata/Identity/ApplicationUsers('{id}')");

        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Patch, uri)
        {
            Content = new StringContent(JsonSerializer.Serialize(user), Encoding.UTF8, "application/json")
        };

        var response = await httpClient.SendAsync(httpRequestMessage);

        return await response.ReadAsync<ApplicationUser>();
    }
    public async Task ChangePassword(string oldPassword, string newPassword)
    {
        var uri = new Uri($"{navigationManager.BaseUri}Account/ChangePassword");

        var content = new FormUrlEncodedContent(new Dictionary<string, string> {
            { "oldPassword", oldPassword },
            { "newPassword", newPassword }
        });

        var response = await httpClient.PostAsync(uri, content);

        if (!response.IsSuccessStatusCode)
        {
            var message = await response.Content.ReadAsStringAsync();

            throw new ApplicationException(message);
        }
    }

    public async Task Register(string userName, string password)
    {
        var uri = new Uri($"{navigationManager.BaseUri}Account/Register");

        var content = new FormUrlEncodedContent(new Dictionary<string, string> {
            { "userName", userName },
            { "password", password }
        });

        var response = await httpClient.PostAsync(uri, content);

        if (!response.IsSuccessStatusCode)
        {
            var message = await response.Content.ReadAsStringAsync();

            throw new ApplicationException(message);
        }
    }

    public async Task ResetPassword(string userName)
    {
        var uri = new Uri($"{navigationManager.BaseUri}Account/ResetPassword");

        var content = new FormUrlEncodedContent(new Dictionary<string, string> {
            { "userName", userName }
        });

        var response = await httpClient.PostAsync(uri, content);

        if (!response.IsSuccessStatusCode)
        {
            var message = await response.Content.ReadAsStringAsync();

            throw new ApplicationException(message);
        }
    }
}