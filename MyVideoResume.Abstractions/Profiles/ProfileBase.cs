using MyVideoResume.Abstractions.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyVideoResume.Abstractions.Profiles;

public class ProfileBase : CommonBase
{
    //User who created
    public string UserId { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public string? Phone { get; set; }
    public DateTime? TermsOfUseAgreementAcceptedDateTime { get; set; }
    public string? TermsOfUserAgreementVersion { get; set; }

    public Boolean IsPaidAccount { get; set; }
}
