using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyVideoResume.Abstractions.Job;

public class JobSummaryItem : JobBase
{
    public string Id { get; set; }
    public string CreationDateTimeFormatted { get; set; }
}
