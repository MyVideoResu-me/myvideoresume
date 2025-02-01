using Microsoft.EntityFrameworkCore;
using MyVideoResume.Abstractions.Account.Profiles;
using MyVideoResume.Abstractions.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyVideoResume.Data.Models.Account.Profiles;

[Table("Companies")]
public class CompanyProfileEntity : BusinessProfile
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    [DeleteBehavior(DeleteBehavior.NoAction)]
    public UserProfileEntity UserProfile { get; set; } //User Who Created the Company

    [DeleteBehavior(DeleteBehavior.NoAction)]
    public List<UserProfileEntity> CompanyUsers{ get; set; } //All The users associated with the Company

    public AddressEntity? MailingAddress { get; set; }

    public AddressEntity? BillingAddress { get; set; }

}
