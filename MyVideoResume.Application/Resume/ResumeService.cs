using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.HeaderPropagation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using MyVideoResume.Abstractions.Core;
using MyVideoResume.Abstractions.Resume;
using MyVideoResume.Data;
using MyVideoResume.Data.Models.Resume;
using MyVideoResume.Web.Common;
using Radzen;
using System.Net.Http.Json;
using System.Text.Json;

namespace MyVideoResume.Application.Resume;

public class ResumeService
{
    private readonly ILogger<ResumeService> _logger;
    private readonly DataContext _dataContext;
    private readonly IConfiguration _configuration;
    private readonly AccountService _accountService;
    private readonly NavigationManager _navigationManager;
    private readonly HttpClient _httpClient;
    private readonly string _baseUri;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public ResumeService(ILogger<ResumeService> logger, IConfiguration configuration, DataContext context, AccountService accountService, NavigationManager navigationManager, IHttpClientFactory httpClientFactory, IServiceScopeFactory serviceScopeFactory)
    {
        _baseUri = configuration.GetValue<string>(Constants.BaseUriConfigurationProperty);
        _dataContext = context;
        _logger = logger;
        _configuration = configuration;
        _accountService = accountService;
        _navigationManager = navigationManager;
        _httpClient = httpClientFactory.CreateClient(Constants.HttpClientFactory);
        _serviceScopeFactory = serviceScopeFactory;
    }

