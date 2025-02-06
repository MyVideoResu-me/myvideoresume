using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using MyVideoResume.Abstractions.Core;
using MyVideoResume.Abstractions.DataCollection;

namespace MyVideoResume.Data.Models.DataCollection;

[Table("DataCollectionRequestLogs")]
public class RequestLogEntity : RequestLog
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }
}