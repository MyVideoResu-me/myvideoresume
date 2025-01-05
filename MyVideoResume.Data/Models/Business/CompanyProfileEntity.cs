using MyVideoResume.Abstractions.Business;
using MyVideoResume.Abstractions.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyVideoResume.Data.Models.Business;

[Table("Companies")]
public class CompanyProfileEntity : CompanyProfile
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    public AddressEntity? MailingAddress { get; set; }

    public AddressEntity? BillingAddress { get; set; }

}
