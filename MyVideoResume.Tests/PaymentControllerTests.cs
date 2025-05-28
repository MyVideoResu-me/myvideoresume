using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using MyVideoResume.Application.Payments;
using System.Threading.Tasks;
using MyVideoResume.Server.Controllers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MyVideoResume.Tests;

public class PaymentControllerTests
{
    [Fact]
    public async Task SavePaymentMethod_InvalidInput_MissingPaymentMethodId_ReturnsBadRequest()
    {
        // Arrange
        var paymentServiceMock = new Mock<IPaymentService>();
        var loggerMock = new Mock<ILogger<PaymentController>>();
        var stripeConfigMock = new Mock<IOptions<StripeConfig>>();
        stripeConfigMock.Setup(x => x.Value).Returns(new StripeConfig());
        var controller = new MyVideoResume.Server.Controllers.PaymentController(
            loggerMock.Object,
            stripeConfigMock.Object,
            paymentServiceMock.Object
        );

        // Act
        var result = await controller.SavePaymentMethod(null);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }


    // Add more controller tests as needed, using direct controller instantiation and mocks.
}
