using MyVideoResume.Abstractions.Resume.Formats.JSONResumeFormat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyVideoResume.Abstractions.Resume;

public interface IResumeItem {
    string Id { get; set; }
    string UserId { get; set; }
    float? SentimentScore { get; set; }
}


