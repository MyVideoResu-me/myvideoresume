using System.Text.Json;
using Microsoft.AspNetCore.Components;
using Radzen;
using MyVideoResume.Abstractions.Core;
using MyVideoResume.Web.Common;
using System.Net.Http.Json;
using MyVideoResume.Data.Models.Business;
using MyVideoResume.Abstractions.Business;

namespace MyVideoResume.Client.Services;

public partial class TaskWebService
{
    private readonly HttpClient _httpClient;
    private readonly NavigationManager _navigationManager;
    private readonly ILogger<TaskWebService> _logger;

    public TaskWebService(NavigationManager navigationManager, IHttpClientFactory factory, ILogger<TaskWebService> logger)
    {
        this._httpClient = factory.CreateClient(Constants.HttpClientFactory);
        this._navigationManager = navigationManager;
        this._logger = logger;
    }

    public async Task<List<TodoDTO>> GetTasks() //Eventually Pass in a Filter Object
    {
        var result = new List<TodoDTO> { };
        try
        {
            var uri = new Uri($"{_navigationManager.BaseUri}{Paths.Tasks_API_View}");
            var response = await _httpClient.GetAsync(uri);
            result = await response.ReadAsync<List<TodoDTO>>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
        }

        return result;
    }

    public async Task<TodoDTO> GetTask(string id)
    {
        var result = new TodoDTO();
        try
        {
            var uri = new Uri($"{_navigationManager.BaseUri}{Paths.Tasks_API_View}/{id}");
            var response = await _httpClient.GetAsync(uri);
            result = await response.ReadAsync<TodoDTO>();
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

    public async Task<ResponseResult<TodoDTO>> Save(TodoDTO item)
    {
        var r = new ResponseResult<TodoDTO>();
        try
        {
            var uri = new Uri($"{_navigationManager.BaseUri}{Paths.Tasks_API_Save}");

            var jsonText = JsonSerializer.Serialize(item);

            var response = await _httpClient.PostAsJsonAsync<string>(uri, jsonText);
            r = await response.ReadAsync<ResponseResult<TodoDTO>>();
        }
        catch (Exception ex)
        {
            r.ErrorMessage = "Failed Saving";
            _logger.LogError(ex.Message, ex);
        }
        return r;
    }
}