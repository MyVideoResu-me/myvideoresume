using System.Threading.Tasks;

namespace MyVideoResume.Application.Payments
{
    public interface IPaymentService
    {
        Task<string> GetOrCreateStripeCustomerAsync(string userId, string userEmail);
        Task<string> CreateSetupIntentAsync(string stripeCustomerId);
        Task AttachPaymentMethodToCustomerAsync(string stripeCustomerId, string paymentMethodId);
    }
}
