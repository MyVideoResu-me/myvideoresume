using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using MyVideoResume.Abstractions.Core;
using MyVideoResume.Abstractions.Job;
using System.ComponentModel;
using System.Linq;

namespace MyVideoResume.Abstractions.Resume;

public class ResumeInformationBase : GISData
{
    public required string UserId { get; set; }

    public string? Name { get; set; }

    public string? Slug { get; set; }

    public string? Description { get; set; }

    public bool? IsPrimaryDefault { get; set; } // Can only have 1 default or Primary

    public float? SentimentScore { get; set; }

    public bool? IsWatched { get; set; }

}

public class ResumeInformation : ResumeInformationBase
{
    public DisplayPrivacy Privacy_ShowResume { get; set; } = DisplayPrivacy.ToPublic;

    public DisplayPrivacy Privacy_ShowContactDetails { get; set; } = DisplayPrivacy.ToConnections;

    public Industry Industry { get; set; } = Industry.Management;
    public List<JobType> EmploymentType { get; set; } = new List<JobType>();
    public PaySchedule PaySchedule { get; set; } = PaySchedule.Yearly;
    public float MinimumSalary { get; set; }
    public ResumeType ResumeType { get; set; }

    public required string ResumeSerialized { get; set; }
}
