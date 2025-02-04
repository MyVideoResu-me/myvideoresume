using Microsoft.AspNetCore.HeaderPropagation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using MyVideoResume.Abstractions.Core;
using MyVideoResume.Abstractions.Job;
using MyVideoResume.Abstractions.Resume;
using MyVideoResume.Application.Account;
using MyVideoResume.Application.Resume;
using MyVideoResume.Data;
using MyVideoResume.Data.Models.Jobs;
using MyVideoResume.Data.Models.Resume;
using MyVideoResume.Web.Common;
using PuppeteerSharp;

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

    public async Task<JobItemEntity> GetJob(string id)
    {
        var item = _dataContext.Jobs.FirstOrDefault(x => x.Id == Guid.Parse(id));

        return item;
    }


    public async Task<ResponseResult> DeleteJob(string userId, string id)
    {

        var result = new ResponseResult();
        result.ErrorMessage = "Failed to Delete";
        try
        {
            var item = _dataContext.Jobs.FirstOrDefault(x => x.Id == Guid.Parse(id) && x.UserId == userId);
            if (item != null)
            {
                item.Slug = string.Empty;
                item.Privacy_ShowJob = DisplayPrivacy.ToSelf;
                item.UserId = string.Empty;
                item.DeletedDateTime = DateTime.UtcNow;
                _dataContext.Jobs.Remove(item);
                _dataContext.SaveChanges();
                result.Result = "Operation Successful";
                result.ErrorMessage = string.Empty;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            result.ErrorMessage = ex.Message;
        }
        return result;
    }

    public async Task<ResponseResult<JobItemEntity>> QueueJobToResumeRequest(ResponseResult<JobItemEntity> result)
    {
        try
        {
            _dataContext.QueueForJobs.Add(new Data.Models.Queues.QueueJobToResumeEntity()
            {
                Job = result.Result,
                CreationDateTime = DateTime.UtcNow,
                Status = BatchProcessStatus.NotStarted
            });
            _dataContext.SaveChanges();
        }
        catch (Exception ex)
        {
            result.ErrorMessage = ex.Message;
            _logger.LogError(ex, ex.Message);
        }

        return result;
    }

    public async Task<JobPreferencesEntity> GetJobPreferences(string userId)
    {
        throw new NotImplementedException();
    }

    public async Task<ResponseResult<JobItemEntity>> CreateJob(string userId, JobItemEntity item)
    {
        var result = new ResponseResult<JobItemEntity>();

        var user = await _dataContext.UserCompanyRolesAssociation.Include(x => x.UserProfile).Include(x => x.CompanyProfile).FirstOrDefaultAsync(x => x.UserProfile.UserId == userId);

        if (user != null)
        {
            _dataContext.Jobs.Add(item);
            item.UserId = userId;
            item.CreatedByUser = user.UserProfile;
            item.CreationDateTime = DateTime.UtcNow;
            _dataContext.SaveChanges();
            result.Result = item;
        }
        return result;
    }

    public async Task<ResponseResult<JobItemEntity>> SaveJobByUrl(string url)
    {
        var result = new ResponseResult<JobItemEntity>();
        try
        {
            var response = string.Empty;
            // Download the Chromium revision if it does not already exist
            await new BrowserFetcher().DownloadAsync();
            using (var browser = await Puppeteer.LaunchAsync(new LaunchOptions { Headless = true, AcceptInsecureCerts = true, Args = new string[] { "--no-sandbox", "--disable-web-security" } }))
            using (var page = await browser.NewPageAsync())
            {
                var res = await page.GoToAsync(url);
                if (res != null)
                {
                    response = await res.TextAsync();


                    if (res.Status == System.Net.HttpStatusCode.Forbidden || response.Contains("You have been blocked"))
                    {
                        result.ErrorMessage = "Failed to Load Job.";
                    }
                    else
                    {
                        result = await _engine.ExtractJob(response);
                    }
                    //var jsSelectAllAnchors = @"Array.from(document.querySelectorAll('a')).map(a => a.href);";
                    //var urls = await page.EvaluateExpressionAsync<string[]>(jsSelectAllAnchors);
                    //foreach (string url in urls)
                    //{
                    //    Console.WriteLine($"Url: {url}");
                    //}
                    //Console.WriteLine("Press any key to continue...");
                    //Console.ReadLine();
                }
                else
                {
                    result.ErrorMessage = "Unable to Process URL";
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            result.ErrorMessage = ex.Message;
        }

        ////call the URL
        //using IServiceScope scope = _serviceScopeFactory.CreateScope();
        //var headerPropagationValues = scope.ServiceProvider.GetRequiredService<HeaderPropagationValues>();
        //headerPropagationValues.Headers = new Dictionary<string, StringValues>(StringComparer.OrdinalIgnoreCase);
        ////eventually set headers coming from other sources (e.g. consuming a queue) 
        //headerPropagationValues.Headers.Add("User-Agent", "background-service");

        //var response = await _httpClient.GetStringAsync(url);
        ////call the AI

        return result;
    }

    //Get All Public Resume Summaries
    public async Task<List<JobItemDTO>> GetPublicJobs(string? userId = null, bool? onlyPublic = null)
    {
        var result = new List<JobItemDTO>();
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

            result = query.Select(x => new JobItemDTO() { UserId = x.UserId, CreationDateTimeFormatted = x.CreationDateTime.Value.ToString("yyyy-MM-dd"), Id = x.Id.ToString(), Responsibilities = x.Responsibilities, Requirements = x.Requirements, Slug = x.Slug, Title = x.Title, Description = x.Description, ATSApplyUrl = x.ATSApplyUrl }).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
        }
        return result;
    }


}
