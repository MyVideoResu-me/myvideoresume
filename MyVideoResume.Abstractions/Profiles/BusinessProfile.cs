using MyVideoResume.Abstractions.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyVideoResume.Abstractions.Profiles;

public class BusinessProfile : ProfileBase
{
    public string? Website { get; set; }

    public DateTime? ApprovedDateTime { get; set; }

    //public double? PaidPurchasePrice { get; set; }
    //public ProcessingStatus Status { get; set; }
    //public DateTime? PaidPurchaseDateTime { get; set; }
}