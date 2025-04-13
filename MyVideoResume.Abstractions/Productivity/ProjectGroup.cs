using MyVideoResume.Abstractions.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace MyVideoResume.Abstractions.Productivity;

public class ProjectGroup: TaskItem, IProductivityItem
{
}