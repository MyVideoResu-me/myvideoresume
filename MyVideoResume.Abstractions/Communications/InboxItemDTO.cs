using MyVideoResume.Abstractions.Communications;
using MyVideoResume.Abstractions.Core;

namespace MyVideoResume.Abstractions.Business;

public class InboxItemDTO : CommunicationBase
{
    public string Id { get; set; }

    public ContactPersonDTO Recipient { get; set; }
    public ContactPersonDTO Sender { get; set; }

    public int UnreadMessageTotal { get; set; }
}

public class ChatMessageDTO : CommunicationBase { 

    public string Id { get; set; }
    public ContactPersonDTO Recipient { get; set; }
    public ContactPersonDTO Sender { get; set; }

    //Add Reactions
}