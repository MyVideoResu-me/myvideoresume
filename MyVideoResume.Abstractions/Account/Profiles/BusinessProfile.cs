using MyVideoResume.Abstractions.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyVideoResume.Abstractions.Account.Profiles;

public class BusinessProfile : ProfileBase
{
    public string Name { get; set; }
    public string? Description { get; set; }
    public string? Website { get; set; }

    public DateTime? ApprovedDateTime { get; set; }
}