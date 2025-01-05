using Microsoft.AspNetCore.HeaderPropagation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using MyVideoResume.Abstractions.Core;
using MyVideoResume.Abstractions.Job;
using MyVideoResume.Abstractions.Resume;
using MyVideoResume.Application.Resume;
using MyVideoResume.Data;
using MyVideoResume.Data.Models;
using MyVideoResume.Web.Common;

namespace MyVideoResume.Application.Job;

public partial class JobService
{
    private readonly ILogger<JobService> _logger;
    private readonly DataContext _dataContext;
    private readonly IConfiguration _configuration;
    private readonly AccountService _accountService;
    private readonly IJobPromptEngine _engine;
    private readonly HttpClient _httpClient;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public JobService(ILogger<JobService> logger, IJobPromptEngine engine, IConfiguration configuration, DataContext context, AccountService accountService, IHttpClientFactory httpClientFactory, IServiceScopeFactory serviceScopeFactory)
    {
        _dataContext = context;
        _logger = logger;
        _configuration = configuration;
        _accountService = accountService;
        _engine = engine;
        _httpClient = httpClientFactory.CreateClient(Constants.HttpClientFactory);
        _serviceScopeFactory = serviceScopeFactory;
    }

    public async Task<JobPreferencesEntity> GetJobPreferences(string userId)
    {
        throw new NotImplementedException();
    }

    public async Task<ResponseResult<JobSummaryItem>> SaveJobByUrl(string url)
    {
        var result = new ResponseResult<JobSummaryItem>();

        //call the URL
        using IServiceScope scope = _serviceScopeFactory.CreateScope();
        var headerPropagationValues = scope.ServiceProvider.GetRequiredService<HeaderPropagationValues>();
        headerPropagationValues.Headers = new Dictionary<string, StringValues>(StringComparer.OrdinalIgnoreCase);
        //eventually set headers coming from other sources (e.g. consuming a queue) 
        headerPropagationValues.Headers.Add("User-Agent", "background-service");

        var response = await _httpClient.GetStringAsync(url);
        //call the AI
        result = await _engine.ExtractJob(response);

        return result;
    }

    //Get All Public Resume Summaries
    public async Task<List<JobSummaryItem>> GetJobSummaryItems(string? userId = null, bool? onlyPublic = null)
    {
        var result = new List<JobSummaryItem>();
        try
        {
            var query = _dataContext.Jobs
                .AsNoTracking()
                .Where(x => x.DeletedDateTime == null);

            if (onlyPublic.HasValue)
            {
                query = query.Where(x => x.Status == JobStatus.Open);
            }

            if (!string.IsNullOrEmpty(userId))
            {
                query = query.Where(x => x.UserId == userId);
            }

            result = query.Select(x => new JobSummaryItem() { UserId = x.UserId, CreationDateTimeFormatted = x.CreationDateTime.Value.ToString("yyyy-MM-dd"), Id = x.Id.ToString(), Responsibilities = x.Responsibilities, Requirements = x.Requirements, Slug = x.Slug, Title = x.Title, Description = x.Description, ATSApplyUrl = x.ATSApplyUrl, OriginalWebsiteUrl = x.OriginalWebsiteUrl }).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
        }
        return result;
    }


}
