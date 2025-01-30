using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyVideoResume.Abstractions.Core;

public enum DisplayPrivacy
{
    [Description("Visible to Public")]
    ToPublic = 0,
    [Description("Visible to Recruiters")]
    ToRecruiters = 1,
    [Description("Visible to Connections")]
    ToConnections = 2,
    [Description("Visible to Self (Private)")]
    ToSelf = 10
}

public enum MyVideoResumeRoles
{
    Admin = 0,
    AccountOwner = 10,
    AccountAdmin = 11,
    Employer = 20,
    EmployerManager = 21,
    EmployerEmployee = 22,
    EmployerExecutive = 23,
    JobSeeker = 30,
    Recruiter = 40,
}

public enum InviteStatus
{
    Invited = 0,
    Accepted = 1,
    Rejected = 2,
    Resent = 3,
    Expired = 4
}