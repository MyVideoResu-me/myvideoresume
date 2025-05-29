using System.Text.Json;
using Microsoft.AspNetCore.Components;
using Radzen;
using MyVideoResume.Abstractions.Core;
using MyVideoResume.Web.Common;
using System.Net.Http.Json;
using Microsoft.Extensions.Caching.Hybrid;
using MyVideoResume.Abstractions.Productivity;

namespace MyVideoResume.Client.Services;

public partial class ProductivityWebService : BaseWebService
{
    private readonly ILogger<ProductivityWebService> _logger;
    private readonly SecurityWebService _securityService;

    public ProductivityWebService(HybridCache cache, NavigationManager navigationManager, IHttpClientFactory factory, ILogger<ProductivityWebService> logger, SecurityWebService securityService) : base(cache, factory, navigationManager)
    {
        this._logger = logger;
        this._securityService = securityService;
    }

    #region Tasks
    public async Task<List<IProductivityItem>> TasksRead() //Eventually Pass in a Filter Object
    {
        var result = new List<IProductivityItem> { };
        try
        {
            var uri = new Uri($"{_navigationManager.BaseUri}{Paths.Tasks_API_View}");
            var response = await _httpClient.GetAsync(uri);
            result = await response.ReadAsync<List<IProductivityItem>>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
        }

        return result;
    }

    public async Task<IProductivityItem> TaskReadById(string id)
    {
        var result = new TaskDTO();
        try
        {
            var uri = new Uri($"{_navigationManager.BaseUri}{Paths.Tasks_API_View}/{id}");
            var response = await _httpClient.GetAsync(uri);
            result = await response.ReadAsync<TaskDTO>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
        }
        return result;
    }

    public async Task<ResponseResult> TaskDelete(string id)
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

    public async Task<ResponseResult<IProductivityItem>> TaskSave(TaskDTO item)
    {
        var result = new ResponseResult<IProductivityItem>();
        try
        {
            var uri = new Uri($"{_navigationManager.BaseUri}{Paths.Tasks_API_Save}");

            // If BoardId is not set, get the default board
            if (string.IsNullOrEmpty(item.BoardId))
            {
                var defaultBoardUri = new Uri($"{_navigationManager.BaseUri}api/board/default");
                var boardResponse = await _httpClient.GetAsync(defaultBoardUri);
                if (boardResponse.IsSuccessStatusCode)
                {
                    var board = await boardResponse.Content.ReadFromJsonAsync<BoardDTO>();
                    if (board != null)
                    {
                        item.BoardId = board.Id;
                    }
                }
            }

            // Set AssignedToUser if not already set
            if (item.AssignedToUser == null && item.AssignedToUserId.HasValue)
            {
                var userProfile = await _securityService.GetUserProfileAsync();
                if (userProfile != null)
                {
                    item.AssignedToUser = userProfile;
                }
            }

            // Configure JSON serialization options
            var jsonOptions = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Converters = { new System.Text.Json.Serialization.JsonStringEnumConverter() }
            };

            // Log the payload being sent
            var payload = JsonSerializer.Serialize(item, jsonOptions);
            _logger.LogInformation("Sending task payload: {Payload}", payload);

            // Create the request with proper content type and serialization
            var content = new StringContent(payload, System.Text.Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(uri, content);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                result = JsonSerializer.Deserialize<ResponseResult<IProductivityItem>>(responseContent, jsonOptions);
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                result.ErrorMessage = $"Failed to save task. Status code: {response.StatusCode}. Error: {errorContent}";
                _logger.LogError("Failed to save task. Status code: {StatusCode}. Error: {Error}", response.StatusCode, errorContent);
            }
        }
        catch (Exception ex)
        {
            result.ErrorMessage = "Failed Saving: " + ex.Message;
            _logger.LogError(ex.Message, ex);
        }
        return result;
    }

    public async Task<String> TaskGetUrl(TaskDTO item)
    {
        var part = "/";
        switch (item.TaskType)
        {
            case TaskType.Onboarding:
                if (item.SubTaskType == TaskType.OnboardingProfile) {
                    part = $"/profile/settings?Action=Profile";
                }
                if (item.SubTaskType == TaskType.OnboardingProfileSettings) {
                    part = $"/profile/settings?Action=Settings";
                }
                if (item.SubTaskType == TaskType.OnboardingPrivacy)
                {
                    part = $"/profile/settings?Action=Privacy";
                }
                break;
            case TaskType.Company:
                //Example /company/profile/Id
                //Example /company/
                part = Paths.Companies_View + "/";

                if (item.ActionToTake == Actions.Create)
                { }
                part = part + item.Id;
                break;

        }
        var url = $"{_navigationManager.BaseUri}{part}";
        return url;
    }
    #endregion
    #region Appointments

    #endregion


}