    public async Task<ResponseResult<float>> GetSentimentPrediction(string id)
    {
        var result = new ResponseResult<float>();
        try
        {
            var item = _dataContext.ResumeInformation.FirstOrDefault(x => x.Id == Guid.Parse(id));
            if (item != null)
            {
                var uri = new Uri($"{_baseUri}{Paths.AI_API_Sentiment}");
                using IServiceScope scope = _serviceScopeFactory.CreateScope();
                var headerPropagationValues = scope.ServiceProvider.GetRequiredService<HeaderPropagationValues>();
                headerPropagationValues.Headers = new Dictionary<string, StringValues>(StringComparer.OrdinalIgnoreCase);
                //eventually set headers coming from other sources (e.g. consuming a queue) 
                headerPropagationValues.Headers.Add("User-Agent", "background-service");
                var response = await _httpClient.PostAsJsonAsync<string>(uri, item.ResumeSerialized);
                var valueReturned = await response.ReadAsync<float>();
                item.SentimentScore = valueReturned;
                await _dataContext.SaveChangesAsync();
                result.Result = valueReturned;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            result.ErrorMessage = ex.Message;
        }

        return result;
    }

    //Get All Public Resume Summaries
    public async Task<List<ResumeSummaryItem>> GetResumeSummaryItems(string? userId = null, bool? onlyPublic = null)
    {
        var result = new List<ResumeSummaryItem>();
        try
        {
            var query = _dataContext.ResumeInformation
                .Include(x => x.MetaResume).ThenInclude(y => y.Basics)
                .Include(x => x.MetaData)
                .Include(x => x.UserProfile)
                .Include(x => x.ResumeTemplate)
                .AsNoTracking()
                .Where(x => x.DeletedDateTime == null);

            if (onlyPublic.HasValue)
            {
                query = query.Where(x => x.Privacy_ShowResume == DisplayPrivacy.ToPublic);
            }

            if (!string.IsNullOrEmpty(userId))
            {
                query = query.Where(x => x.UserId == userId);
            }

            result = query.Select(x => new ResumeSummaryItem() { ResumeSerialized = x.ResumeSerialized, SentimentScore = x.SentimentScore, UserId = x.UserId, CreationDateTimeFormatted = x.CreationDateTime.Value.ToString("yyyy-MM-dd"), IsPublic = true, Id = x.Id.ToString(), ResumeTemplateName = x.ResumeTemplate.Name, ResumeSummary = x.MetaResume.Basics.Summary, ResumeSlug = x.Slug, ResumeName = x.MetaResume.Basics.Name }).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
        }
        return result;
    }

    private IQueryable<ResumeInformationEntity> GetReadonlyResume()
    {

        return GetResume().AsNoTracking();
    }

    private IQueryable<ResumeInformationEntity> GetResume()
    {
        var items = _dataContext.ResumeInformation
            .Include(x => x.MetaResume)
            .Include(x => x.MetaResume).ThenInclude(x => x.Publications)
            .Include(x => x.MetaResume).ThenInclude(x => x.Projects)
            .Include(x => x.MetaResume).ThenInclude(x => x.Languages)
            .Include(x => x.MetaResume).ThenInclude(x => x.Work)
            .Include(x => x.MetaResume).ThenInclude(x => x.Awards)
            .Include(x => x.MetaResume).ThenInclude(x => x.References)
            .Include(x => x.MetaResume).ThenInclude(x => x.Basics)
            .Include(x => x.MetaResume).ThenInclude(x => x.Certificates)
            .Include(x => x.MetaResume).ThenInclude(x => x.Education)
            .Include(x => x.MetaResume).ThenInclude(x => x.Interests)
            .Include(x => x.MetaResume).ThenInclude(x => x.Volunteer)
            .Include(x => x.MetaResume).ThenInclude(x => x.Skills)
            .Include(x => x.MetaData)
            .Include(x => x.UserProfile)
            .Include(x => x.ResumeTemplate);
        return items;
    }

    private IQueryable<MetaResumeEntity> GetMetaResume()
    {
        var items = _dataContext.Resumes
            .Include(x => x.Publications)
            .Include(x => x.Projects)
            .Include(x => x.Languages)
            .Include(x => x.Work)
            .Include(x => x.Awards)
            .Include(x => x.References)
            .Include(x => x.Basics)
            .Include(x => x.Certificates)
            .Include(x => x.Education)
            .Include(x => x.Interests)
            .Include(x => x.Volunteer)
            .Include(x => x.Skills);
        return items;
    }

    public async Task<ResumeInformationEntity> GetResume(string resumeId)
    {
        var result = new ResumeInformationEntity();
        try
        {
            //Is it a slug?
            result = GetReadonlyResume().FirstOrDefault(x => x.Slug == resumeId);
            if (result == null)
            {
                result = GetReadonlyResume().FirstOrDefault(x => x.Id == Guid.Parse(resumeId));
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
        }
        return result;
    }

    public async Task<List<ResumeInformationEntity>> GetResumes(string userId)
    {
        var result = new List<ResumeInformationEntity>();
        try
        {
            result = await GetReadonlyResume().Where(x => x.UserId == userId).ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
        }
        return result;
    }

    public async Task<ResponseResult> DeleteResume(string userId, string resumeId)
    {

        var result = new ResponseResult();
        result.ErrorMessage = "Failed to Delete Resume";
        try
        {
            var userProfile = _dataContext.UserProfiles.Include(x => x.ResumeItems).FirstOrDefault(x => x.UserId == userId);
            if (userProfile != null)
            {
                var resume = _dataContext.ResumeInformation.Include(x => x.MetaResume).FirstOrDefault(x => x.Id == Guid.Parse(resumeId));
                if (resume != null)
                {
                    //Check if the User owns the resume
                    if (resume.UserId == userId)
                    {
                        resume.Privacy_ShowContactDetails = DisplayPrivacy.ToSelf;
                        resume.Privacy_ShowResume = DisplayPrivacy.ToSelf;
                        resume.UserId = string.Empty;
                        resume.DeletedDateTime = DateTime.UtcNow;
                        if (resume.MetaResume != null)
                        {
                            resume.MetaResume.UserId = string.Empty;
                            resume.MetaResume.DeletedDateTime = DateTime.UtcNow;
                        }
                        userProfile.ResumeItems.Remove(resume);
                        _dataContext.SaveChanges();
                        result.Result = "Operation Successful";
                        result.ErrorMessage = string.Empty;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            result.ErrorMessage = ex.Message;
        }
        return result;
    }

    public async Task<ResponseResult<ResumeInformationEntity>> CreateResumeInformation(string userId, string resumeText)
    {
        var result = new ResponseResult<ResumeInformationEntity>() { };

        try
        {
            if (userId.HasValue() && resumeText.HasValue()) //should validate that its a real user account...
            {
                var resumeInformation = JsonSerializer.Deserialize<ResumeInformationEntity>(resumeText);
                if (resumeInformation != null)
                {
                    string? existingId = null;

                    var tempResumeInformation = GetResumeInformation(userId, resumeInformation.Id.ToString());
                    if (tempResumeInformation != null)
                        existingId = resumeInformation.Id.ToString();

                    var metaResume = JsonSerializer.Serialize<MetaResumeEntity>(resumeInformation.MetaResume);
                    result = await CreateResume(userId, metaResume, existingId);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            result.ErrorMessage = ex.Message;
        }
        return result;
    }

    private ResumeInformationEntity GetResumeInformation(string userId, string resumeItemId)
    {
        return _dataContext.ResumeInformation.FirstOrDefault(x => x.Id == Guid.Parse(resumeItemId) && x.UserId == userId);
    }

    public async Task<ResponseResult<ResumeInformationEntity>> CreateResume(string userId, string resumeText, string? resumeItemId = null)
    {
        var result = new ResponseResult<ResumeInformationEntity>() { };

        try
        {
            if (!string.IsNullOrWhiteSpace(userId)) //should validate that its a real user account...
            {
                var profile = _dataContext.UserProfiles.Include(x => x.ResumeItems).FirstOrDefault(x => x.UserId == userId);
                if (profile == null)
                {
                    profile = await _accountService.CreateProfile(userId);
                }
                if (profile.ResumeItems == null)
                    profile.ResumeItems = new List<ResumeInformationEntity>();

                // Create the standard template
                var standardTemplate = _dataContext.ResumeTemplates.FirstOrDefault(x => x.TransformerComponentName == "StandardTemplate");
                if (standardTemplate == null)
                {
                    standardTemplate = ResumeTemplateEntity.CreateStandardResumeTemplate();
                    standardTemplate.UserId = userId;
                    _dataContext.ResumeTemplates.Add(standardTemplate);
                    await _dataContext.SaveChangesAsync();
                }

                //Save the Resume to get an object to populate into 
                var tempMetaresume = JsonSerializer.Deserialize<MetaResumeEntity>(resumeText, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                //Does the Meta Resume Exist?
                var existingMetaResume = GetMetaResume().FirstOrDefault(x => x.Id == tempMetaresume.Id && userId == userId);
                if (existingMetaResume != null)
                {
                    //EXISTING
                    tempMetaresume.UpdateDateTime = DateTime.UtcNow;
                    //_dataContext.Entry(existingMetaResume).CurrentValues.SetValues(tempMetaresume);
                }
                else
                {
                    tempMetaresume.UserId = userId;
                    tempMetaresume.CreationDateTime = DateTime.UtcNow;
                    //_dataContext.Resumes.Add(tempMetaresume);
                    //existingMetaResume = tempMetaresume;
                }
                tempMetaresume = _dataContext.InsertUpdateOrDeleteGraph(tempMetaresume, existingMetaResume);
                await _dataContext.SaveChangesAsync();


                var resumeInformation = new ResumeInformationEntity()
                {
                    CreationDateTime = DateTime.UtcNow,
                    UserId = userId,
                    ResumeSerialized = resumeText,
                    Name = tempMetaresume.Basics.Name,
                    Privacy_ShowResume = DisplayPrivacy.ToPublic,
                    Privacy_ShowContactDetails = DisplayPrivacy.ToPublic,
                    ResumeTemplate = standardTemplate,
                    UserProfile = profile
                };

                if (resumeItemId.HasValue())
                {
                    var tempResumeInformation = GetResumeInformation(userId, resumeItemId);
                    if (tempResumeInformation != null)
                    {
                        _dataContext.InsertUpdateOrDeleteGraph(resumeInformation, tempResumeInformation);
                        resumeInformation = tempResumeInformation;
                    }
                }
                else
                    _dataContext.Add(resumeInformation);
                await _dataContext.SaveChangesAsync();

                //Associate the ResumeInfo with the MetaResume
                tempMetaresume.ResumeInformation = resumeInformation;
                resumeInformation.MetaResume = tempMetaresume;
                await _dataContext.SaveChangesAsync();

                result.Result = resumeInformation;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            result.ErrorMessage = ex.Message;
        }
        return result;
    }
}
