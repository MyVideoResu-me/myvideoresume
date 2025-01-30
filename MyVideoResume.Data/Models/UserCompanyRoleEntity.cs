using Microsoft.EntityFrameworkCore;
using MyVideoResume.Abstractions.Core;
using MyVideoResume.Data.Models.Account.Profiles;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyVideoResume.Data.Models;

[Table("UserCompanyRoles")]
public class UserCompanyRoleEntity: CommonBase
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    public InviteStatus InviteStatus { get; set; }
    public DateTime? InviteStatusStartDateTime { get; set; }
    public DateTime? InviteStatusEndDateTime { get; set; }

    [DeleteBehavior(DeleteBehavior.NoAction)]
    public UserProfileEntity UserProfile { get; set; }

    [DeleteBehavior(DeleteBehavior.NoAction)]
    public CompanyProfileEntity CompanyProfile { get; set; }

    public List<MyVideoResumeRoles> RolesAssigned { get; set; }
}
