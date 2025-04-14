using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyVideoResume.Abstractions.Core;

public class Email : ContactSource { }
public class Phone : ContactSource { }

public class ContactSource : CommonBase
{
    public string Value { get; set; }
    public CommunicationType ContactType { get; set; }
}
