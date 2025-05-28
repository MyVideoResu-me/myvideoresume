using System.Text.Json;
using Microsoft.AspNetCore.Components;
using Radzen;
using MyVideoResume.Abstractions.Core;
using MyVideoResume.Web.Common;
using System.Net.Http.Json;
using MyVideoResume.Abstractions.Business;
using Microsoft.Extensions.Caching.Hybrid;
using MyVideoResume.Abstractions.Account.Profiles;

namespace MyVideoResume.Client.Services;

public partial class CompanyWebService : BaseWebService
{
    private readonly ILogger<CompanyWebService> _logger;

    public CompanyWebService(HybridCache cache, NavigationManager navigationManager, IHttpClientFactory factory, ILogger<CompanyWebService> logger) : base(cache, factory, navigationManager)
    {
        this._logger = logger;
    }

    public async Task<List<BusinessProfileDTO>> Read() //Eventually Pass in a Filter Object
    {
        var result = new List<BusinessProfileDTO> { };
        try
        {
            var uri = new Uri($"{_navigationManager.BaseUri}{Paths.Inbox_API_View}");
            var response = await _httpClient.GetAsync(uri);
            result = await response.ReadAsync<List<BusinessProfileDTO>>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
        }

        return result;
    }

    public async Task<BusinessProfileDTO> GetTask(string id)
    {
        var result = new BusinessProfileDTO() 
        { 
            Id = string.Empty,
            Name = string.Empty,
            UserId = string.Empty,
            Emails = new List<Email>(),
            Phones = new List<Phone>()
        };
        try
        {
            var uri = new Uri($"{_navigationManager.BaseUri}{Paths.Tasks_API_View}/{id}");
            var response = await _httpClient.GetAsync(uri);
            result = await response.ReadAsync<BusinessProfileDTO>();
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

    public async Task<ResponseResult<BusinessProfileDTO>> Save(BusinessProfileDTO item)
    {
        var r = new ResponseResult<BusinessProfileDTO>();
        try
        {
            var uri = new Uri($"{_navigationManager.BaseUri}{Paths.Tasks_API_Save}");

            var jsonText = JsonSerializer.Serialize(item);

            var response = await _httpClient.PostAsJsonAsync<string>(uri, jsonText);
            r = await response.ReadAsync<ResponseResult<BusinessProfileDTO>>();
        }
        catch (Exception ex)
        {
            r.ErrorMessage = "Failed Saving";
            _logger.LogError(ex.Message, ex);
        }
        return r;
    }
}
