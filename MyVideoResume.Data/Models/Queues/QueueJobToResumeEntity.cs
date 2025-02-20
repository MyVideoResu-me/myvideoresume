using Microsoft.EntityFrameworkCore;
using MyVideoResume.Abstractions.Core;
using MyVideoResume.Abstractions.Job;
using MyVideoResume.Data.Models.Account.Profiles;
using MyVideoResume.Data.Models.Jobs;
using MyVideoResume.Data.Models.Resume;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyVideoResume.Data.Models.Queues;

[Table("QueueJobToResume")]
public class QueueJobToResumeEntity : CommonBase
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    [DeleteBehavior(DeleteBehavior.NoAction)]
    public JobItemEntity? Job { get; set; }

    public List<ApplicantToJobEntity>? Applications { get; set; }

    public BatchProcessStatus Status { get; set; }
    public DateTime? StartBatchProcessDateTime { get; set; }
    public DateTime? EndBatchProcessDateTime { get; set; }
}
