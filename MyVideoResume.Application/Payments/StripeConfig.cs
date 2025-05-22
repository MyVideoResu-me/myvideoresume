using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyVideoResume.Application.Payments;

public class StripeConfig
{
    public string PublishableKey { get; set; }
    public string SecretKey { get; set; }
    public string WebhookSigningKey { get; set; }
}
