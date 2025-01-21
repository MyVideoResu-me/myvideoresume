using MyVideoResume.Abstractions.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyVideoResume.Abstractions.Business;

public class CompanyProfile : CommonBase
{
    //User who owns the Task
    public string UserId { get; set; }

    public string Name { get; set; }

    public string? Description { get; set; }

    public string? Website { get; set; }

    public string? Phone { get; set; }

    public DateTime? TermsOfUseAgreementAcceptedDateTime { get; set; }
    public string? TermsOfUserAgreementVersion { get; set; }

    public DateTime? ApprovedDateTime { get; set; }

    //public double? PaidPurchasePrice { get; set; }
    //public ProcessingStatus Status { get; set; }
    //public DateTime? PaidPurchaseDateTime { get; set; }
}