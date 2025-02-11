using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using MyVideoResume.Abstractions.Core;
using MyVideoResume.Abstractions.Job;
using MyVideoResume.Abstractions.MetaContent;
using MyVideoResume.Abstractions.Resume.Formats.JSONResumeFormat;
using System.ComponentModel;
using System.Linq;
using System.Text.Json.Serialization;

namespace MyVideoResume.Abstractions.Resume;
public enum ResumeType
{
    ResumeBuilder = 0,
    JSONResumeFormat = 1,
    WordDoc = 2,
    Pdf = 3,
}

public class ResumeInformationDTO : ResumeInformation, IResumeItem
{
    public string Id { get; set; }
    public JSONResumeDTO MetaResume { get; set; }
    public List<MetaDataDTO> MetaData { get; set; }

    public ResumeTemplate? ResumeTemplate { get; set; }

    [JsonIgnore]
    public Dictionary<string, MetaDataDTO> MetaDataLookup
    {
        get
        {
            if (MetaData != null)
                return MetaData.ToDictionary(K => K.ReferenceId, Y => Y);
            else
                return new Dictionary<string, MetaDataDTO>();
        }
    }

}


public class ResumeInformation : GISData
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