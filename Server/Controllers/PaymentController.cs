using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MyVideoResume.Abstractions.Business.Tasks;
using MyVideoResume.Abstractions.Core;
using MyVideoResume.Abstractions.Job;
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

namespace MyVideoResume.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public partial class PaymentController : ControllerBase
{
    private readonly ILogger<PaymentController> _logger;
    private readonly string _webhookSecret;

    public PaymentController(ILogger<PaymentController> logger, IOptions<StripeConfig> options)
    {
        _logger = logger;
        _webhookSecret = options.Value.WebhookSigningKey;
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
