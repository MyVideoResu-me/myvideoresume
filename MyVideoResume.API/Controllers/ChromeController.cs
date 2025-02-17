using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyVideoResume.Abstractions.Core;
using MyVideoResume.Application.Job;
using MyVideoResume.Data.Models.Jobs;
using MyVideoResume.Extensions;
using System.Security.Claims;

namespace MyVideoResume.API.Controllers;

public class JobChromeRequest
{
    public string Token { get; set; }
    public string Html { get; set; }
    public string OriginUrl { get; set; }
}


[Route("[controller]")]
[ApiController]
public partial class ChromeController : ControllerBase
{
    private readonly IJobPromptEngine _engine;
    private readonly ILogger<ChromeController> _logger;
    private readonly JobService _service;

    public ChromeController(IJobPromptEngine engine, ILogger<ChromeController> logger, JobService jobService)
    {
        _logger = logger;
        _engine = engine;
        _service = jobService;
    }

    [Authorize]
    [HttpPost("createjobbestmatch")]
    public async Task<ActionResult<ResponseResult<JobItemEntity>>> CreateJobBestMatch([FromBody] JobChromeRequest jobChromeRequest)
    {
        var id = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var result = new ResponseResult<JobItemEntity>();
        //try
        //{
        //    result = await _service.SaveJobByUrl(url);
        //}
        //catch (Exception ex)
        //{
        //    _logger.LogError(ex.Message, ex);
        //    result.ErrorMessage = ex.Message;
        //}
        return Ok(result);
    }

    [HttpPost("CreateFromHtml")]
    public async Task<ActionResult<ResponseResult<JobItemEntity>>> CreateFromHtml([FromBody] string html)
    {
        var result = new ResponseResult<JobItemEntity>();
        try
        {
            var temppdfresult = await _engine.ExtractJob(html);
            if (!temppdfresult.ErrorMessage.HasValue())
            {
                //Remove the Markdown from the Response
                var job = temppdfresult.Result;
                result = await _service.CreateJob("automation", job); //TODO: This won't work; Fix
            }
            else
            {
                result.ErrorMessage = temppdfresult.ErrorMessage;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            result.ErrorMessage = ex.Message;
        }
        return result;
    }


    [Authorize]
    [HttpPost("CreateFromFile")]
    public async Task<ActionResult<ResponseResult<JobItemEntity>>> CreateFromFile(IFormFile file)
    {
        var result = new ResponseResult<JobItemEntity>();
        //try
        //{
        //    if (file != null)
        //    {
        //        //Lets Verify if the user is logged in.. If so, we'll create a resume.
        //        var id = User.FindFirstValue(ClaimTypes.NameIdentifier);

        //        var fileasstring = _documentProcessor.ConvertToString(file);
        //        var temppdfresult = await _engine.ExtractJob(fileasstring);
        //        if (!temppdfresult.ErrorMessage.HasValue())
        //        {
        //            //Remove the Markdown from the Response
        //            var job = temppdfresult.Result;
        //            result = await _service.CreateJob(id, job);
        //        }
        //        else
        //        {
        //            result.ErrorMessage = temppdfresult.ErrorMessage;
        //        }
        //    }
        //}
        //catch (Exception ex)
        //{
        //    _logger.LogError(ex.Message, ex);
        //    result.ErrorMessage = ex.Message;
        //}
        return result;
    }
}
