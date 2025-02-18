using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyVideoResume.Abstractions.Job;

public class JobChromeRequest
{
    public string Token { get; set; }
    public string Html { get; set; }
    public string OriginUrl { get; set; }
}
