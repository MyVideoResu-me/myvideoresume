using MyVideoResume.Abstractions.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyVideoResume.Abstractions.Business;

public class ToDo: CommonBase, IActionItem
{
    //User who owns the Task
    public string UserId { get; set; }

    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    public string Text { get; set; }
    public ActionItemStatus Status { get; set; }
}