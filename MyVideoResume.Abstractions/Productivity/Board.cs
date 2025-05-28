using MyVideoResume.Abstractions.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyVideoResume.Abstractions.Productivity;

public class BoardDTO : Board {

    public required string Id { get; set; }
}


public class Board: CommonBase
{
    //User who owns the Task
    public required string CreatedByUserId { get; set; }

    public bool IsDefault { get; set; }

    public required string Name { get; set; }

    public string? Description { get; set; }

}
