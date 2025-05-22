using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MyVideoResume.Abstractions.Core;
using MyVideoResume.Abstractions.Job;
using MyVideoResume.Abstractions.Productivity;
using MyVideoResume.Application.Job;
using MyVideoResume.Application.Payments;
using MyVideoResume.Application.Resume;
using MyVideoResume.Data.Models;
using MyVideoResume.Data.Models.Jobs;
using MyVideoResume.Data.Models.Resume;
using MyVideoResume.Documents;
using MyVideoResume.Web.Common;
using Stripe;
using System.Security.Claims;
using MyVideoResume.Application.Payments; // Added for IPaymentService
using System.Threading.Tasks; // Added for Task

namespace MyVideoResume.Server.Controllers;

// DTO for the save-payment-method endpoint
public class SavePaymentMethodRequest
{
    public string PaymentMethodId { get; set; }
}

[Route("api/[controller]")]
[ApiController]
public partial class PaymentController : ControllerBase
{
    private readonly ILogger<PaymentController> _logger;
    private readonly string _webhookSecret;
    private readonly IPaymentService _paymentService; // Added IPaymentService

    public PaymentController(ILogger<PaymentController> logger, IOptions<StripeConfig> options, IPaymentService paymentService) // Injected IPaymentService
    {
        _logger = logger;
        _webhookSecret = options.Value.WebhookSigningKey;
        _paymentService = paymentService; // Store injected service
    }

    [HttpPost("stripe/webhook")]
    public async Task<IActionResult> Post()
    {
        string json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();

        try
        {
            var stripeEvent = EventUtility.ConstructEvent(json,
                Request.Headers["Stripe-Signature"], _webhookSecret);
            switch (stripeEvent.Type)
            {
                case EventTypes.CustomerSourceUpdated:
                    //make sure payment info is valid
                    break;
                case EventTypes.CustomerSourceExpiring:
                    //send reminder email to update payment method
                    break;
                case EventTypes.ChargeFailed:
                    //do something
                    break;
            }
        }
        catch (StripeException e)
        {
            _logger.LogError(e.Message, e);
            return BadRequest(e.Message);
        }
        return Ok();
    }

