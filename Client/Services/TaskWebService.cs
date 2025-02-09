using System.Text.Json;
using Microsoft.AspNetCore.Components;
using Radzen;
using MyVideoResume.Abstractions.Core;
using MyVideoResume.Web.Common;
using System.Net.Http.Json;
using MyVideoResume.Data.Models.Business;
using Microsoft.Extensions.Caching.Hybrid;
using MyVideoResume.Abstractions.Business.Tasks;
using MyVideoResume.Data.Models.Business.Tasks;

namespace MyVideoResume.Client.Services;

public partial class TaskWebService : BaseWebService
{
    private readonly ILogger<TaskWebService> _logger;

    public TaskWebService(HybridCache cache, NavigationManager navigationManager, IHttpClientFactory factory, ILogger<TaskWebService> logger) : base(cache, factory, navigationManager)
    {
        this._logger = logger;
    }

    public async Task<List<TaskDTO>> GetTasks() //Eventually Pass in a Filter Object
    {
        var result = new List<TaskDTO> { };
        try
        {
            var uri = new Uri($"{_navigationManager.BaseUri}{Paths.Tasks_API_View}");
            var response = await _httpClient.GetAsync(uri);
            result = await response.ReadAsync<List<TaskDTO>>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
        }

        return result;
    }

    public async Task<TaskDTO> GetTask(string id)
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

    public async Task<ResponseResult<TaskDTO>> Save(TaskDTO item)
    {
        var r = new ResponseResult<TaskDTO>();
        try
        {
            var uri = new Uri($"{_navigationManager.BaseUri}{Paths.Tasks_API_Save}");

            var jsonText = JsonSerializer.Serialize(item);

            var response = await _httpClient.PostAsJsonAsync<string>(uri, jsonText);
            r = await response.ReadAsync<ResponseResult<TaskDTO>>();
        }
        catch (Exception ex)
        {
            r.ErrorMessage = "Failed Saving";
            _logger.LogError(ex.Message, ex);
        }
        return r;
    }

    public async Task<String> GetUrl(TaskEntity item)
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
                part = part + item.CompanyProfile.Id;
                break;

        }
        var url = $"{_navigationManager.BaseUri}{part}";
        return url;
    }
}