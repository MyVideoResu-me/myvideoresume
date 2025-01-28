using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyVideoResume.Abstractions.Business;
using MyVideoResume.Abstractions.Core;
using MyVideoResume.Abstractions.Job;
using MyVideoResume.Abstractions.Match;
using MyVideoResume.Application.Job;
using MyVideoResume.Application.Resume;
using MyVideoResume.Data.Models;
using MyVideoResume.Data.Models.Jobs;
using MyVideoResume.Data.Models.Resume;
using MyVideoResume.Documents;
using MyVideoResume.Web.Common;
using System.Security.Claims;

namespace MyVideoResume.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public partial class MatchController : ControllerBase
{
    private readonly ILogger<MatchController> _logger;
    private readonly MatchService _service;

    public MatchController(ILogger<MatchController> logger, MatchService service)
    {
        _logger = logger;
        _service = service;
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<ResponseResult<JobResumeMatchResponse>>> MatchByJobContentResumeId([FromBody] JobContentResumeIdMatchRequest request)
    {
        var result = new ResponseResult<JobResumeMatchResponse>();
        try
        {
            result = await _service.MatchByJobContentResumeId(request);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            result.ErrorMessage = ex.Message;
        }
        return result;
    }

    [Authorize]
    [HttpPost("byid")]
    public async Task<ActionResult<ResponseResult<JobResumeMatchResponse>>> MatchByJobResumeId([FromBody] JobResumeIdMatchRequest request)
    {
        var result = new ResponseResult<JobResumeMatchResponse>();
        try
        {
            result = await _service.MatchByJobResumeId(request);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            result.ErrorMessage = ex.Message;
        }
        return result;
    }

    [Authorize]
    [HttpPost("bycontent")]
    public async Task<ActionResult<ResponseResult<JobResumeMatchResponse>>> MatchByJobResumeContent([FromBody] JobResumeByContentMatchRequest request)
    {
        var result = new ResponseResult<JobResumeMatchResponse>();
        try
        {
            result = await _service.MatchByJobResumeContent(request);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            result.ErrorMessage = ex.Message;
        }
        return result;
    }

}
