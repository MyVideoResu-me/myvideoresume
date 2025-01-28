using Microsoft.EntityFrameworkCore;
using MyVideoResume.Abstractions.Core;
using MyVideoResume.Abstractions.Profiles;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyVideoResume.Data.Models.Business;

[Table("Companies")]
public class CompanyProfileEntity : BusinessProfile
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    [DeleteBehavior(DeleteBehavior.NoAction)]
    public UserProfileEntity UserProfile { get; set; }

    public AddressEntity? MailingAddress { get; set; }

    public AddressEntity? BillingAddress { get; set; }

}
