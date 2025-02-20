using Microsoft.EntityFrameworkCore;
using MyVideoResume.Abstractions.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyVideoResume.Abstractions.Account.Profiles;

public class UserProfileDTO : UserProfile
{
    public string Id { get; set; }
    public List<Phone>? Phones { get; set; }
    public List<Email>? Emails { get; set; }
}

public class UserProfile : ProfileBase
{
    public string FirstName { get; set; }

    public string LastName { get; set; }

    public DateTime? DateOfBirth { get; set; }

    public bool? IsRoleSelected { get; set; }
    public DateTime? IsRoleSelectedDateTime { get; set; }

    public ProfileStatus? ProfileStatus { get; set; } //Hiring => Show Jobs they own that are public; OpenToWork => Show featured Resume
    public DateTime? ProfileStatusDataTime { get; set; }

    public MyVideoResumeRoles? RoleSelected { get; set; }

    public DisplayPrivacy? Privacy_ShowProfile { get; set; } = DisplayPrivacy.ToPublic;

    public DisplayPrivacy? Privacy_ShowProfileContactDetails { get; set; } = DisplayPrivacy.ToConnections;

}
