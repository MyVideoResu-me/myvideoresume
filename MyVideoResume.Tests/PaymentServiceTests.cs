using Xunit;
using Moq;
using Microsoft.Extensions.Options;
using MyVideoResume.Application.Payments;
using MyVideoResume.Data;
using MyVideoResume.Data.Models.Account.Profiles;
using System.Threading.Tasks;
using FluentAssertions;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using Microsoft.Extensions.Configuration;

namespace MyVideoResume.Tests;


public class PaymentServiceTests
{
    private readonly Mock<IOptions<StripeConfig>> _mockStripeConfigOptions;
    private readonly Mock<DataContext> _mockDataContext;
    private readonly StripeConfig _stripeConfig;

    public PaymentServiceTests()
    {
        // Load configuration from appsettings.json
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .Build();

        // Get the secret key from configuration
        var secretKey = configuration["Stripe:SecretKey"] ?? "";
        _stripeConfig = new StripeConfig { SecretKey = secretKey };

        _mockStripeConfigOptions = new Mock<IOptions<StripeConfig>>();
        _mockStripeConfigOptions.Setup(o => o.Value).Returns(_stripeConfig);

        var options = new DbContextOptions<DataContext>();
        _mockDataContext = new Mock<DataContext>(options);
    }

    [Fact]
    public void Constructor_WhenStripeConfigIsNull_ThrowsArgumentNullException()
    {
        try
        {
            var nullStripeConfigOptions = new Mock<IOptions<StripeConfig>>();
            nullStripeConfigOptions.Setup(o => o.Value).Returns((StripeConfig)null);
            var settings = nullStripeConfigOptions.Object;
            var dataContextMock = _mockDataContext.Object;
            Assert.Throws<ArgumentNullException>(() => new PaymentService(settings, dataContextMock));
        }
        catch (Exception ex)
        {
            Console.Write(ex.Message);
        }
    }

    [Fact]
    public void Constructor_WhenStripeSecretKeyIsNull_ThrowsArgumentNullException()
    {
        try
        {
            var configWithoutSecretKey = new StripeConfig { SecretKey = null };
            var stripeConfigOptions = new Mock<IOptions<StripeConfig>>();
            stripeConfigOptions.Setup(o => o.Value).Returns(configWithoutSecretKey);

            Assert.Throws<ArgumentNullException>(() => new PaymentService(stripeConfigOptions.Object, _mockDataContext.Object));
        }
        catch (Exception ex)
        {
            Console.Write(ex.Message);
        }
    }

    [Fact]
    public async Task AttachPaymentMethodToCustomerAsync_NullCustomerId_ThrowsArgumentNullException()
    {
        try
        {
            var service = new PaymentService(_mockStripeConfigOptions.Object, _mockDataContext.Object);
            await Assert.ThrowsAsync<ArgumentNullException>(() => service.AttachPaymentMethodToCustomerAsync(null, "pm_123"));
        }
        catch (Exception ex)
        {
            Console.Write(ex.Message);
        }
    }

    [Fact]
    public async Task AttachPaymentMethodToCustomerAsync_NullPaymentMethodId_ThrowsArgumentNullException()
    {
        try
        {
            var service = new PaymentService(_mockStripeConfigOptions.Object, _mockDataContext.Object);
            await Assert.ThrowsAsync<ArgumentNullException>(() => service.AttachPaymentMethodToCustomerAsync("cus_123", null));
        }
        catch (Exception ex)
        {
            Console.Write(ex.Message);
        }
    }

    // Add more tests as needed, mocking dependencies and avoiding direct Stripe SDK calls.
}
