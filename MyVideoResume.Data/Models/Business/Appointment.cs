using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyVideoResume.Data.Models.Business;

public class Appointment
{
    public DateTime Start { get; set; }
    public DateTime End { get; set; }

    public string Text { get; set; }
}
