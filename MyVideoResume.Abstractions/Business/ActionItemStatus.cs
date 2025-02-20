using MyVideoResume.Abstractions.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyVideoResume.Abstractions.Business;

public interface IActionItem
{
    string Text { get; set; }
    string? Description { get; set; }

    DateTime Start { get; set; }
    DateTime? End { get; set; }

    string? CreatedByUserId { get; set; }
    string AssignedToUserId { get; set; }

    ActionItemStatus Status { get; set; }
}