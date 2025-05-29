using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyVideoResume.Abstractions.Core;
using MyVideoResume.Abstractions.Productivity;
using MyVideoResume.Application.Account;
using MyVideoResume.Data;
using MyVideoResume.Data.Models.Account.Profiles;
using MyVideoResume.Data.Models.Productivity;
using MyVideoResume.Web.Common;
using System;

namespace MyVideoResume.Application.Productivity;

public class ProductivityService
{
    private readonly DataContext _dataContext;
    private readonly IMapper _mapper;
    private readonly ILogger<ProductivityService> _logger;

    public ProductivityService(DataContext dataContextService, ILogger<ProductivityService> logger, IMapper mapper)
    {
        this._dataContext = dataContextService;
        this._logger = logger;
        this._mapper = mapper;
    }

    public async Task<List<TaskEntity>> CreateOnboardingTasks(string userId, UserProfileEntity userProfile, CompanyProfileEntity companyProfile)
    {
        var tasks = new List<TaskEntity>();
        try
        {
            //Lets confirm that the onboarding tasks don't exist
            var onboardingTasks = _dataContext.Tasks.FirstOrDefault(x => x.TaskType == TaskType.Onboarding && x.AssignedToUserId.ToString() == userId);
            if (onboardingTasks == null)
            {
                //Check if there is a Default Board
                var board = _dataContext.Boards.FirstOrDefault(x => x.CreatedByUserId == userId && x.IsDefault == true);
                if (board == null)
                {
                    board = new BoardEntity() { CreatedByUser = userProfile, Tasks = new List<TaskEntity>(), CreatedByUserId = userId, CompanyProfile = companyProfile, Name = "My Board", IsDefault = true, CreationDateTime = DateTime.UtcNow };
                }

                //What are the tasks that they should complete to be 100% profile
                TaskEntity taskProfile = new TaskEntity()
                {
                    AssignedToUser = userProfile,
                    AssignedToUserId = Guid.Parse(userId),
                    CreationDateTime = DateTime.UtcNow,
                    Start = DateTime.UtcNow,
                    Status = ProductivityItemStatus.ToDo,
                    CompanyProfile = companyProfile,
                    TaskType = TaskType.Onboarding,
                    SubTaskType = TaskType.OnboardingProfile,
                    Text = "Update Your Profile",
                    Description = "Please update your profile."
                };
                tasks.Add(taskProfile);

                TaskEntity taskProfileSettings = new TaskEntity()
                {
                    AssignedToUser = userProfile,
                    AssignedToUserId = Guid.Parse(userId),
                    CreationDateTime = DateTime.UtcNow,
                    Start = DateTime.UtcNow,
                    Status = ProductivityItemStatus.ToDo,
                    CompanyProfile = companyProfile,
                    TaskType = TaskType.Onboarding,
                    SubTaskType = TaskType.OnboardingProfileSettings,
                    Text = "Update Your Profile Settings",
                    Description = "Please update your profile settings."
                };
                tasks.Add(taskProfileSettings);

                board.Tasks.AddRange(tasks);
                await _dataContext.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
        }

        return tasks;
    }

    public async Task<List<TaskEntity>> UpdateTasksByRole(string userId)
    {
        return null;
    }

    /// <summary>
    /// Gets all tasks for a user
    /// </summary>
    /// <param name="userId">The user ID</param>
    /// <returns>List of task DTOs</returns>
    public async Task<List<TaskDTO>> GetTasks(string userId)
    {
        var tasks = new List<TaskDTO>();
        try
        {
            var taskEntities = await _dataContext.Tasks
                .Where(t => t.AssignedToUserId.ToString() == userId || t.CreatedByUserId.ToString() == userId)
                .OrderByDescending(t => t.CreationDateTime)
                .ToListAsync();

            tasks = _mapper.Map<List<TaskDTO>>(taskEntities);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
        }
        return tasks;
    }

    /// <summary>
    /// Gets a specific task by ID
    /// </summary>
    /// <param name="taskId">The task ID</param>
    /// <param name="userId">The user ID</param>
    /// <returns>Task DTO</returns>
    public async Task<TaskDTO> GetTaskById(string taskId, string userId)
    {
        TaskDTO task = new TaskDTO();
        try
        {
            if (!Guid.TryParse(taskId, out var id))
            {
                return task;
            }

            var taskEntity = await _dataContext.Tasks
                .FirstOrDefaultAsync(t => t.Id == id && 
                    (t.AssignedToUserId.ToString() == userId || t.CreatedByUserId.ToString() == userId));

            if (taskEntity != null)
            {
                task = _mapper.Map<TaskDTO>(taskEntity);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
        }
        return task;
    }

    /// <summary>
    /// Saves a task (creates or updates)
    /// </summary>
    /// <param name="taskDto">The task to save</param>
    /// <param name="userId">The user ID</param>
    /// <returns>Response with the saved task</returns>
    public async Task<ResponseResult<IProductivityItem>> SaveTask(TaskDTO taskDto, string userId)
    {
        var response = new ResponseResult<IProductivityItem>();
        try
        {
            // Check if this is an update or create
            TaskEntity taskEntity;
            bool isNewTask = string.IsNullOrEmpty(taskDto.Id) || taskDto.Id == Guid.Empty.ToString();
            
            if (isNewTask)
            {
                // This is a new task
                taskEntity = _mapper.Map<TaskEntity>(taskDto);
                taskEntity.CreationDateTime = DateTime.UtcNow;
                
                // Set default values if not provided
                if (taskEntity.CreatedByUserId == null || taskEntity.CreatedByUserId == Guid.Empty)
                {
                    taskEntity.CreatedByUserId = Guid.TryParse(userId, out var guid) ? guid : (Guid?)null;
                }
                
                // Find the default board to add the task to
                var defaultBoard = await _dataContext.Boards
                    .FirstOrDefaultAsync(b => b.CreatedByUserId == userId && b.IsDefault);
                
                if (defaultBoard != null)
                {
                    // Set BoardId on the task DTO for reference
                    taskDto.BoardId = defaultBoard.Id.ToString();
                }
                
                _dataContext.Tasks.Add(taskEntity);
            }
            else
            {
                // This is an update to an existing task
                if (!Guid.TryParse(taskDto.Id, out var taskId))
                {
                    response.ErrorMessage = "Invalid task ID format";
                    return response;
                }
                
                taskEntity = await _dataContext.Tasks.FindAsync(taskId);
            if (taskEntity == null)
            {
                response.ErrorMessage = "Task not found";
                return response;
            }
            
                // Check if the user has permission to update this task
            if (taskEntity.CreatedByUserId?.ToString() != userId && taskEntity.AssignedToUserId?.ToString() != userId)
            {
                    response.ErrorMessage = "You don't have permission to update this task";
                return response;
            }
            
                // Update the task properties
                _mapper.Map(taskDto, taskEntity);

                // Ensure update doesn't change the creator
                if (taskEntity.CreatedByUserId?.ToString() != userId && !string.IsNullOrEmpty(taskDto.CreatedByUserId))
        {
                    taskEntity.CreatedByUserId = Guid.TryParse(userId, out var guid) ? guid : (Guid?)null;
        }
        
                _dataContext.Tasks.Update(taskEntity);
    }

            await _dataContext.SaveChangesAsync();

            // Map back to DTO for the response
            response.Result = _mapper.Map<TaskDTO>(taskEntity);
}
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            response.ErrorMessage = "Error saving task: " + ex.Message;
        }

        return response;
    }

    /// <summary>
    /// Deletes a task
    /// </summary>
    /// <param name="taskId">The task ID</param>
    /// <param name="userId">The user ID</param>
    /// <returns>Response result</returns>
    public async Task<ResponseResult> DeleteTask(string taskId, string userId)
    {
        var response = new ResponseResult();
        try
        {
            if (!Guid.TryParse(taskId, out var id))
            {
                response.ErrorMessage = "Invalid task ID";
                return response;
            }

            var taskEntity = await _dataContext.Tasks.FindAsync(id);

            if (taskEntity == null)
            {
                response.ErrorMessage = "Task not found";
                return response;
            }

            // Check if the user has permission to delete this task
            if (taskEntity.CreatedByUserId?.ToString() != userId && taskEntity.AssignedToUserId.ToString() != userId)
            {
                response.ErrorMessage = "You don't have permission to delete this task";
                return response;
            }

            _dataContext.Tasks.Remove(taskEntity);
            await _dataContext.SaveChangesAsync();

            response.Result = "Task deleted successfully";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            response.ErrorMessage = "Error deleting task: " + ex.Message;
        }

        return response;
    }
}
