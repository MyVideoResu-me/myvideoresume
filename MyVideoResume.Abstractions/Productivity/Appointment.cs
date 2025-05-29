using MyVideoResume.Abstractions.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyVideoResume.Abstractions.Productivity;

public class AppointmentDTO : Appointment, IProductivityItem
{    public string Id { get; set; }
}

public class Appointment : TaskItem
{
}
