using MyVideoResume.Abstractions.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyVideoResume.Abstractions.Business;

public class BoardDTO : Board {

    public string Id { get; set; }
}


public class Board: CommonBase
{
    //User who owns the Task
    public string CreatedByUserId { get; set; }

    public bool IsDefault { get; set; }

    public string Name { get; set; }

    public string? Description { get; set; }

}