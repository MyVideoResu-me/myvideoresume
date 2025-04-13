using MyVideoResume.Abstractions.Core;

namespace MyVideoResume.Abstractions.Productivity;

public class TaskDTO : TaskItem, IProductivityItem
{
    public string Id { get; set; }

    public ContactPersonDTO? CreatedByUser { get; set; }
    public ContactPersonDTO AssignedToUser { get; set; }

    public string BoardId { get; set; }

}

public class TaskItem : CommonBase, IProductivityItem
{
    //User who owns the Task
    public string? CreatedByUserId { get; set; }

    public string AssignedToUserId { get; set; }

    public string Text { get; set; }

    public string? Description { get; set; }

    public DateTime Start { get; set; }

    public DateTime? End { get; set; }

    public TaskType TaskType { get; set; }
    public TaskType? SubTaskType { get; set; }
    public Actions? ActionToTake { get; set; }

    public ProductivityItemStatus Status { get; set; }
}