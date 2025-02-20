using MyVideoResume.Abstractions.Core;
using MyVideoResume.Data.Models.Account.Profiles;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyVideoResume.Data.Models.Resume; 

[Table("WatchedResumes")]
public class WatchedResumeEntity : CommonBase
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    [Required]
    public Guid UserId { get; set; }

    [ForeignKey("UserId")]
    public UserProfileEntity UserProfile { get; set; }

    [Required]
    public Guid ResumeId { get; set; }

    [ForeignKey("ResumeId")]
    public ResumeInformationEntity ResumeInformation { get; set; }

    public DateTime WatchedDateTime { get; set; } = DateTime.UtcNow;
}
