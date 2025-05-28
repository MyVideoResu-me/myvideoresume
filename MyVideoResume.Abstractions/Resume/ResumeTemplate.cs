using MyVideoResume.Abstractions.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyVideoResume.Abstractions.Resume;

public class ResumeTemplate : CommonBase
{
    public string? UserId { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public string? TransformerComponentName { get; set; }
    public string? Namespace { get; set; }
}
