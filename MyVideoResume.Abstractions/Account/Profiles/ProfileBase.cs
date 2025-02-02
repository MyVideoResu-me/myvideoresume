using MyVideoResume.Abstractions.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyVideoResume.Abstractions.Account.Profiles;

public class ProfileBase : CommonBase
{
    //User who created
    public string UserId { get; set; }

    public List<string>? SocialProfiles { get; set; }

    
    public DateTime? TermsOfUseAgreementAcceptedDateTime { get; set; }
    public string? TermsOfUserAgreementVersion { get; set; }

    public bool IsPaidAccount { get; set; }
    public DateTime? IsPaidAccountDateTime { get; set; }
    public double? PaidPurchasePrice { get; set; }
    public DateTime? PaidPurchaseDateTime { get; set; }
    public AccountType? AccountType { get; set; }
    public AccountUsageType? AccountUsageType { get; set; }
}
