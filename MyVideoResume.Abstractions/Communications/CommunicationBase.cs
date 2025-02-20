using MyVideoResume.Abstractions.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyVideoResume.Abstractions.Communications;

public interface ICommunicationItem
{
    string Title { get; set; }

    string Body { get; set; }

    CommunicationTypes CommunicationType { get; set; }
    DateTime? ReadAcknowledgeDateTime { get; set; }

    string SenderId { get; set; }

    string RecipientId { get; set; }
}


public class CommunicationBase : CommonBase, ICommunicationItem
{
    public string Title { get; set; }

    public string Body { get; set; }

    //User who initiated
    public string SenderId { get; set; }

    //User who is Receiving 
    public string RecipientId { get; set; }

    //Company associated with the item
    public string? CompanyId { get; set; }

    public DateTime? ReadAcknowledgeDateTime { get; set; }

    public CommunicationTypes CommunicationType { get; set; }
}