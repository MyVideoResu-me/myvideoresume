using MyVideoResume.Abstractions.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyVideoResume.Abstractions.Productivity;

public interface IProductivityItem
{
    public string Id { get; set; }

    string Text { get; set; }
    string? Description { get; set; }

    DateTime Start { get; set; }
    DateTime? End { get; set; }

    string? CreatedByUserId { get; set; }
    Guid? AssignedToUserId { get; set; }

    ProductivityItemStatus Status { get; set; }
}