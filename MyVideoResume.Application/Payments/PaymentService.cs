using Microsoft.Extensions.Options;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyVideoResume.Data; // Assuming DataContext is here
using MyVideoResume.Data.Models; // Assuming ApplicationUser is here
// It's good practice to have a specific configuration model for Stripe
// public class StripeConfig { public string SecretKey { get; set; } public string PublishableKey { get; set; } }

namespace MyVideoResume.Application.Payments
{
    // IMPORTANT: This service contains placeholder logic for database interactions.
    // These sections MUST be implemented with actual database operations (e.g., using Entity Framework Core)
    // and include proper error handling (e.g., DbUpdateException, handling not found scenarios, etc.)
    // for a production-ready system.
    public class PaymentService : IPaymentService
    {
        private readonly DataContext _context; // Assuming DataContext is your EF Core DbContext
        private readonly StripeClient _stripeClient;
        private readonly string _stripeSecretKey;

        public PaymentService(IOptions<StripeConfig> stripeConfig, DataContext context)
        {
            _context = context;
            if (stripeConfig?.Value?.SecretKey == null)
            {
                throw new ArgumentNullException(nameof(stripeConfig), "Stripe SecretKey is not configured.");
            }
            _stripeSecretKey = stripeConfig.Value.SecretKey;
            // It's recommended to use an HttpClient instance provided by IHttpClientFactory for StripeClient
            // For simplicity here, we'll let the Stripe SDK manage its HttpClient.
            _stripeClient = new StripeClient(_stripeSecretKey);
            // Or, you can set the static API key (older approach):
            // StripeConfiguration.ApiKey = _stripeSecretKey;
        }

        public async Task<string> GetOrCreateStripeCustomerAsync(string userId, string userEmail)
        {
            // --- Database Interaction Placeholder ---
            // TODO: Replace the following simulated database logic with actual database operations.
            // This involves:
            // 1. Querying your database (e.g., ApplicationUsers table) for the user by `userId`.
            // 2. Checking if the user record exists and if they already have a `StripeCustomerId`.
            // 3. If a `StripeCustomerId` exists, return it.
            // 4. If not, after creating the customer in Stripe (see below), save the new `stripeCustomer.Id`
            //    to the user's record in your database.
            // 5. Implement proper error handling for database operations (e.g., user not found, DbUpdateException).

            // Example (conceptual - adapt to your actual User model and DataContext):
            /*
            var applicationUser = await _context.ApplicationUsers.FirstOrDefaultAsync(u => u.Id == userId);
            if (applicationUser == null)
            {
                // Handle user not found scenario - this should ideally not happen if userId is from authenticated claims.
                throw new InvalidOperationException($"User with ID {userId} not found.");
            }

            if (!string.IsNullOrWhiteSpace(applicationUser.StripeCustomerId))
            {
                Console.WriteLine($"GetOrCreateStripeCustomerAsync: Found existing StripeCustomerId '{applicationUser.StripeCustomerId}' for userId: {userId}");
                return applicationUser.StripeCustomerId;
            }
            */
            
            // Current placeholder simulation:
            Console.WriteLine($"GetOrCreateStripeCustomerAsync: Simulating DB check for StripeCustomerId for userId: {userId}. (No actual DB call).");
            // string existingStripeCustomerId = null; // Simulate not found initially to proceed with creation logic.
            // --- End Database Interaction Placeholder ---

            var customerOptions = new CustomerCreateOptions
            {
                Email = userEmail,
                Description = $"Customer for userId: {userId}",
                Metadata = new Dictionary<string, string> { { "UserId", userId } } // Useful for reconciliation
            };

            var customerService = new CustomerService(_stripeClient);
            Customer stripeCustomer;
            try
            {
                stripeCustomer = await customerService.CreateAsync(customerOptions);
                Console.WriteLine($"GetOrCreateStripeCustomerAsync: Stripe customer created with ID: {stripeCustomer.Id} for userId: {userId}.");
            }
            catch (StripeException ex)
            {
                Console.WriteLine($"GetOrCreateStripeCustomerAsync: Stripe API error while creating customer for userId {userId}: {ex.Message}");
                // Depending on the error, you might want to throw or handle differently.
                // For example, if customer already exists with that email, Stripe might error or have specific handling.
                throw; // Re-throw for the controller to handle, or implement more specific error handling.
            }


            // --- Database Interaction Placeholder (Saving new StripeCustomerId) ---
            // TODO: Save stripeCustomer.Id to your user model in the database here.
            /*
            applicationUser.StripeCustomerId = stripeCustomer.Id;
            try
            {
                _context.ApplicationUsers.Update(applicationUser);
                await _context.SaveChangesAsync();
                Console.WriteLine($"GetOrCreateStripeCustomerAsync: Successfully saved new StripeCustomerId '{stripeCustomer.Id}' to DB for userId: {userId}");
            }
            catch (DbUpdateException dbEx)
            {
                // Handle database save failure
                Console.WriteLine($"GetOrCreateStripeCustomerAsync: Failed to save new StripeCustomerId for userId {userId} to database: {dbEx.Message}");
                // Depending on policy, you might need to decide if this is a critical failure.
                // For now, we'll assume the Stripe customer was created and proceed, but this needs robust handling.
                throw; // Or handle more gracefully
            }
            */
            Console.WriteLine($"GetOrCreateStripeCustomerAsync: TODO: Save StripeCustomerId '{stripeCustomer.Id}' to DB for userId: {userId}. (No actual DB call).");
            // --- End Database Interaction Placeholder ---

            return stripeCustomer.Id;
        }

