using MyVideoResume.Abstractions.Core;
using MyVideoResume.Data.Models.Communications;
using MyVideoResume.Data.Models.Productivity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyVideoResume.Data.Models.Jobs;

[Table("ApplicantToJob")]
public class ApplicantToJobEntity : CommonBase
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    public Guid? JobItemEntityId { get; set; }

    public Guid? JobCampaignId { get; set; }

    public Guid? UserProfileEntityApplyingId { get; set; }

    public Guid? ResumeInformationEntityId { get; set; }

    public JobApplicationStatus JobApplicationStatus { get; set; }
    public RejectionStatus? RejectionStatus { get; set; }

    public string MatchResults { get; set; }
    public DateTime MatchResultsDate { get; set; }
    public float MatchScoreRating { get; set; }

    public List<TaskEntity> Tasks { get; set; }
    public string? Intake { get; set; } //HTML notes about this application

    public List<ActivityEntity> Activities { get; set; }
}
