using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyVideoResume.Abstractions.Core;
using MyVideoResume.Abstractions.Job;
using MyVideoResume.Application.Job;
using MyVideoResume.Data.Models;
using MyVideoResume.Data.Models.Resume;
using MyVideoResume.Documents;
using MyVideoResume.Web.Common;
using System.Security.Claims;

namespace MyVideoResume.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public partial class JobController : ControllerBase
{
    private readonly IJobPromptEngine _engine;
    private readonly ILogger<JobController> _logger;
    private readonly JobService _service;
    private readonly DocumentProcessor _documentProcessor;

    public JobController(IJobPromptEngine engine, ILogger<JobController> logger, JobService resumeService, DocumentProcessor documentProcessor)
    {
        _logger = logger;
        _engine = engine;
        _service = resumeService;
        _documentProcessor = documentProcessor;
    }

    [HttpGet("getjobpreferences/{userId}")]
    public JobPreferencesEntity GetJobPreferences(string userId)
    {
        return null;
    }

    [HttpPost("savejobpreferences/{userId}")]
    public JobPreferencesEntity SaveJobPreferences(string userId, [FromBody] JobPreferences preferences)
    {
        return null;
    }

    //[HttpGet("{id}")]
    //public async Task<ActionResult<JobItemEntity>> Get(string id)
    //{
    //    var result = new JobItemEntity();
    //    try
    //    {
    //        result = await _service.GetJob(id);
    //    }
    //    catch (Exception ex)
    //    {
    //        _logger.LogError(ex.Message, ex);
    //    }
    //    return result;
    //}

    [HttpGet("GetPublicJobs")]
    public async Task<ActionResult<List<JobSummaryItem>>> GetPublicJobs()
    {
        var result = new List<JobSummaryItem>();
        try
        {
            result = await _service.GetJobSummaryItems(onlyPublic: true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
        }
        return result;
    }

    [Authorize]
    [HttpGet("GetSummaryItems")]
    public async Task<ActionResult<List<JobSummaryItem>>> GetSummaryItems()
    {
        var result = new List<JobSummaryItem>();
        try
        {
            var id = User.FindFirstValue(ClaimTypes.NameIdentifier);
            result = await _service.GetJobSummaryItems(id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
        }
        return result;
    }

    //[Authorize]
    //[HttpGet]
    //public async Task<ActionResult<List<ResumeInformationEntity>>> Get()
    //{
    //    var result = new List<ResumeInformationEntity>();
    //    try
    //    {
    //        var id = User.FindFirstValue(ClaimTypes.NameIdentifier);
    //        result = await _service.GetResumes(id);
    //    }
    //    catch (Exception ex)
    //    {
    //        _logger.LogError(ex.Message, ex);
    //    }
    //    return result;
    //}

    //[HttpPost("Save")]
    //public async Task<ActionResult<ResponseResult<ResumeInformationEntity>>> Save([FromBody] string resumeJson)
    //{
    //    var result = new ResponseResult<ResumeInformationEntity>();
    //    try
    //    {
    //        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
    //        result = await _service.CreateResumeInformation(userId, resumeJson);
    //    }
    //    catch (Exception ex)
    //    {
    //        result.ErrorMessage = "Error Saving";
    //        _logger.LogError(ex.Message, ex);
    //    }
    //    return result;

    //}

    //[Authorize]
    //[HttpPost("{resumeId}")]
    //public async Task<ActionResult<ResponseResult>> Delete(string resumeId)
    //{
    //    var result = new ResponseResult();
    //    try
    //    {
    //        var id = User.FindFirstValue(ClaimTypes.NameIdentifier);
    //        result = await _service.DeleteResume(id, resumeId);
    //    }
    //    catch (Exception ex)
    //    {
    //        _logger.LogError(ex.Message, ex);
    //        result.Result = ex.Message;
    //    }
    //    return result;
    //}

    [HttpPost("Extract")]
    public async Task<ActionResult<ResponseResult<JobSummaryItem>>> Extract([FromBody] string url)
    {
        var result = new ResponseResult<JobSummaryItem>();
        try
        {
            result = await _service.SaveJobByUrl(url);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            result.ErrorMessage = ex.Message;
        }
        return result;
    }

    [HttpPost("Parse")]
    public async Task<ActionResult<ResponseResult<string>>> Parse(IFormFile file)
    {
        var result = new ResponseResult<string>();
        try
        {
            if (file != null)
            {
                result = await _engine.JobParseJSON(file);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            result.ErrorMessage = ex.Message;
        }
        return result;
    }

    //[Authorize]
    //[HttpPost("CreateFromFile")]
    //public async Task<ActionResult<ResponseResult<ResumeInformationEntity>>> CreateFromFile(IFormFile file)
    //{
    //    var result = new ResponseResult<ResumeInformationEntity>();
    //    try
    //    {
    //        if (file != null)
    //        {
    //            //Lets Verify if the user is logged in.. If so, we'll create a resume.
    //            var id = User.FindFirstValue(ClaimTypes.NameIdentifier);

    //            switch (file.ContentType)
    //            {
    //                case "application/json":
    //                    {
    //                        var resumeJson = _documentProcessor.JSONToString(file.OpenReadStream());
    //                        result = await _service.CreateResume(id, resumeJson);
    //                        break;
    //                    }
    //                case "application/pdf":
    //                    {
    //                        var tempresult = await _engine.ResumeParseJSON(file);
    //                        if (!tempresult.ErrorMessage.HasValue())
    //                        {
    //                            //Remove the Markdown from the Response
    //                            var resume = tempresult.Result;
    //                            result = await _service.CreateResume(id, resume);
    //                        }
    //                        else
    //                        {
    //                            result.ErrorMessage = tempresult.ErrorMessage;
    //                        }
    //                        break;
    //                    }
    //                case "application/msword":
    //                    break;
    //                default:
    //                    {
    //                        break;
    //                    }
    //            }
    //        }
    //    }
    //    catch (Exception ex)
    //    {
    //        _logger.LogError(ex.Message, ex);
    //        result.ErrorMessage = ex.Message;
    //    }
    //    return result;
    //}
}
