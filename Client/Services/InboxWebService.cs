using System.Text.Json;
using Microsoft.AspNetCore.Components;
using Radzen;
using MyVideoResume.Abstractions.Core;
using MyVideoResume.Web.Common;
using System.Net.Http.Json;
using MyVideoResume.Data.Models.Business;
using MyVideoResume.Data.Models;
using MyVideoResume.Abstractions.Business;

namespace MyVideoResume.Client.Services;

public partial class InboxWebService
{
    private readonly HttpClient _httpClient;
    private readonly NavigationManager _navigationManager;
    private readonly ILogger<InboxWebService> _logger;

    public InboxWebService(NavigationManager navigationManager, IHttpClientFactory factory, ILogger<InboxWebService> logger)
    {
        this._httpClient = factory.CreateClient(Constants.HttpClientFactory);
        this._navigationManager = navigationManager;
        this._logger = logger;
    }

    public async Task<List<InboxItemDTO>> GetInboxItems() //Eventually Pass in a Filter Object
    {
        var result = new List<InboxItemDTO> { };
        try
        {
            var uri = new Uri($"{_navigationManager.BaseUri}{Paths.Inbox_API_View}");
            var response = await _httpClient.GetAsync(uri);
            result = await response.ReadAsync<List<InboxItemDTO>>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
        }

        return result;
    }

    public async Task<TodoEntity> GetTask(string id)
    {
        var result = new TodoEntity();
        try
        {
            var uri = new Uri($"{_navigationManager.BaseUri}{Paths.Tasks_API_View}/{id}");
            var response = await _httpClient.GetAsync(uri);
            result = await response.ReadAsync<TodoEntity>();
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

    public async Task<ResponseResult<TodoEntity>> Save(TodoEntity item)
    {
        var r = new ResponseResult<TodoEntity>();
        try
        {
            var uri = new Uri($"{_navigationManager.BaseUri}{Paths.Tasks_API_Save}");

            var jsonText = JsonSerializer.Serialize(item);

            var response = await _httpClient.PostAsJsonAsync<string>(uri, jsonText);
            r = await response.ReadAsync<ResponseResult<TodoEntity>>();
        }
        catch (Exception ex)
        {
            r.ErrorMessage = "Failed Saving";
            _logger.LogError(ex.Message, ex);
        }
        return r;
    }
}