using Microsoft.EntityFrameworkCore;
using MyVideoResume.Abstractions.Business;
using MyVideoResume.Abstractions.Communications;
using MyVideoResume.Abstractions.Core;
using MyVideoResume.Data.Models.Account.Profiles;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyVideoResume.Data.Models;

[Table("Alerts")]
public class AlertEntity : CommunicationBase
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    [DeleteBehavior(DeleteBehavior.NoAction)]
    public UserProfileEntity User { get; set; }

    [DeleteBehavior(DeleteBehavior.NoAction)]
    public CompanyProfileEntity? Company { get; set; }
}
