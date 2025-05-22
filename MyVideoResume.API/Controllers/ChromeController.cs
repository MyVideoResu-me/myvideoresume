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

/// <summary>
/// Controller for handling Chrome extension-related operations.
/// </summary>
[Route("[controller]")]
[ApiController]
public partial class ChromeController : ControllerBase
{
    private readonly ILogger<ChromeController> _logger;
    private readonly Processor _processor;

    /// <summary>
    /// Initializes a new instance of the <see cref="ChromeController"/> class.
    /// </summary>
    /// <param name="logger">The logger for logging errors and information.</param>
    /// <param name="processor">The processor for handling Chrome extension requests.</param>
    public ChromeController(ILogger<ChromeController> logger, Processor processor)
    {
        _logger = logger;
        _processor = processor;
    }

    /// <summary>
    /// Creates a job best match based on the provided Chrome extension request.
    /// </summary>
    /// <param name="jobChromeRequest">The request containing job details from the Chrome extension.</param>
    /// <returns>
    /// A response containing the best match details, including the markdown resume, summary recommendations, and scores.
    /// </returns>
    /// <response code="200">Returns the best match details.</response>
    /// <response code="400">If the request is invalid.</response>
    /// <response code="500">If an internal server error occurs.</response>
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

    /// <summary>
    /// Analyzes a job and resume match based on the provided Chrome extension request.
    /// </summary>
    /// <param name="jobChromeRequest">The request containing job and resume details from the Chrome extension.</param>
    /// <returns>
    /// A response containing the match analysis, including summary recommendations and a score.
    /// </returns>
    /// <response code="200">Returns the match analysis details.</response>
    /// <response code="400">If the request is invalid.</response>
    /// <response code="500">If an internal server error occurs.</response>
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
