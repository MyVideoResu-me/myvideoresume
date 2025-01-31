using Microsoft.EntityFrameworkCore;
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
    System, //System Recommendation or Match
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

    public Guid? JobItemEntityId { get; set; }

    public Guid? UserProfileEntityApplyingId { get; set; }

    public Guid? ResumeInformationEntityId { get; set; }

    public JobApplicationStatus JobApplicationStatus { get; set; }

    public string MatchResults { get; set; }
    public DateTime MatchResultsDate { get; set; }
    public float MatchScoreRating { get; set; }
}
