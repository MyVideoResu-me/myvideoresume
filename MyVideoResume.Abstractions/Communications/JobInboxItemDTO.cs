using MyVideoResume.Abstractions.Communications;
using MyVideoResume.Abstractions.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyVideoResume.Abstractions.Business;

public class JobInboxItemDTO : InboxItemDTO
{
    public string JobId { get; set; }
}