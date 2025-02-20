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

[Table("QueueJobToProcessEntity")]
public class QueueJobToProcessEntity : CommonBase
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    [DeleteBehavior(DeleteBehavior.NoAction)]
    public JobItemEntity? JobProcessed { get; set; }

    [DeleteBehavior(DeleteBehavior.NoAction)]
    public JobWebsiteEntity? Website { get; set; }

    public string Url { get; set; }
    public int Attempts { get; set; } //Max 2
    public DateTime? AttemptLastDateTime { get; set; }
    public string? FailedAttemptMessage { get; set; }

    public BatchProcessStatus Status { get; set; }
    public DateTime? StartBatchProcessDateTime { get; set; }
    public DateTime? EndBatchProcessDateTime { get; set; }
}
