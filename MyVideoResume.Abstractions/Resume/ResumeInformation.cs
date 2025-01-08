using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using MyVideoResume.Abstractions.Core;
using MyVideoResume.Abstractions.Job;
using System.ComponentModel;
using System.Linq;

namespace MyVideoResume.Abstractions.Resume;
public enum ResumeType
{
    ResumeBuilder = 0,
    JSONResumeFormat = 1,
    WordDoc = 2,
    Pdf = 3,
}

public enum DisplayPrivacy
{
    [Description("Visible to Public")]
    ToPublic = 0,
    [Description("Visible to Recruiters")]
    ToRecruiters = 1,
    [Description("Visible to Connections")]
    ToConnections = 2,
    [Description("Visible to Self (Private)")]
    ToSelf = 10
}

public class ResumeInformation : CommonBase
{
    public string UserId { get; set; }

    public string? Name { get; set; }

    public string? Slug { get; set; }

    public string? Description { get; set; }

    public DisplayPrivacy Privacy_ShowResume { get; set; } = DisplayPrivacy.ToPublic;

    public DisplayPrivacy Privacy_ShowContactDetails { get; set; } = DisplayPrivacy.ToConnections;

    public Industry Industry { get; set; } = Industry.Management;
    public List<JobType> EmploymentType { get; set; } = new List<JobType>();
    public PaySchedule PaySchedule { get; set; } = PaySchedule.Yearly;
    public float MinimumSalary { get; set; }
    public ResumeType ResumeType { get; set; }
    public float? SentimentScore { get; set; }
    public string ResumeSerialized { get; set; }
}