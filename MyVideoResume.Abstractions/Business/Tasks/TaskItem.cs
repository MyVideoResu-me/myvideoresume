using MyVideoResume.Abstractions.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyVideoResume.Abstractions.Business.Tasks;

public class TaskDTO : TaskItem
{
    public string Id { get; set; }

    public ContactPersonDTO? CreatedByUser { get; set; }
    public ContactPersonDTO AssignedToUser { get; set; }

    public string BoardId { get; set; }

}

public class TaskItem : CommonBase
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

    public ActionItemStatus Status { get; set; }
}