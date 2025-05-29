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
using System.Linq;

namespace MyVideoResume.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public partial class TaskController : ControllerBase
{
    private readonly ILogger<TaskController> _logger;
    private readonly ProductivityService _productivityService;
    private readonly JsonSerializerOptions _jsonOptions;

    public TaskController(ILogger<TaskController> logger, ProductivityService productivityService)
    {
        _logger = logger;
        _productivityService = productivityService;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Converters = { new System.Text.Json.Serialization.JsonStringEnumConverter() }
        };
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
    public async Task<ActionResult<ResponseResult<IProductivityItem>>> Save([FromBody] JsonElement taskDtoJson)
    {
        var result = new ResponseResult<IProductivityItem>();
        try
        {
            // Deserialize using our custom options
            var taskDto = JsonSerializer.Deserialize<TaskDTO>(taskDtoJson.GetRawText(), _jsonOptions);

            // Log the incoming task data
            _logger.LogInformation("Received task data: {@TaskData}", taskDto);

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                
                _logger.LogWarning("Model validation failed. Errors: {@ValidationErrors}", errors);
                
                var validationDetails = new
                {
                    Message = "Validation failed",
                    Errors = errors,
                    TaskData = new
                    {
                        Id = taskDto?.Id,
                        Text = taskDto?.Text,
                        Start = taskDto?.Start,
                        End = taskDto?.End,
                        TaskType = taskDto?.TaskType,
                        Status = taskDto?.Status,
                        CreatedByUserId = taskDto?.CreatedByUserId,
                        AssignedToUserId = taskDto?.AssignedToUserId
                    }
                };

                result.ErrorMessage = JsonSerializer.Serialize(validationDetails, _jsonOptions);
                return BadRequest(result);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (taskDto == null)
            {
                result.ErrorMessage = "Invalid task data";
                return BadRequest(result);
            }

            // Basic validation
            if (string.IsNullOrEmpty(taskDto.Text))
            {
                result.ErrorMessage = "Task text/title is required";
                return BadRequest(result);
            }

            if (taskDto.Start == default)
            {
                result.ErrorMessage = "Task start date is required";
                return BadRequest(result);
            }

            // For new tasks, ensure ID is not set
            if (string.IsNullOrEmpty(taskDto.Id))
            {
                taskDto.Id = null; // Ensure ID is null for new tasks
            }

            // Set the creator/assignee if not already set
            if (string.IsNullOrEmpty(taskDto.CreatedByUserId))
            {
                taskDto.CreatedByUserId = userId;
            }

            if (taskDto.AssignedToUserId.HasValue)
            {
                taskDto.AssignedToUserId = Guid.Parse(userId);
            }

            // Set creation time for new tasks
            if (taskDto.CreationDateTime == null)
            {
                taskDto.CreationDateTime = DateTime.UtcNow;
            }

            // Update time for existing tasks
            if (!string.IsNullOrEmpty(taskDto.Id))
            {
                taskDto.UpdateDateTime = DateTime.UtcNow;
            }

            result = await _productivityService.SaveTask(taskDto, userId);
            if (!string.IsNullOrEmpty(result.ErrorMessage))
            {
                _logger.LogWarning("Task save failed: {ErrorMessage}", result.ErrorMessage);
                return BadRequest(result);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving task");
            result.ErrorMessage = "Error saving task: " + ex.Message;
            return BadRequest(result);
        }
        return Ok(result);
    }
}

