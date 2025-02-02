using MyVideoResume.Abstractions.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyVideoResume.Abstractions.Job;

public class Fee
{
    public string UserId { get; set; }
    public string Description { get; set; }
    public float Amount { get; set; }
    public BonusType BonusType { get; set; }
    public BonusFrequency BonusFrequency { get; set; }
}
