﻿using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyVideoResume.Abstractions.Core;
using MyVideoResume.Abstractions.Job;
using MyVideoResume.Abstractions.Productivity;
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
public partial class TaskController : ControllerBase
{
    private readonly ILogger<TaskController> _logger;

    public TaskController(ILogger<TaskController> logger)
    {
        _logger = logger;
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult<TaskDTO>> Get(string id)
    {
        var result = new TaskDTO();
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            //result = await _service.GetJob(id, userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
        }
        return result;
    }

    [HttpGet]
    [Authorize]
    public async Task<ActionResult<List<TaskDTO>>> Get()
    {
        var result = new List<TaskDTO>();
        try
        {
            //result = await _service.GetJobSummaryItems(onlyPublic: true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
        }
        return result;
    }

    [Authorize]
    [HttpPost("{id}")]
    public async Task<ActionResult<ResponseResult>> Delete(string id)
    {
        var result = new ResponseResult();
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            //result = await _service.DeleteJob(id, jobId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            result.Result = ex.Message;
        }
        return result;
    }
}
