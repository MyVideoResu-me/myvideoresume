using Microsoft.EntityFrameworkCore;
using MyVideoResume.Abstractions.Core;
using MyVideoResume.Data.Models.Jobs;
using MyVideoResume.Data.Models.Profiles;
using MyVideoResume.Data.Models.Resume;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyVideoResume.Data.Models;

public enum MyVideoResumeRoles {

    Admin=0,
    AccountOwner = 10,
    AccountAdmin = 11,
    Employer = 20,
    EmployerManager = 21,
    EmployerEmployee = 22,
    EmployerExecutive = 23,
    JobSeeker = 30,
    Recruiter = 40,
}

public enum InviteStatus { 

    Invited = 0,
    Accepted = 1,
    Rejected = 2,
    Resent = 3,
    Expired = 4
}


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
