using MyVideoResume.Abstractions.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyVideoResume.Abstractions.Business;

public interface IAppointment {
    DateTime Start { get; set; }
    DateTime End { get; set; }
    string Text { get; set; }

}

public class Appointment: CommonBase, IAppointment
{
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    public string Text { get; set; }
}