        public async Task<string> CreateSetupIntentAsync(string stripeCustomerId)
        {
            if (string.IsNullOrWhiteSpace(stripeCustomerId))
            {
                throw new ArgumentNullException(nameof(stripeCustomerId), "Stripe Customer ID cannot be null or empty.");
            }

            var options = new SetupIntentCreateOptions
            {
                Customer = stripeCustomerId,
                PaymentMethodTypes = new List<string> { "card" },
                Usage = "on_session", // Or "off_session" depending on your flow
            };

            var service = new SetupIntentService(_stripeClient);
            SetupIntent setupIntent = await service.CreateAsync(options);

            return setupIntent.ClientSecret;
        }

        public async Task AttachPaymentMethodToCustomerAsync(string stripeCustomerId, string paymentMethodId)
        {
            if (string.IsNullOrWhiteSpace(stripeCustomerId))
            {
                throw new ArgumentNullException(nameof(stripeCustomerId), "Stripe Customer ID cannot be null or empty.");
            }
            if (string.IsNullOrWhiteSpace(paymentMethodId))
            {
                throw new ArgumentNullException(nameof(paymentMethodId), "Payment Method ID cannot be null or empty.");
            }

            // Attach the PaymentMethod to the Customer
            var paymentMethodAttachOptions = new PaymentMethodAttachOptions
            {
                Customer = stripeCustomerId,
            };
            var paymentMethodService = new PaymentMethodService(_stripeClient);
            await paymentMethodService.AttachAsync(paymentMethodId, paymentMethodAttachOptions);

            Console.WriteLine($"PaymentMethod {paymentMethodId} attached to customer {stripeCustomerId}.");

            // Optionally, set it as the default payment method for invoices
            var customerUpdateOptions = new CustomerUpdateOptions
            {
                InvoiceSettings = new CustomerInvoiceSettingsOptions
                {
                    DefaultPaymentMethod = paymentMethodId,
                }
            };
            var customerService = new CustomerService(_stripeClient);
            await customerService.UpdateAsync(stripeCustomerId, customerUpdateOptions);
            
            Console.WriteLine($"Customer {stripeCustomerId} updated to use PaymentMethod {paymentMethodId} as default for invoices.");
        }
    }
}
