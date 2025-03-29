using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyVideoResume.Abstractions.Core;
using MyVideoResume.Abstractions.Job;
using MyVideoResume.Abstractions.Match;
using MyVideoResume.Abstractions.Resume.Formats.JSONResumeFormat;
using MyVideoResume.Application;
using MyVideoResume.Application.Job;
using MyVideoResume.Data.Models.Jobs;
using MyVideoResume.Extensions;
using System.Security.Claims;

namespace MyVideoResume.API.Controllers;

public class ChromeResponse
{
    public string MarkdownResume { get; set; }
    public string SummaryRecommendations { get; set; }
    public float OldScore { get; set; }
    public float NewScore { get; set; }
}

[Route("[controller]")]
[ApiController]
public partial class ChromeController : ControllerBase
{
    private readonly ILogger<ChromeController> _logger;
    private readonly Processor _processor;

    public ChromeController(ILogger<ChromeController> logger, Processor processor)
    {
        _logger = logger;
        _processor = processor;
    }

    [Authorize]
    [HttpPost("createjobbestmatch")]
    public async Task<ActionResult<ResponseResult<ChromeResponse>>> CreateJobBestMatch([FromBody] JobChromeRequest jobChromeRequest)
    {
        var id = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var result = new ResponseResult<ChromeResponse>();
        try
        {
            var tempresult = await _processor.ProcessChromeExtensionRequest(id, jobChromeRequest);
            if (!tempresult.ErrorMessage.HasValue() && tempresult.Result != null)
            {
                result.Result = new ChromeResponse
                {
                    MarkdownResume = tempresult.Result.Resume.ToMarkdown(),
                    SummaryRecommendations = tempresult.Result.SummaryRecommendations,
                    OldScore = tempresult.Result.OldScore,
                    NewScore = tempresult.Result.NewScore
                };
            }
            else
            {
                result.ErrorMessage = tempresult.ErrorMessage;
                result.ErrorCode = tempresult.ErrorCode;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            result.ErrorMessage = ex.Message;
            result.ErrorCode = ErrorCodes.SystemError;
        }
        return Ok(result);
    }

    [Authorize]
    [HttpPost("jobresumeanalysis")]
    public async Task<ActionResult<ResponseResult<JobResumeMatchResponse>>> JobResumeAnalysis([FromBody] JobChromeRequest jobChromeRequest)
    {
        var id = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var result = new ResponseResult<JobResumeMatchResponse>();
        try
        {
            result = await _processor.ProcessJobResumeMatch(id, jobChromeRequest);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            result.ErrorMessage = ex.Message;
            result.ErrorCode = ErrorCodes.SystemError;
        }
        return Ok(result);
    }
}
