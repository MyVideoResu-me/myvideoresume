using Microsoft.AspNetCore.Components;
using Radzen;
using MyVideoResume.Web.Common;

namespace MyVideoResume.Client.Services.FeatureFlag;

public partial class FeatureFlagWebService
{
    private readonly NavigationManager _navigationManager;
    private readonly ILogger<FeatureFlagWebService> _logger;
    private readonly HttpClient _httpClient;
    public FeatureFlagWebService(NavigationManager navigationManager, IHttpClientFactory factory, ILogger<FeatureFlagWebService> logger)
    {
        _navigationManager = navigationManager;
        _httpClient = factory.CreateClient(Constants.HttpClientFactory);
        _logger = logger;
    }

    public async Task<Dictionary<string, bool>> GetFeatureFlags()
    {
        var result = new Dictionary<string, bool>();
        try
        {
            var uri = new Uri($"{_navigationManager.BaseUri}{Paths.FeatureFlags_API}");
            var response = await _httpClient.GetAsync(uri);
            result = await response.ReadAsync<Dictionary<string, bool>>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
        }

        return result;
    }
}