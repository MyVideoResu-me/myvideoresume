using Microsoft.AspNetCore.Components;
using MyVideoResume.Abstractions.Core;
using MyVideoResume.Abstractions.Resume;
using Microsoft.Extensions.Caching.Hybrid;

namespace MyVideoResume.Client.Services;

public partial class DashboardWebService : BaseWebService
{
    private readonly ResumeWebService _resumeService;

    private readonly ILogger<DashboardWebService> _logger;

    public DashboardWebService(HybridCache cache, NavigationManager navigationManager, IHttpClientFactory factory, ILogger<DashboardWebService> logger, ResumeWebService resumeService) : base(cache, factory, navigationManager)
    {
        _logger = logger;
        _resumeService = resumeService;
    }
    public async Task<List<ResumeSummaryItem>> GetResumeSummaries()
    {
        var result = await _resumeService.GetResumeSummaryItems();
        return result;
    }

    public async Task<ResponseResult> Delete(string resumeId)
    {
        var result = await _resumeService.Delete(resumeId);
        return result;
    }
}