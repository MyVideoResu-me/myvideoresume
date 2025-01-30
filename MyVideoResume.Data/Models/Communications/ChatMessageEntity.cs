using Microsoft.EntityFrameworkCore;
using MyVideoResume.Abstractions.Communications;
using MyVideoResume.Data.Models.Account.Profiles;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyVideoResume.Data.Models.Communications;

[Table("ChatMessages")]
public class ChatMessageEntity : CommunicationBase
{

    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }
    [DeleteBehavior(DeleteBehavior.NoAction)]
    public UserProfileEntity Sender { get; set; }
    [DeleteBehavior(DeleteBehavior.NoAction)]
    public UserProfileEntity Recipient { get; set; }
}
