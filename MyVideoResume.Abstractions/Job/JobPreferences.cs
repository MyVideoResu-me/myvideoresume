using MyVideoResume.Abstractions.Core;
using System.ComponentModel;

namespace MyVideoResume.Abstractions.Job;

public class JobPreferences : CommonBase
{
    public string? UserHandle { get; set; }
    public Industry Industry { get; set; } = Industry.Management;
    public ExperienceLevel Seniority { get; set; } = ExperienceLevel.Entry;
    public List<JobType> EmploymentType { get; set; } = new List<JobType>();
    public PaySchedule PaySchedule { get; set; } = PaySchedule.Yearly;
    public float MinimumSalary { get; set; }
    public string UserId { get; set; }
    public List<WorkSetting> WorkSetting { get; set; } = new List<WorkSetting>();
}
