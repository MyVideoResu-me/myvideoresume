using Microsoft.EntityFrameworkCore;
using MyVideoResume.Abstractions.Core;
using MyVideoResume.Data.Models.Account.Profiles;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyVideoResume.Data.Models.Account;

[Table("UserCompanyRolesAssociation")]
public class UserCompanyRoleAssociationEntity: CommonBase
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    public string UserId { get; set; } //User who created the Entry

    public InviteStatus InviteStatus { get; set; } //Could be an Owner.

    public DateTime? InviteStatusStartDateTime { get; set; }
    public DateTime? InviteStatusEndDateTime { get; set; }

    [DeleteBehavior(DeleteBehavior.NoAction)]
    public UserProfileEntity UserProfile { get; set; }

    [DeleteBehavior(DeleteBehavior.NoAction)]
    public CompanyProfileEntity CompanyProfile { get; set; }

    public List<MyVideoResumeRoles> RolesAssigned { get; set; }

    public DateTime? TermsOfUseAgreementAcceptedDateTime { get; set; }
    public string? TermsOfUserAgreementVersion { get; set; }

    public bool IsPaidAccount { get; set; }
    public DateTime? IsPaidAccountDateTime { get; set; }
    public double? PaidPurchasePrice { get; set; }
    public DateTime? PaidPurchaseDateTime { get; set; }
    public AccountUsageType? AccountUsageType { get; set; }

}
