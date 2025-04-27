using System;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Authorization;

using MyVideoResume.Data.Models;

namespace MyVideoResume.Client.Services;

public class ApplicationAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly SecurityWebService securityService;
    private ApplicationAuthenticationState authenticationState;
    private ILogger<ApplicationAuthenticationStateProvider> _logger;

    public ApplicationAuthenticationStateProvider(SecurityWebService securityService, ILogger<ApplicationAuthenticationStateProvider> logger)
    {
        this.securityService = securityService;
        _logger = logger;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var identity = new ClaimsIdentity();

        try
        {
            var state = await GetApplicationAuthenticationStateAsync();

            if (state.IsAuthenticated)
            {
                identity = new ClaimsIdentity(state.Claims.Select(c => new Claim(c.Type, c.Value)), "MyVideoResume");
            }
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex.Message, ex);
        }

        var result = new AuthenticationState(new ClaimsPrincipal(identity));

        await securityService.InitializeAsync(result);

        return result;
    }

    private async Task<ApplicationAuthenticationState> GetApplicationAuthenticationStateAsync()
    {
        if (authenticationState == null)
        {
            authenticationState = await securityService.GetAuthenticationStateAsync();
        }

        return authenticationState;
    }
}