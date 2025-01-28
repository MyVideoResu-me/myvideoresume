using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyVideoResume.Abstractions.Business;

public enum ActionItemStatus
{
    Planning,
    ToDo,
    Open,
    InProgress,
    Closed,
    Descoped,
}

public interface IActionItem
{
    string Text { get; set; }

    DateTime Start { get; set; }
    DateTime End { get; set; }

    string CreatedByUserId { get; set; }
    string AssignedToUserId { get; set; }

    ActionItemStatus Status { get; set; }
}