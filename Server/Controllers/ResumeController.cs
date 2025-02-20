using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyVideoResume.Abstractions.Core;
using MyVideoResume.Abstractions.Match;
using MyVideoResume.Abstractions.Resume;
using MyVideoResume.AI;
using MyVideoResume.Application.Resume;
using MyVideoResume.Client.Services;
using MyVideoResume.Data;
using MyVideoResume.Data.Models;
using MyVideoResume.Data.Models.Resume;
using MyVideoResume.Documents;
using MyVideoResume.Extensions;
using MyVideoResume.Services;
using MyVideoResume.Web.Common;
using System.Security.Claims;
using System.Text.Json;

namespace MyVideoResume.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public partial class ResumeController : ControllerBase
{
    private readonly IResumePromptEngine _engine;
    private readonly ILogger<ResumeController> _logger;
    private readonly ResumeService _resumeService;
    private readonly MatchService _matchService;
    private readonly DocumentProcessor _documentProcessor;

    public ResumeController(IResumePromptEngine engine, ILogger<ResumeController> logger, ResumeService resumeService, DocumentProcessor documentProcessor, MatchService matchService)
    {
        _logger = logger;
        _engine = engine;
        _resumeService = resumeService;
        _documentProcessor = documentProcessor;
        _matchService = matchService;
    }

    [HttpGet("{resumeId}")]
    public async Task<ActionResult<ResumeInformationEntity>> Get(string resumeId)
    {
        var result = new ResumeInformationEntity();
        try
        {
            result = await _resumeService.GetResume(resumeId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
        }
        return result;
    }

    [HttpGet("GetResumesPublic")]
    public async Task<ActionResult<List<ResumeInformationSummaryDTO>>> GetResumesPublic()
    {
        var result = new List<ResumeInformationSummaryDTO>();
        try
        {
            result = await _resumeService.GetResumeInformationSummaryData(onlyPublic: true, take: 5);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
        }
        return result;
    }

    [Authorize]
    [HttpGet("GetResumesOwnedbyAuthUser")]
    public async Task<ActionResult<List<ResumeInformationSummaryDTO>>> GetResumesOwnedbyAuthUser()
    {
        var result = new List<ResumeInformationSummaryDTO>();
        try
        {
            var id = User.FindFirstValue(ClaimTypes.NameIdentifier); //Users Resumes
            result = await _resumeService.GetResumeInformationSummaryData(id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
        }
        return result;
    }

    [Authorize]
    [HttpGet]
    public async Task<ActionResult<List<ResumeInformationEntity>>> Get()
    {
        var result = new List<ResumeInformationEntity>();
        try
        {
            var id = User.FindFirstValue(ClaimTypes.NameIdentifier);
            result = await _resumeService.GetResumes(id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
        }
        return result;
    }

    [HttpPost("Save")]
    public async Task<ActionResult<ResponseResult<ResumeInformationEntity>>> Save([FromBody] string resumeJson)
    {
        var result = new ResponseResult<ResumeInformationEntity>();
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            result = await _resumeService.CreateResumeInformation(userId, resumeJson);
        }
        catch (Exception ex)
        {
            result.ErrorMessage = "Error Saving";
            _logger.LogError(ex.Message, ex);
        }
        return result;
    }

    [Authorize]
    [HttpPost("{resumeId}")]
    public async Task<ActionResult<ResponseResult>> Delete(string resumeId)
    {
        var result = new ResponseResult();
        try
        {
            var id = User.FindFirstValue(ClaimTypes.NameIdentifier);
            result = await _resumeService.DeleteResume(id, resumeId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            result.Result = ex.Message;
        }
        return result;
    }

    [Authorize]
    [HttpPost("default/{resumeId}")]
    public async Task<ActionResult<ResponseResult<bool>>> SetDefaultResume(string resumeId)
    {
        var result = new ResponseResult<bool>();
        try
        {
            var id = User.FindFirstValue(ClaimTypes.NameIdentifier);
            result = await _resumeService.SetDefaultResume(id, resumeId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            result.ErrorMessage = ex.Message;
        }
        return result;
    }

    [Authorize]
    [HttpPost("watch/{resumeId}")]
    public async Task<ActionResult<ResponseResult<bool>>> WatchResume(string resumeId)
    {
        var result = new ResponseResult<bool>();
        try
        {
            var id = User.FindFirstValue(ClaimTypes.NameIdentifier);
            result = await _resumeService.WatchResume(id, resumeId, true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            result.ErrorMessage = ex.Message;
        }
        return result;
    }

    [Authorize]
    [HttpPost("unwatch/{resumeId}")]
    public async Task<ActionResult<ResponseResult<bool>>> UnwatchResume(string resumeId)
    {
        var result = new ResponseResult<bool>();
        try
        {
            var id = User.FindFirstValue(ClaimTypes.NameIdentifier);
            result = await _resumeService.WatchResume(id, resumeId, false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            result.ErrorMessage = ex.Message;
        }
        return result;
    }


    [HttpPost("SentimentPrediction")]
    public async Task<ActionResult<ResponseResult<float>>> SentimentPrediction([FromBody] string id)
    {
        var result = new ResponseResult<float>();
        try
        {
            result = await _resumeService.GetSentimentPrediction(id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            result.ErrorMessage = ex.Message;
        }
        return result;
    }


    [HttpPost("Summarize")]
    public async Task<ActionResult<ResponseResult>> Summarize([FromBody] string resumeText)
    {
        var result = new ResponseResult();
        try
        {
            result = await _engine.SummarizeResume(resumeText);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            result.Result = ex.Message;
        }
        return result;
    }

    [HttpPost("Parse")]
    public async Task<ActionResult<ResponseResult>> Parse(IFormFile file)
    {
        var result = new ResponseResult();
        try
        {
            if (file != null)
            {
                result = await _engine.ResumeParseJSON(_documentProcessor.ConvertToString(file));
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            result.Result = ex.Message;
        }
        return result;
    }

    [Authorize]
    [HttpPost("CreateFromFile")]
    public async Task<ActionResult<ResponseResult<ResumeInformationEntity>>> CreateFromFile(IFormFile file)
    {
        var result = new ResponseResult<ResumeInformationEntity>();
        try
        {
            if (file != null)
            {
                //Lets Verify if the user is logged in.. If so, we'll create a resume.
                var id = User.FindFirstValue(ClaimTypes.NameIdentifier);

                var resumeJson = _documentProcessor.ConvertToString(file);
                if (file.ContentType == "application/json")
                {
                    result = await _resumeService.CreateResume(id, resumeJson);
                }
                else
                {
                    var temppdfresult = await _engine.ResumeParseJSON(resumeJson);
                    if (!temppdfresult.ErrorMessage.HasValue())
                    {
                        //Remove the Markdown from the Response
                        var resume = temppdfresult.Result;
                        result = await _resumeService.CreateResume(id, resume);
                    }
                    else
                    {
                        result.ErrorMessage = temppdfresult.ErrorMessage;
                    }
                }

                //TODO: Check the Account if Paid or if the profile has it turned on
                //Queue the Resume to be Matched
                if (!result.ErrorMessage.HasValue())
                {
                    result = await _resumeService.QueueResumeToJobRequest(result);
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
}
