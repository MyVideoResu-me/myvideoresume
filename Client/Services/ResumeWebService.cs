using System.Text.Json;
using Microsoft.AspNetCore.Components;
using Radzen;
using MyVideoResume.Data.Models.Resume;
using MyVideoResume.Abstractions.Core;
using MyVideoResume.Abstractions.Resume;
using MyVideoResume.Web.Common;
using System.Net.Http.Json;
using Microsoft.Extensions.Caching.Hybrid;
//using Refit;

namespace MyVideoResume.Client.Services;

public partial class ResumeWebService : BaseWebService
{
    protected HybridCache _cache;
    private readonly MatchWebService matchWebService;
    private readonly SecurityWebService _securityService;
    private readonly ILogger<ResumeWebService> _logger;

    public ResumeWebService(HybridCache cache, NavigationManager navigationManager, IHttpClientFactory factory, ILogger<ResumeWebService> logger, SecurityWebService securityService, MatchWebService matchWebService) : base(cache, factory, navigationManager)
    {
        this._logger = logger;
        this._securityService = securityService;
        this.matchWebService = matchWebService;
        this._cache = cache;
    }

    #region AI / Summarize, Sentiment Analysis
    public async Task<ResponseResult<float>> GetSentimentAnalysisById(string id)
    {
        var result = new ResponseResult<float>();
        var uri = new Uri($"{_navigationManager.BaseUri}{Paths.Resume_API_Sentiment}");
        var response = await _httpClient.PostAsJsonAsync<string>(uri, id);
        result = await response.ReadAsync<ResponseResult<float>>();

        return result;
    }

    public async Task<float> GetSentimentAnalysisByText(string resumeAsText)
    {
        var uri = new Uri($"{_navigationManager.BaseUri}{Paths.AI_API_Sentiment}");
        var response = await _httpClient.PostAsJsonAsync<string>(uri, resumeAsText);
        return await response.ReadAsync<float>();
    }

    public async Task<ResponseResult> Summarize(string resume)
    {
        var uri = new Uri($"{_navigationManager.BaseUri}api/resume/summarize");
        var response = await _httpClient.PostAsJsonAsync<string>(uri, resume);
        var r = await response.ReadAsync<ResponseResult>();
        return r;
    }

    #endregion

    #region Homepage
    public async Task<List<ResumeInformationSummaryDTO>> GetResumesPublic() //Eventually Pass in a Search Object
    {
        var result = new List<ResumeInformationSummaryDTO> { };
        try
        {
            var cachedResult = await _cache.GetOrCreateAsync(CacheKeys.PublicResumes, async (x) =>
            {
                var uri = new Uri($"{_navigationManager.BaseUri}api/resume/GetResumesPublic");
                var response = await _httpClient.GetAsync(uri);
                return result = await response.ReadAsync<List<ResumeInformationSummaryDTO>>();
            });
            result = cachedResult;
            if (result.Count == 0)
            {
                await _cache.RemoveAsync(CacheKeys.PublicResumes);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
        }

        return result;
    }
    #endregion

    #region Resume List
    public async Task<List<ResumeInformationSummaryDTO>> GetResumesOwnedbyAuthUser() //Eventually Pass in a Search Object
    {
        var result = new List<ResumeInformationSummaryDTO> { };
        try
        {
            var cachekeyUserOwnedResumes = $"{CacheKeys.UserResumes}{_securityService.User.Id}";
            var cachedResult = await _cache.GetOrCreateAsync(cachekeyUserOwnedResumes, async (x) =>
            {
                var uri = new Uri($"{_navigationManager.BaseUri}api/resume/GetResumesOwnedbyAuthUser");
                var response = await _httpClient.GetAsync(uri);
                result = await response.ReadAsync<List<ResumeInformationSummaryDTO>>();
                return result = result.OrderByDescending(x => x.CreationDateTimeFormatted).ToList();
            });
            result = cachedResult;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
        }

        return result;
    }
    #endregion

    public async Task<List<ResumeInformationEntity>> GetResumes()
    {
        var result = new List<ResumeInformationEntity> { };
        try
        {
            var uri = new Uri($"{_navigationManager.BaseUri}api/resume");
            var response = await _httpClient.GetAsync(uri);
            result = await response.ReadAsync<List<ResumeInformationEntity>>();
            result = result.OrderByDescending(x => x.CreationDateTime).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
        }

        return result;
    }

    public async Task<ResumeInformationDTO> GetResume(string resumeId)
    {
        var result = new ResumeInformationDTO();
        try
        {
            var cachedResult = await _cache.GetOrCreateAsync(resumeId, async (x) =>
            {
                var uri = new Uri($"{_navigationManager.BaseUri}api/resume/{resumeId}");
                var response = await _httpClient.GetAsync(uri);
                return result = await response.ReadAsync<ResumeInformationDTO>();
            });
            result = cachedResult;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
        }

        return result;
    }

    public async Task<ResponseResult> Delete(string resumeId)
    {
        var result = new ResponseResult();
        try
        {
            var uri = new Uri($"{_navigationManager.BaseUri}api/resume/{resumeId}");
            var content = new FormUrlEncodedContent(new Dictionary<string, string> { { "resumeId", resumeId } });
            var response = await _httpClient.PostAsync(uri, content);
            result = await response.ReadAsync<ResponseResult>();
            await _cache.RemoveAsync(resumeId);
            var cachekeyUserOwnedResumes = $"{CacheKeys.UserResumes}{_securityService.User.Id}";
            await _cache.RemoveAsync(cachekeyUserOwnedResumes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            result.ErrorMessage = "Error Deleting.";
        }
        return result;
    }

    public async Task<ResponseResult<ResumeInformationDTO>> Save(ResumeInformationDTO resume)
    {
        var r = new ResponseResult<ResumeInformationDTO>();
        try
        {
            var uri = new Uri($"{_navigationManager.BaseUri}{Paths.Resume_API_Save}");

            var jsonText = JsonSerializer.Serialize(resume);

            var response = await _httpClient.PostAsJsonAsync<string>(uri, jsonText);
            r = await response.ReadAsync<ResponseResult<ResumeInformationDTO>>();

            var cachekeyUserOwnedResumes = $"{CacheKeys.UserResumes}{_securityService.User.Id}";
            await _cache.RemoveAsync(cachekeyUserOwnedResumes);

        }
        catch (Exception ex)
        {
            r.ErrorMessage = "Failed Saving";
            _logger.LogError(ex.Message, ex);
        }
        return r;
    }
}