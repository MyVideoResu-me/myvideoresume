using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyVideoResume.Abstractions.Core;
using MyVideoResume.Abstractions.Job;
using MyVideoResume.Abstractions.Productivity;
using MyVideoResume.Application.Job;
using MyVideoResume.Application.Productivity;
using MyVideoResume.Application.Resume;
using MyVideoResume.Data.Models;
using MyVideoResume.Data.Models.Jobs;
using MyVideoResume.Data.Models.Productivity;
using MyVideoResume.Data.Models.Resume;
using MyVideoResume.Documents;
using MyVideoResume.Web.Common;
using System.Security.Claims;
using System.Text.Json;

namespace MyVideoResume.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public partial class TaskController : ControllerBase
{
    private readonly ILogger<TaskController> _logger;
    private readonly ProductivityService _productivityService;

    public TaskController(ILogger<TaskController> logger, ProductivityService productivityService)
    {
        _logger = logger;
        _productivityService = productivityService;
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult<TaskDTO>> Get(string id)
    {
        var result = new TaskDTO();
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            result = await _productivityService.GetTaskById(id, userId);
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
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            result = await _productivityService.GetTasks(userId);
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
            result = await _productivityService.DeleteTask(id, userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            result.ErrorMessage = ex.Message;
        }
        return result;
    }

    [Authorize]
    [HttpPost("save")]
    public async Task<ActionResult<ResponseResult<IProductivityItem>>> Save([FromBody] TaskDTO taskDto)
    {
        var result = new ResponseResult<IProductivityItem>();
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (taskDto == null)
            {
                result.ErrorMessage = "Invalid task data";
                return result;
            }

            // Set the creator/assignee if not already set
            if (string.IsNullOrEmpty(taskDto.CreatedByUserId))
            {
                taskDto.CreatedByUserId = userId;
            }

            if (taskDto.AssignedToUserId == Guid.Empty && Guid.TryParse(userId, out var assignedToUserId))
            {
                taskDto.AssignedToUserId = assignedToUserId;
            }

            result = await _productivityService.SaveTask(taskDto, userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            result.ErrorMessage = "Error saving task: " + ex.Message;
        }
        return result;
    }
}

