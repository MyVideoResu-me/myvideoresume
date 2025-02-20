using MyVideoResume.Abstractions.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyVideoResume.Abstractions.DataCollection;

public class RequestLog : CommonBase
{
    public string Url { get; set; }
    public string? Method { get; set; }
    public string? UserAgent { get; set; }
    public string? IpAddress { get; set; }
    public string? Referrer { get; set; }

    public string? ReferrerUserId { get; set; }
    public string? CampaignId { get; set; }

    //Logged in User who is visiting the artifact
    public string? UserId { get; set; }
    public string? DataCollectionId { get; set; }
    public DataCollectionTypes? DataCollectionType { get; set; }

    // New properties for parsed user-agent information
    public string? Browser { get; set; }      // Browser name
    public string? BrowserVersion { get; set; } // Browser version
    public string? OS { get; set; }            // Operating system name
    public string? OSVersion { get; set; }     // Operating system version
    public string? Device { get; set; }        // Device type

    //Processed after Initial Logging
    public string? UserProfileId { get; set; }
    public string? CompanyProfileId { get; set; }

    public BatchProcessStatus Status { get; set; }
    public DateTime? StartBatchProcessDateTime { get; set; }
    public DateTime? EndBatchProcessDateTime { get; set; }

}