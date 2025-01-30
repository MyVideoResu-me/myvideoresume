using MyVideoResume.Abstractions.Core;
using MyVideoResume.Data.Models.Account.Profiles;
using MyVideoResume.Data.Models.Resume;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyVideoResume.Data.Models.Jobs;

public enum JobApplicationStatus
{
    Interested,
    Saved,
    Applied,
    Closed,
    Hired
}

[Table("ApplicantToJob")]
public class ApplicantToJobEntity : CommonBase
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    public JobItemEntity Job { get; set; }

    public UserProfileEntity UserApplying { get; set; }

    public ResumeInformationEntity ResumeItem { get; set; }

    public JobApplicationStatus JobApplicationStatus { get; set; }

    public string MatchResults { get; set; }
    public DateTime MatchResultsDate { get; set; }
    public float MatchScoreRating { get; set; }
}
