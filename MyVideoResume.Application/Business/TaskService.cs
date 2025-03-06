using AutoMapper;
using Microsoft.Extensions.Logging;
using MyVideoResume.Abstractions.Business;
using MyVideoResume.Abstractions.Business.Tasks;
using MyVideoResume.Abstractions.Core;
using MyVideoResume.Application.Account;
using MyVideoResume.Data;
using MyVideoResume.Data.Models.Account.Profiles;
using MyVideoResume.Data.Models.Business.Tasks;

namespace MyVideoResume.Application.Business;

public class TaskService
{
    private readonly DataContext _dataContext;
    private readonly IMapper _mapper;
    private readonly ILogger<AccountService> _logger;

    public TaskService(DataContext dataContextService, ILogger<AccountService> logger, IMapper mapper)
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
                    Status = ActionItemStatus.ToDo,
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
                    Status = ActionItemStatus.ToDo,
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



}