    [HttpPost("setup-intent")]
    [Authorize]
    public async Task<IActionResult> CreateSetupIntent()
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userEmail = User.FindFirstValue(ClaimTypes.Email);

            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(userEmail))
            {
                _logger.LogWarning("User ID or Email not found in claims for setup-intent.");
                return BadRequest(new { message = "User identification failed." });
            }

            _logger.LogInformation($"Processing setup-intent for userId: {userId}, email: {userEmail}");

            string stripeCustomerId = await _paymentService.GetOrCreateStripeCustomerAsync(userId, userEmail);
            if (string.IsNullOrEmpty(stripeCustomerId))
            {
                _logger.LogError($"CreateSetupIntent: Could not get or create Stripe customer for userId: {userId}. This may be due to an issue in PaymentService or database connectivity.");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "There was a problem processing your customer information. Please try again later." });
            }
            
            _logger.LogInformation($"CreateSetupIntent: Got Stripe Customer ID: {stripeCustomerId} for userId: {userId}");

            string clientSecret = await _paymentService.CreateSetupIntentAsync(stripeCustomerId);
            if (string.IsNullOrEmpty(clientSecret))
            {
                _logger.LogError($"CreateSetupIntent: Could not create SetupIntent for Stripe Customer ID: {stripeCustomerId}. This might be an issue with Stripe API or configuration.");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Failed to initialize the payment setup process. Please try again." });
            }
            
            _logger.LogInformation($"CreateSetupIntent: Created SetupIntent with client secret for Stripe Customer ID: {stripeCustomerId}");
            return Ok(new { clientSecret }); // Ensure OkObjectResult for clientSecret
        }
        catch (ArgumentNullException argEx)
        {
            _logger.LogError(argEx, $"CreateSetupIntent: A required argument was null. UserId: {User.FindFirstValue(ClaimTypes.NameIdentifier)}");
            // This could be if stripeCustomerId was null passed to CreateSetupIntentAsync, though local checks should prevent it.
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An internal configuration error occurred. Please contact support." });
        }
        catch (StripeException stripeEx)
        {
            _logger.LogError(stripeEx, $"CreateSetupIntent: Stripe API error while creating SetupIntent for userId: {User.FindFirstValue(ClaimTypes.NameIdentifier)}. Stripe Error: {stripeEx.StripeError?.Message}");
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = $"A payment provider error occurred: {stripeEx.StripeError?.Message ?? "Please try again."}" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"CreateSetupIntent: Unexpected error for userId: {User.FindFirstValue(ClaimTypes.NameIdentifier)}");
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An unexpected error occurred while preparing the payment form. Please try again." });
        }
    }

    [HttpPost("save-payment-method")]
    [Authorize]
    public async Task<IActionResult> SavePaymentMethod([FromBody] SavePaymentMethodRequest request)
    {
        if (request == null || string.IsNullOrWhiteSpace(request.PaymentMethodId))
        {
            _logger.LogWarning("SavePaymentMethod: Called with invalid request body or empty PaymentMethodId.");
            return BadRequest(new { message = "PaymentMethodId is required." });
        }

        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userEmail = User.FindFirstValue(ClaimTypes.Email); // Get email for GetOrCreateStripeCustomerAsync

            if (string.IsNullOrEmpty(userId)) // userEmail can be null if customer should already exist, service handles it
            {
                _logger.LogWarning("SavePaymentMethod: User ID not found in claims.");
                return BadRequest(new { message = "User identification failed. Please ensure you are logged in." });
            }
            
            _logger.LogInformation($"SavePaymentMethod: Processing for userId: {userId}, PaymentMethodId: {request.PaymentMethodId}");

            string stripeCustomerId = await _paymentService.GetOrCreateStripeCustomerAsync(userId, userEmail);
            if (string.IsNullOrEmpty(stripeCustomerId))
            {
                 _logger.LogError($"SavePaymentMethod: Could not get or create Stripe customer for userId: {userId}.");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "There was a problem processing your customer information. Please try again later." });
            }

            _logger.LogInformation($"SavePaymentMethod: Got Stripe Customer ID: {stripeCustomerId} for userId: {userId}. Attaching payment method.");

            await _paymentService.AttachPaymentMethodToCustomerAsync(stripeCustomerId, request.PaymentMethodId);
            
            _logger.LogInformation($"SavePaymentMethod: Successfully attached PaymentMethodId: {request.PaymentMethodId} to Stripe Customer ID: {stripeCustomerId} for userId: {userId}");
            return Ok(new { message = "Payment method saved successfully." });
        }
        catch (ArgumentNullException argEx)
        {
             _logger.LogError(argEx, $"SavePaymentMethod: A required argument was null for userId: {User.FindFirstValue(ClaimTypes.NameIdentifier)}, PaymentMethodId: {request.PaymentMethodId}");
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An internal configuration error occurred while saving the payment method." });
        }
        catch (StripeException stripeEx)
        {
            _logger.LogError(stripeEx, $"SavePaymentMethod: Stripe API error for userId: {User.FindFirstValue(ClaimTypes.NameIdentifier)}, PaymentMethodId: {request.PaymentMethodId}. Stripe Error: {stripeEx.StripeError?.Message}");
            // Check for specific Stripe errors if applicable, e.g., card errors, though many are caught client-side.
            // A common error here might be trying to attach a payment method in a way that's not allowed.
            return StatusCode(StatusCodes.Status400BadRequest, new { message = $"A payment provider error occurred: {stripeEx.StripeError?.Message ?? "Could not save payment method."}" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"SavePaymentMethod: Unexpected error for userId: {User.FindFirstValue(ClaimTypes.NameIdentifier)}, PaymentMethodId: {request.PaymentMethodId}.");
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An unexpected error occurred while saving your payment method. Please try again." });
        }
    }

    // The existing Get and Delete methods are below.
    // They are not part of this subtask's changes but are kept for completeness.
    [HttpGet]
    [Authorize]
    public async Task<ActionResult<List<TaskDTO>>> Get()
    {
        var result = new List<TaskDTO>();
        try
        {
            //result = await _service.GetJobSummaryItems(onlyPublic: true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
        }
        return result;
    }

    [Authorize]
    [HttpPost("{id}")]
    public async Task<ActionResult<ResponseResult>> Delete(string id)
    {
        var result = new ResponseResult();
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            //result = await _service.DeleteJob(id, jobId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            result.Result = ex.Message;
        }
        return result;
    }
}
