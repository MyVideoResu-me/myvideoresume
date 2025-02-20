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
using MyVideoResume.Abstractions.Resume;
using MyVideoResume.Extensions;

namespace MyVideoResume.Client.Services;

public partial class MatchWebService : BaseWebService
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
            string combinedValue = jobDescription + resumeId;
            string combinedKey = combinedValue.GenerateSHA256Hash();

            var cachedResult = await _cache.GetOrCreateAsync(combinedKey, async (x) =>
            {
                var uri = new Uri($"{_navigationManager.BaseUri}api/match");
                var request = new JobContentResumeIdMatchRequest() { JobContent = jobDescription, ResumeId = resumeId };
                var response = await _httpClient.PostAsJsonAsync<JobContentResumeIdMatchRequest>(uri, request);
                return r = await response.ReadAsync<ResponseResult<JobResumeMatchResponse>>();
            });

            r = cachedResult;
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
            string combinedValue = jobId + resumeId;
            string combinedKey = combinedValue.GenerateSHA256Hash();

            var cachedResult = await _cache.GetOrCreateAsync(combinedKey, async (x) =>
            {
                var uri = new Uri($"{_navigationManager.BaseUri}api/match/byid");
                var request = new JobResumeIdMatchRequest() { JobId = jobId, ResumeId = resumeId };
                var response = await _httpClient.PostAsJsonAsync<JobResumeIdMatchRequest>(uri, request);
                return r = await response.ReadAsync<ResponseResult<JobResumeMatchResponse>>();
            });

            r = cachedResult;

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
            string combinedValue = jobDescription + resumeContent + "MatchByJobResumeContent";
            string combinedKey = combinedValue.GenerateSHA256Hash();

            var cachedResult = await _cache.GetOrCreateAsync(combinedKey, async (x) =>
            {
                var uri = new Uri($"{_navigationManager.BaseUri}api/match/bycontent");
                var request = new JobResumeByContentMatchRequest() { JobContent = jobDescription, ResumeContent = resumeContent };
                var response = await _httpClient.PostAsJsonAsync<JobResumeByContentMatchRequest>(uri, request);
                return r = await response.ReadAsync<ResponseResult<JobResumeMatchResponse>>();
            });

            r = cachedResult;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            r.ErrorMessage = ex.Message;
        }
        return r;
    }

    public async Task<ResponseResult<JobResumeBestResumeResponse>> BestResumeByJobResumeContent(string jobDescription, string resumeContent)
    {
        var r = new ResponseResult<JobResumeBestResumeResponse>();
        try
        {
            string combinedValue = jobDescription + resumeContent + "BestResumeByJobResumeContent";
            string combinedKey = combinedValue.GenerateSHA256Hash();

            var cachedResult = await _cache.GetOrCreateAsync(combinedKey, async (x) =>
            {
                var uri = new Uri($"{_navigationManager.BaseUri}api/match/bestresume");
                var request = new JobResumeByContentMatchRequest() { JobContent = jobDescription, ResumeContent = resumeContent };
                var response = await _httpClient.PostAsJsonAsync<JobResumeByContentMatchRequest>(uri, request);
                r = await response.ReadAsync<ResponseResult<JobResumeBestResumeResponse>>();
                if (r.ErrorMessage.HasValue())
                    return null;
                else
                    return r;
            });

            if (cachedResult == null)
                await _cache.RemoveAsync(combinedKey);
            
            r = cachedResult;

        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            r.ErrorMessage = ex.Message;
        }
        return r;
    }
}