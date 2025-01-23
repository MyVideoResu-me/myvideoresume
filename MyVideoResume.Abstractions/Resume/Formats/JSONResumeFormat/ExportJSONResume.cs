using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MyVideoResume.Abstractions.Resume.Formats.JSONResumeFormat;

public class ExportJSONResume
{
    public ExportBasics Basics { get; set; }
    public List<ExportWork> Work { get; set; }
    public List<ExportVolunteer> Volunteer { get; set; }
    public List<ExportEducation> Education { get; set; }
    public List<ExportAward> Awards { get; set; }
    public List<ExportCertificate> Certificates { get; set; }
    public List<ExportPublication> Publications { get; set; }
    public List<ExportSkill> Skills { get; set; }
    public List<ExportLanguageItem> Languages { get; set; }
    public List<ExportInterest> Interests { get; set; }
    public List<ExportReferenceItem> References { get; set; }
    public List<ExportProject> Projects { get; set; }
}

public class ExportBasics
{
    public string Name { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public string Image { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? Url { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public ExportLocation? Location { get; set; }
}

public class ExportLocation
{
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Region { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
    public string CountryCode { get; set; } = string.Empty;
}

public class ExportWork
{
    public string Name { get; set; } = string.Empty;
    public string Position { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public string StartDate { get; set; } = string.Empty;
    public string EndDate { get; set; } = string.Empty;
    public List<string>? Highlights { get; set; }
    [JsonIgnore, NotMapped]
    public string HighlightsFlattened
    {
        get => String.Join(Environment.NewLine, Highlights);
        set
        {
            Highlights.Clear();
            string[] val;
            if (value.Contains("\n"))
                val = value.Split("\n");
            else
                val = value.Split(Environment.NewLine);
            Highlights.AddRange(val);
        }
    }
    public string? Url { get; set; } = string.Empty;
}

public class ExportVolunteer
{
    public string Organization { get; set; } = string.Empty;
    public string Position { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public string StartDate { get; set; } = string.Empty;
    public string EndDate { get; set; } = string.Empty;
    public List<string>? Highlights { get; set; }
    public string? Url { get; set; } = string.Empty;
}

public class ExportEducation
{
    public string Institution { get; set; } = string.Empty;
    public string? Area { get; set; } = string.Empty;
    public string? StudyType { get; set; } = string.Empty;
    public string StartDate { get; set; } = string.Empty;
    public string EndDate { get; set; } = string.Empty;
    public string? Score { get; set; } = string.Empty;
    public List<string>? Courses { get; set; } 
    public string? Url { get; set; } = string.Empty;
}

public class ExportAward
{
    public string Title { get; set; } = string.Empty;
    public string Date { get; set; } = string.Empty;
    public string Awarder { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
}

public class ExportCertificate
{
    public string Name { get; set; } = string.Empty;
    public string Date { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string? Url { get; set; } = string.Empty;
}

public class ExportPublication
{
    public string Name { get; set; } = string.Empty;
    public string Publisher { get; set; } = string.Empty;
    public string ReleaseDate { get; set; } = string.Empty;
    public string? Url { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
}

public class ExportSkill
{
    public string Name { get; set; } = string.Empty;
    public string Level { get; set; } = string.Empty;
    public List<string>? Keywords { get; set; }
}

public class ExportLanguageItem
{
    public string Language { get; set; } = string.Empty;
    public string Fluency { get; set; } = string.Empty;
}

public class ExportInterest
{
    public string Name { get; set; } = string.Empty;
    public List<string>? Keywords { get; set; }
}

public class ExportReferenceItem
{
    public string Name { get; set; } = string.Empty;
    public string Reference { get; set; } = string.Empty;
}

public class ExportProject
{
    public string Name { get; set; } = string.Empty;
    public string StartDate { get; set; } = string.Empty;
    public string EndDate { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<string>? Highlights { get; set; }
    public string? Url { get; set; } = string.Empty;
}