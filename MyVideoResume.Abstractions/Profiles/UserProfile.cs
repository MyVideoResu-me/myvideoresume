﻿using Microsoft.EntityFrameworkCore;
using MyVideoResume.Abstractions.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyVideoResume.Abstractions.Profiles;

public class UserProfileDTO : UserProfile
{
    public string Id { get; set; }
}

public class UserProfile : ProfileBase
{
    public string FirstName { get; set; }

    public string LastName { get; set; }

    public DateTime? DateOfBirth { get; set; }

    public bool? IsRoleSelected { get; set; }

    public DateTime? IsRoleSelectedDateTime { get; set; }

}
