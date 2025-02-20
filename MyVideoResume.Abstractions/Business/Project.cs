using MyVideoResume.Abstractions.Business.Tasks;
using MyVideoResume.Abstractions.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace MyVideoResume.Abstractions.Business;

public class Project: TaskItem, IActionItem
{
}