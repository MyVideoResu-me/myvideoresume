using Microsoft.EntityFrameworkCore;
using MyVideoResume.Abstractions.Business;
using MyVideoResume.Abstractions.Communications;
using MyVideoResume.Abstractions.Core;
using MyVideoResume.Abstractions.Resume;
using MyVideoResume.Data.Models.Business;
using MyVideoResume.Data.Models.Communications;
using MyVideoResume.Data.Models.Jobs;
using MyVideoResume.Data.Models.Resume;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyVideoResume.Data.Models;



[Table("Inbox")]
public class InboxEntity : CommunicationBase
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    //User Who Created the Message
    [DeleteBehavior(DeleteBehavior.NoAction)]
    public UserProfileEntity Sender { get; set; }

    //User Who Created the Message
    [DeleteBehavior(DeleteBehavior.NoAction)]
    public UserProfileEntity Recipient { get; set; }


    [DeleteBehavior(DeleteBehavior.NoAction)]
    public List<ChatMessageEntity>? ChatMessages { get; set; }


    [DeleteBehavior(DeleteBehavior.NoAction)]
    public CompanyProfileEntity? Company { get; set; }


    //What is the Message about? Can be a JOB or RESUME
    [DeleteBehavior(DeleteBehavior.NoAction)]
    public List<JobItemEntity>? Job { get; set; }
    [DeleteBehavior(DeleteBehavior.NoAction)]
    public List<ResumeInformationEntity>? Resume { get; set; }
}
