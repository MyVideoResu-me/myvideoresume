using MyVideoResume.Abstractions.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyVideoResume.Abstractions.Account.Profiles
{
    public class UserProfileRoleUpdateRequest
    {
        public UserProfileDTO UserProfile { get; set; }
        public MyVideoResumeRoles Role { get; set; }
    }
}
