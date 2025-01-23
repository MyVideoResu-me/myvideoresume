using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MyVideoResume.Abstractions.Resume.Formats.JSONResumeFormat;

public class JSONResume
{
    public string? Id { get; set; }

    [JsonIgnore]
    public string UserId { get; set; }

    public Basics Basics { get; set; }
    public List<Work> Work { get; set; }
    public List<Volunteer> Volunteer { get; set; }
    public List<Education> Education { get; set; }
    public List<Award> Awards { get; set; }
    public List<Certificate> Certificates { get; set; }
    public List<Publication> Publications { get; set; }
    public List<Skill> Skills { get; set; }
    public List<LanguageItem> Languages { get; set; }
    public List<Interest> Interests { get; set; }
    public List<ReferenceItem> References { get; set; }
    public List<Project> Projects { get; set; }
}

public class Basics
{
    public string? Id { get; set; } 

    public string Name { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public string Image { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? Url { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public Location? Location { get; set; }
}

public class Location
{
    public string? Id { get; set; }
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Region { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
    public string CountryCode { get; set; } = string.Empty;
}

public class Work
{
    public string? Id { get; set; } 
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

public class Volunteer
{
    public string? Id { get; set; }
    public string Organization { get; set; } = string.Empty;
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

public class Education
{
    public string? Id { get; set; } 
    public string Institution { get; set; } = string.Empty;
    public string? Area { get; set; } = string.Empty;
    public string? StudyType { get; set; } = string.Empty;
    public string StartDate { get; set; } = string.Empty;
    public string EndDate { get; set; } = string.Empty;
    public string? Score { get; set; } = string.Empty;
    public List<string>? Courses { get; set; }
    [JsonIgnore, NotMapped]
    public string CoursesFlattened
    {
        get => String.Join(Environment.NewLine, Courses);
        set
        {
            Courses.Clear();
            string[] val;
            if (value.Contains("\n"))
                val = value.Split("\n");
            else
                val = value.Split(Environment.NewLine);
            Courses.AddRange(val);
        }
    }
    public string? Url { get; set; } = string.Empty;
}

public class Award
{
    public string? Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Date { get; set; } = string.Empty;
    public string Awarder { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
}

public class Certificate
{
    public string? Id { get; set; } 
    public string Name { get; set; } = string.Empty;
    public string Date { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string? Url { get; set; } = string.Empty;
}

public class Publication
{
    public string? Id { get; set; } 
    public string Name { get; set; } = string.Empty;
    public string Publisher { get; set; } = string.Empty;
    public string ReleaseDate { get; set; } = string.Empty;
    public string? Url { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
}

public class Skill
{
    public string? Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Level { get; set; } = string.Empty;
    public List<string>? Keywords { get; set; }
    [JsonIgnore, NotMapped]
    public string KeywordsFlattened
    {
        get => String.Join(Environment.NewLine, Keywords);
        set
        {
            Keywords.Clear();
            string[] val;
            if (value.Contains("\n"))
                val = value.Split("\n");
            else
                val = value.Split(Environment.NewLine);
            Keywords.AddRange(val);
        }
    }
}

public class LanguageItem
{
    public string? Id { get; set; } 
    public string Language { get; set; } = string.Empty;
    public string Fluency { get; set; } = string.Empty;
}

public class Interest
{
    public string? Id { get; set; } 
    public string Name { get; set; } = string.Empty;
    public List<string>? Keywords { get; set; }
    [JsonIgnore, NotMapped]
    public string KeywordsFlattened
    {
        get => String.Join(Environment.NewLine, Keywords);
        set
        {
            Keywords.Clear();
            string[] val;
            if (value.Contains("\n"))
                val = value.Split("\n");
            else
                val = value.Split(Environment.NewLine);
            Keywords.AddRange(val);
        }
    }
}

public class ReferenceItem
{
    public string? Id { get; set; } 
    public string Name { get; set; } = string.Empty;
    public string Reference { get; set; } = string.Empty;
}

public class Project
{
    public string? Id { get; set; } 
    public string Name { get; set; } = string.Empty;
    public string StartDate { get; set; } = string.Empty;
    public string EndDate { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
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