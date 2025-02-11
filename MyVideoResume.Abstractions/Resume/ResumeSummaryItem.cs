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

public class ResumeSummaryItem : IResumeItem
{
    public string Id { get; set; }

    public string UserId { get; set; }
    public string ResumeName { get; set; }
    public string ResumeSlug { get; set; }
    public string ResumeTemplateName { get; set; }
    public string ResumeSummary { get; set; }
    public float? SentimentScore { get; set; }
    public bool IsPublic { get; set; }

    public string CreationDateTimeFormatted { get; set; }
}
