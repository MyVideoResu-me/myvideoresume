using MyVideoResume.Abstractions.Core;

namespace MyVideoResume.Abstractions.Productivity;

public class TaskDTO : TaskItem, IProductivityItem
{
    public required string Id { get; set; }

    public ContactPersonDTO? CreatedByUser { get; set; }
    public required ContactPersonDTO AssignedToUser { get; set; }

    public required string BoardId { get; set; }

}

public class TaskItem : CommonBase
{
    //User who owns the Task
    public string? CreatedByUserId { get; set; }

    public required string AssignedToUserId { get; set; }

    public required string Text { get; set; }

    public string? Description { get; set; }

    public DateTime Start { get; set; }

    public DateTime? End { get; set; }

    public TaskType TaskType { get; set; }
    public TaskType? SubTaskType { get; set; }
    public Actions? ActionToTake { get; set; }

    public ProductivityItemStatus Status { get; set; }
}
