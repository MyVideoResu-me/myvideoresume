using MyVideoResume.Abstractions.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Principal;

namespace MyVideoResume.Data.Models.Jobs;

[Table("JobWebsites")]
public class JobWebsiteEntity : CommonBase
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    public string Url { get; set; }

    public string ParsingRegularExpression { get; set; }

    public bool Active { get; set; }

    public string? LastProcessingStatus { get; set; }
}
