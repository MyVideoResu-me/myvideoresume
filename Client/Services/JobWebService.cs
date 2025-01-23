using System;
using System.Web;
using System.Linq;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text;
using System.Text.Json;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Radzen;
using MyVideoResume.Data.Models;
using MyVideoResume.Client.Shared.Security.Recaptcha;
using MyVideoResume.Data;
using Microsoft.EntityFrameworkCore;
using MyVideoResume.Data.Models.Resume;
using MyVideoResume.Abstractions.Core;
using static System.Net.WebRequestMethods;
using MyVideoResume.Abstractions.Job;
using MyVideoResume.Abstractions.Resume;
using MyVideoResume.Client.Shared;
using MyVideoResume.Web.Common;
using System.Net.Http.Json;
using MyVideoResume.Data.Models.Jobs;
//using Refit;

namespace MyVideoResume.Client.Services;

public partial class JobWebService
{
    private readonly HttpClient _httpClient;
    private readonly NavigationManager _navigationManager;
    private readonly ILogger<DashboardWebService> _logger;

    public JobWebService(NavigationManager navigationManager, IHttpClientFactory factory, ILogger<DashboardWebService> logger)
    {
        this._httpClient = factory.CreateClient(Constants.HttpClientFactory);
        this._navigationManager = navigationManager;
        this._logger = logger;
    }

    public async Task<List<JobSummaryItem>> GetPublicJobs() //Eventually Pass in a Search Object
    {
        var result = new List<JobSummaryItem> { };
        try
        {
            var uri = new Uri($"{_navigationManager.BaseUri}{Paths.Jobs_API_View}");
            var response = await _httpClient.GetAsync(uri);
            result = await response.ReadAsync<List<JobSummaryItem>>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
        }

        return result;
    }

    public async Task<List<JobSummaryItem>> GetJobSummaryItems() //Eventually Pass in a Search Object
    {
        var result = new List<JobSummaryItem> { };
        try
        {
            var uri = new Uri($"{_navigationManager.BaseUri}{Paths.Jobs_API_SummaryItems}");
            var response = await _httpClient.GetAsync(uri);
            result = await response.ReadAsync<List<JobSummaryItem>>();
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