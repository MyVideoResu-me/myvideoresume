using System.Text.Json;
using Microsoft.AspNetCore.Components;
using Radzen;
using MyVideoResume.Abstractions.Core;
using MyVideoResume.Abstractions.Job;
using MyVideoResume.Web.Common;
using System.Net.Http.Json;
using MyVideoResume.Data.Models.Jobs;
using MyVideoResume.Abstractions.Match;
using Microsoft.Extensions.Caching.Hybrid;

namespace MyVideoResume.Client.Services;

public partial class MatchWebService: BaseWebService
{
    private readonly ILogger<MatchWebService> _logger;

    public MatchWebService(HybridCache cache, NavigationManager navigationManager, IHttpClientFactory factory, ILogger<MatchWebService> logger) : base(cache, factory, navigationManager)
    {
        this._logger = logger;
    }

    public async Task<ResponseResult<JobResumeMatchResponse>> MatchByJobContentResumeId(string jobDescription, string resumeId)
    {
        var r = new ResponseResult<JobResumeMatchResponse>();
        try
        {
            var uri = new Uri($"{_navigationManager.BaseUri}api/match");
            var request = new JobContentResumeIdMatchRequest() { JobContent = jobDescription, ResumeId = resumeId };
            var response = await _httpClient.PostAsJsonAsync<JobContentResumeIdMatchRequest>(uri, request);
            r = await response.ReadAsync<ResponseResult<JobResumeMatchResponse>>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            r.ErrorMessage = ex.Message;
        }
        return r;
    }

    public async Task<ResponseResult<JobResumeMatchResponse>> MatchByJobResumeId(string jobId, string resumeId)
    {
        var r = new ResponseResult<JobResumeMatchResponse>();
        try
        {
            var uri = new Uri($"{_navigationManager.BaseUri}api/match/byid");
            var request = new JobResumeIdMatchRequest() { JobId = jobId, ResumeId = resumeId };
            var response = await _httpClient.PostAsJsonAsync<JobResumeIdMatchRequest>(uri, request);
            r = await response.ReadAsync<ResponseResult<JobResumeMatchResponse>>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            r.ErrorMessage = ex.Message;
        }
        return r;
    }


    public async Task<ResponseResult<JobResumeMatchResponse>> MatchByJobResumeContent(string jobDescription, string resumeContent)
    {
        var r = new ResponseResult<JobResumeMatchResponse>();
        try
        {
            var uri = new Uri($"{_navigationManager.BaseUri}api/match/bycontent");
            var request = new JobResumeByContentMatchRequest() { JobContent = jobDescription, ResumeContent = resumeContent };
            var response = await _httpClient.PostAsJsonAsync<JobResumeByContentMatchRequest>(uri, request);
            r = await response.ReadAsync<ResponseResult<JobResumeMatchResponse>>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            r.ErrorMessage = ex.Message;
        }
        return r;
    }

}