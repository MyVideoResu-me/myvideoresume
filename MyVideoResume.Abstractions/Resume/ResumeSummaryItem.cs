using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyVideoResume.Abstractions.Resume;

public class ResumeSummaryItem
{
    public string Id { get; set; }

    public string UserId { get; set; }
    public string ResumeName { get; set; }
    public string ResumeSlug { get; set; }
    public string ResumeTemplateName { get; set; }
    public string ResumeSummary { get; set; }

    public string ResumeSerialized { get; set; }

    public float? SentimentScore { get; set; }
    public bool IsPublic { get; set; }

    public string CreationDateTimeFormatted { get; set; }
}
