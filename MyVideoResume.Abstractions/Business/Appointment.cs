using MyVideoResume.Abstractions.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
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

public interface IActionItem {
    DateTime Start { get; set; }
    DateTime End { get; set; }
    string Text { get; set; }
    ActionItemStatus Status { get; set; }
}

public class Appointment: ToDo, IActionItem
{
}