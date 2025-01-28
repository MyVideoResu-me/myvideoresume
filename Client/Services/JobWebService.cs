using System.Text.Json;
using Microsoft.AspNetCore.Components;
using Radzen;
using MyVideoResume.Abstractions.Core;
using MyVideoResume.Abstractions.Job;
using MyVideoResume.Web.Common;
using System.Net.Http.Json;
using MyVideoResume.Data.Models.Jobs;

namespace MyVideoResume.Client.Services;

public partial class JobWebService
{
    private readonly HttpClient _httpClient;
    private readonly NavigationManager _navigationManager;
    private readonly ILogger<JobWebService> _logger;

    public JobWebService(NavigationManager navigationManager, IHttpClientFactory factory, ILogger<JobWebService> logger)
    {
        this._httpClient = factory.CreateClient(Constants.HttpClientFactory);
        this._navigationManager = navigationManager;
        this._logger = logger;
    }

    public async Task<List<JobItemDTO>> GetPublicJobs() //Eventually Pass in a Search Object
    {
        var result = new List<JobItemDTO> { };
        try
        {
            var uri = new Uri($"{_navigationManager.BaseUri}{Paths.Jobs_API_View}");
            var response = await _httpClient.GetAsync(uri);
            result = await response.ReadAsync<List<JobItemDTO>>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
        }

        return result;
    }

    public async Task<List<JobItemDTO>> GetJobSummaryItems() //Eventually Pass in a Search Object
    {
        var result = new List<JobItemDTO> { };
        try
        {
            var uri = new Uri($"{_navigationManager.BaseUri}{Paths.Jobs_API_SummaryItems}");
            var response = await _httpClient.GetAsync(uri);
            result = await response.ReadAsync<List<JobItemDTO>>();
            result = result.OrderByDescending(x => x.CreationDateTimeFormatted).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
        }

        return result;
    }


    public async Task<JobItemEntity> GetJob(string id)
    {
        var result = new JobItemEntity();
        try
        {
            var uri = new Uri($"{_navigationManager.BaseUri}api/job/{id}");
            var response = await _httpClient.GetAsync(uri);
            result = await response.ReadAsync<JobItemEntity>();
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
            //var uri = new Uri($"{_navigationManager.BaseUri}{string.Format(Paths.Jobs_API_ViewById, id)}");
            var uri = new Uri($"{_navigationManager.BaseUri}api/job/{id}");
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

    public async Task<ResponseResult<JobItemEntity>> Save(JobItemEntity item)
    {
        var r = new ResponseResult<JobItemEntity>();
        try
        {
            var uri = new Uri($"{_navigationManager.BaseUri}{Paths.Jobs_API_Save}");

            var jsonText = JsonSerializer.Serialize(item);

            var response = await _httpClient.PostAsJsonAsync<string>(uri, jsonText);
            r = await response.ReadAsync<ResponseResult<JobItemEntity>>();
        }
        catch (Exception ex)
        {
            r.ErrorMessage = "Failed Saving";
            _logger.LogError(ex.Message, ex);
        }
        return r;
    }

    public async Task<ResponseResult<JobItemEntity>> Extract(string url)
    {
        var uri = new Uri($"{_navigationManager.BaseUri}{Paths.Jobs_API_Extract}");
        var response = await _httpClient.PostAsJsonAsync<string>(uri, url);
        var r = await response.ReadAsync<ResponseResult<JobItemEntity>>();
        return r;
    }
}