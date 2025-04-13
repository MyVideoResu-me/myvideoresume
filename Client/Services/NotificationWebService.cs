using System.Text.Json;
using Microsoft.AspNetCore.Components;
using Radzen;
using MyVideoResume.Abstractions.Core;
using MyVideoResume.Web.Common;
using System.Net.Http.Json;
using MyVideoResume.Abstractions.Business;
using Microsoft.Extensions.Caching.Hybrid;

namespace MyVideoResume.Client.Services;

public partial class NotificationWebService : BaseWebService
{
    private readonly ILogger<NotificationWebService> _logger;

    public NotificationWebService(HybridCache cache, NavigationManager navigationManager, IHttpClientFactory factory, ILogger<NotificationWebService> logger) : base(cache, factory, navigationManager)
    {
        this._logger = logger;
    }

    public async Task<List<NotificationDTO>> NotificationsRead() //Eventually Pass in a Filter Object
    {
        var result = new List<NotificationDTO> { };
        try
        {
            var uri = new Uri($"{_navigationManager.BaseUri}{Paths.Inbox_API_View}");
            var response = await _httpClient.GetAsync(uri);
            result = await response.ReadAsync<List<NotificationDTO>>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
        }

        return result;
    }

    public async Task<NotificationDTO> NotificationsReadById(string id)
    {
        var result = new NotificationDTO();
        try
        {
            var uri = new Uri($"{_navigationManager.BaseUri}{Paths.Tasks_API_View}/{id}");
            var response = await _httpClient.GetAsync(uri);
            result = await response.ReadAsync<NotificationDTO>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
        }
        return result;
    }

    public async Task<ResponseResult> NotificationDelete(string id)
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

    public async Task<ResponseResult<NotificationDTO>> NotificationSave(NotificationDTO item)
    {
        var r = new ResponseResult<NotificationDTO>();
        try
        {
            var uri = new Uri($"{_navigationManager.BaseUri}{Paths.Tasks_API_Save}");

            var jsonText = JsonSerializer.Serialize(item);

            var response = await _httpClient.PostAsJsonAsync<string>(uri, jsonText);
            r = await response.ReadAsync<ResponseResult<NotificationDTO>>();
        }
        catch (Exception ex)
        {
            r.ErrorMessage = "Failed Saving";
            _logger.LogError(ex.Message, ex);
        }
        return r;
    }
}