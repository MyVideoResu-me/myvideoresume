using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MyVideoResume.Abstractions.Resume.Formats.JSONResumeFormat;

public static class ResumeExtensions
{
    public static string ToMarkdown(this JSONResume resume)
    {
        if (resume == null)
            return string.Empty;

        var sb = new StringBuilder();

        // Header
        sb.AppendLine($"# {resume.Basics.Name}");
        sb.AppendLine($"**{resume.Basics.Label}**");
        //sb.AppendLine($"![Image]({resume.Basics.Image})");
        sb.AppendLine();
        sb.AppendLine($"Email: {resume.Basics.Email}");
        sb.AppendLine($"Phone: {resume.Basics.Phone}");
        if (!string.IsNullOrEmpty(resume.Basics.Url))
            sb.AppendLine($"[Website]({resume.Basics.Url})");
        sb.AppendLine();
        sb.AppendLine("## Summary");
        sb.AppendLine(resume.Basics.Summary);
        sb.AppendLine();

        // Location
        if (resume.Basics.Location != null && !string.IsNullOrEmpty(resume.Basics.Location.Address))
        {
            sb.AppendLine("## Location");
            sb.AppendLine($"{resume.Basics.Location.Address}, {resume.Basics.Location.City}, {resume.Basics.Location.Region}, {resume.Basics.Location.PostalCode}, {resume.Basics.Location.CountryCode}");
            sb.AppendLine();
        }

        // Work Experience
        if (resume.Work != null && resume.Work.Any())
        {
            sb.AppendLine("## Work Experience");
            foreach (var work in resume.Work)
            {
                if (!string.IsNullOrEmpty(work.Name))
                {
                    sb.AppendLine($"### {work.Position} at {work.Name}");
                    sb.AppendLine($"{work.StartDate} - {work.EndDate}");
                    sb.AppendLine(work.Summary);
                    if (work.Highlights != null && work.Highlights.Any())
                    {
                        sb.AppendLine("#### Highlights");
                        foreach (var highlight in work.Highlights)
                        {
                            sb.AppendLine($"- {highlight}");
                        }
                    }
                    sb.AppendLine();
                }
            }
        }

        // Volunteer Experience
        if (resume.Volunteer != null && resume.Volunteer.Any())
        {
            sb.AppendLine("## Volunteer Experience");
            foreach (var volunteer in resume.Volunteer)
            {
                sb.AppendLine($"### {volunteer.Position} at {volunteer.Organization}");
                sb.AppendLine($"{volunteer.StartDate} - {volunteer.EndDate}");
                sb.AppendLine(volunteer.Summary);
                if (volunteer.Highlights != null && volunteer.Highlights.Any())
                {
                    sb.AppendLine("#### Highlights");
                    foreach (var highlight in volunteer.Highlights)
                    {
                        sb.AppendLine($"- {highlight}");
                    }
                }
                sb.AppendLine();
            }
        }

        // Education
        if (resume.Education != null && resume.Education.Any())
        {
            sb.AppendLine("## Education");
            foreach (var education in resume.Education)
            {
                sb.AppendLine($"### {education.Institution}");
                sb.AppendLine($"{education.StartDate} - {education.EndDate}");
                if (!string.IsNullOrEmpty(education.StudyType))
                    sb.AppendLine($"{education.StudyType} in {education.Area}");
                if (!string.IsNullOrEmpty(education.Score))
                    sb.AppendLine($"Score: {education.Score}");
                if (education.Courses != null && education.Courses.Any())
                {
                    sb.AppendLine("#### Courses");
                    foreach (var course in education.Courses)
                    {
                        sb.AppendLine($"- {course}");
                    }
                }
                sb.AppendLine();
            }
        }

        // Awards
        if (resume.Awards != null && resume.Awards.Any())
        {
            sb.AppendLine("## Awards");
            foreach (var award in resume.Awards)
            {
                sb.AppendLine($"### {award.Title}");
                sb.AppendLine($"{award.Date}");
                sb.AppendLine($"Awarded by: {award.Awarder}");
                sb.AppendLine(award.Summary);
                sb.AppendLine();
            }
        }

        // Certificates
        if (resume.Certificates != null && resume.Certificates.Any())
        {
            sb.AppendLine("## Certificates");
            foreach (var certificate in resume.Certificates)
            {
                sb.AppendLine($"### {certificate.Name}");
                sb.AppendLine($"{certificate.Date}");
                sb.AppendLine($"Issued by: {certificate.Issuer}");
                sb.AppendLine($"[Certificate]({certificate.Url})");
                sb.AppendLine();
            }
        }

        // Publications
        if (resume.Publications != null && resume.Publications.Any())
        {
            sb.AppendLine("## Publications");
            foreach (var publication in resume.Publications)
            {
                sb.AppendLine($"### {publication.Name}");
                sb.AppendLine($"{publication.ReleaseDate}");
                sb.AppendLine($"Published by: {publication.Publisher}");
                sb.AppendLine($"[Publication]({publication.Url})");
                sb.AppendLine(publication.Summary);
                sb.AppendLine();
            }
        }

        // Skills
        if (resume.Skills != null && resume.Skills.Any())
        {
            sb.AppendLine("## Skills");
            foreach (var skill in resume.Skills)
            {
                if (!string.IsNullOrEmpty(skill.Name))
                {
                    sb.AppendLine($"### {skill.Name}");
                    if (!string.IsNullOrEmpty(skill.Level))
                        sb.AppendLine($"Level: {skill.Level}");
                    if (skill.Keywords != null && skill.Keywords.Any())
                    {
                        sb.AppendLine("#### Keywords");
                        foreach (var keyword in skill.Keywords)
                        {
                            sb.AppendLine($"- {keyword}");
                        }
                    }
                    sb.AppendLine();
                }
            }
        }

        // Languages
        if (resume.Languages != null && resume.Languages.Any())
        {
            sb.AppendLine("## Languages");
            foreach (var language in resume.Languages)
            {
                sb.AppendLine($"### {language.Language}");
                sb.AppendLine($"Fluency: {language.Fluency}");
                sb.AppendLine();
            }
        }

        // Interests
        if (resume.Interests != null && resume.Interests.Any())
        {
            sb.AppendLine("## Interests");
            foreach (var interest in resume.Interests)
            {
                sb.AppendLine($"### {interest.Name}");
                if (interest.Keywords != null && interest.Keywords.Any())
                {
                    sb.AppendLine("#### Keywords");
                    foreach (var keyword in interest.Keywords)
                    {
                        sb.AppendLine($"- {keyword}");
                    }
                }
                sb.AppendLine();
            }
        }

        // References
        if (resume.References != null && resume.References.Any())
        {
            sb.AppendLine("## References");
            foreach (var reference in resume.References)
            {
                sb.AppendLine($"### {reference.Name}");
                sb.AppendLine(reference.Reference);
                sb.AppendLine();
            }
        }

        // Projects
        if (resume.Projects != null && resume.Projects.Any())
        {
            sb.AppendLine("## Projects");
            foreach (var project in resume.Projects)
            {
                sb.AppendLine($"### {project.Name}");
                sb.AppendLine($"{project.StartDate} - {project.EndDate}");
                sb.AppendLine(project.Description);
                if (project.Highlights != null && project.Highlights.Any())
                {
                    sb.AppendLine("#### Highlights");
                    foreach (var highlight in project.Highlights)
                    {
                        sb.AppendLine($"- {highlight}");
                    }
                }
                sb.AppendLine($"[Project]({project.Url})");
                sb.AppendLine();
            }
        }

        return sb.ToString();
    }
}

public class JSONResumeDTO : JSONResume
{

    public DateTime? CreationDateTime { get; set; }
    public DateTime? UpdateDateTime { get; set; }
    public DateTime? DeletedDateTime { get; set; }

}

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
        get => Highlights != null ? String.Join(Environment.NewLine, Highlights) : string.Empty;
        set
        {
            Highlights ??= new List<string>();
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
        get => Highlights != null ? String.Join(Environment.NewLine, Highlights) : string.Empty;
        set
        {
            Highlights ??= new List<string>();
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
        get => Highlights != null ? String.Join(Environment.NewLine, Highlights) : string.Empty;
        set
        {
            Highlights ??= new List<string>();
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
