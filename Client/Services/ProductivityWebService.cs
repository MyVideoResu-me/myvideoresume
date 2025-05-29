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

    public ProductivityWebService(HybridCache cache, NavigationManager navigationManager, IHttpClientFactory factory, ILogger<ProductivityWebService> logger) : base(cache, factory, navigationManager)
    {
        this._logger = logger;
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

            // Send the TaskDTO object directly
            var response = await _httpClient.PostAsJsonAsync(uri, item);

            if (response.IsSuccessStatusCode)
            {
                result = await response.ReadAsync<ResponseResult<IProductivityItem>>();
            }
            else
            {
                result.ErrorMessage = $"Failed to save task. Status code: {response.StatusCode}";
                _logger.LogError($"Failed to save task. Status code: {response.StatusCode}");
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