using MyVideoResume.Abstractions.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyVideoResume.Abstractions.Job;

public class JobItemDTO : JobBase
{
    public string Id { get; set; }
    public string CreationDateTimeFormatted { get; set; }
    public ContactPersonDTO CreatedByUser { get; set; }
}
