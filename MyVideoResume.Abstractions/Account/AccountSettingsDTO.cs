using MyVideoResume.Abstractions.Account.Profiles;
using MyVideoResume.Abstractions.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyVideoResume.Abstractions.Account;

//Account Settings Page DTO 
//Flattens:
// - the Profile
//- Preferences
//- BillingInfo
public class AccountSettingsDTO
{
    public string Id { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public virtual string? Email { get; set; }

    public DateTime? DateOfBirth { get; set; }

    public bool? IsRoleSelected { get; set; }
    public DateTime? IsRoleSelectedDateTime { get; set; }

    public ProfileStatus? ProfileStatus { get; set; } //Hiring => Show Jobs they own that are public; OpenToWork => Show featured Resume
    public DateTime? ProfileStatusDataTime { get; set; }

    public MyVideoResumeRoles? RoleSelected { get; set; }

    public DisplayPrivacy? Privacy_ShowProfile { get; set; } = DisplayPrivacy.ToPublic;

    public DisplayPrivacy? Privacy_ShowProfileContactDetails { get; set; } = DisplayPrivacy.ToConnections;

    public List<string>? SocialProfiles { get; set; }


    public DateTime? TermsOfUseAgreementAcceptedDateTime { get; set; }
    public string? TermsOfUserAgreementVersion { get; set; }

    public bool IsPaidAccount { get; set; }
    public DateTime? IsPaidAccountDateTime { get; set; }
    public double? PaidPurchasePrice { get; set; }
    public DateTime? PaidPurchaseDateTime { get; set; }
    public AccountType? AccountType { get; set; }
    public AccountUsageType? AccountUsageType { get; set; }

    //Reset the password will be a request email. 


    //protected ApplicationUser user;
    //protected UserProfileDTO userProfile;
    //protected JobPreferencesEntity jobPreferences;

}