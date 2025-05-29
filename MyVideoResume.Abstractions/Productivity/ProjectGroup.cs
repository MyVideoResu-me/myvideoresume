using MyVideoResume.Abstractions.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace MyVideoResume.Abstractions.Productivity;

public class ProjectGroupDTO : ProjectGroup, IProductivityItem
{
    public string Id { get; set; }
}

public class ProjectGroup: TaskItem
{
}
