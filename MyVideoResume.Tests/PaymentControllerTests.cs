using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http;
using System.Threading.Tasks;
using MyVideoResume.Application.Payments; // For IPaymentService and StripeConfig
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Json; // For ReadFromJsonAsync and PostAsJsonAsync
using FluentAssertions;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text; // For StringContent
using Newtonsoft.Json; // For JsonConvert
using Stripe; // For StripeException

// Define a minimal Server.Program for WebApplicationFactory if not directly accessible
// This is often needed if Program.cs is not public or has complex setup not relevant for tests.
// However, usually WebApplicationFactory<TEntryPoint> works directly with Program.cs in the Server project.
// For this test, we'll assume WebApplicationFactory<MyVideoResume.Server.Program> can be used.
// If MyVideoResume.Server.Program is not directly usable due to internal visibility or complex setup,
// a public TStartup class in the Server project might be needed.

public class TestAuthHandlerOptions : AuthenticationSchemeOptions
{
}

// Custom Authentication Handler to simulate an authenticated user for tests
public class TestAuthHandler : AuthenticationHandler<TestAuthHandlerOptions>
{
    public const string AuthenticationScheme = "TestScheme";
    private readonly string _defaultUserId;
    private readonly string _defaultUserEmail;

    public TestAuthHandler(IOptionsMonitor<TestAuthHandlerOptions> options, ILoggerFactory logger, System.Text.Encodings.Web.UrlEncoder encoder)
        : base(options, logger, encoder)
    {
        _defaultUserId = "test-user-id";
        _defaultUserEmail = "testuser@example.com";
    }
    
    // Overload for specific user
    public TestAuthHandler(IOptionsMonitor<TestAuthHandlerOptions> options, ILoggerFactory logger, System.Text.Encodings.Web.UrlEncoder encoder, string userId, string userEmail)
        : base(options, logger, encoder)
    {
         _defaultUserId = userId;
         _defaultUserEmail = userEmail;
    }


    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var claims = new[] {
            new Claim(ClaimTypes.NameIdentifier, _defaultUserId),
            new Claim(ClaimTypes.Name, "Test User"),
            new Claim(ClaimTypes.Email, _defaultUserEmail)
            // Add other claims as needed by your application
        };
        var identity = new ClaimsIdentity(claims, AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, AuthenticationScheme);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}


public class PaymentControllerTests : IClassFixture<WebApplicationFactory<MyVideoResume.Server.Program>>
{
    private readonly WebApplicationFactory<MyVideoResume.Server.Program> _factory;
    private readonly Mock<IPaymentService> _mockPaymentService;

    public PaymentControllerTests(WebApplicationFactory<MyVideoResume.Server.Program> factory)
    {
        _mockPaymentService = new Mock<IPaymentService>();

        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                // Remove the real IPaymentService registration if it exists
                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IPaymentService));
                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }
                // Add the mock IPaymentService
                services.AddScoped<IPaymentService>(_ => _mockPaymentService.Object);

                // Configure test authentication
                services.AddAuthentication(TestAuthHandler.AuthenticationScheme)
                    .AddScheme<TestAuthHandlerOptions, TestAuthHandler>(TestAuthHandler.AuthenticationScheme, options => { });
            });
        });
    }

    private HttpClient GetAuthenticatedClient(string userId = "test-user-id", string userEmail = "testuser@example.com")
    {
        var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false,
        });
        // To use the TestAuthHandler, we'd typically set a default scheme or ensure it's picked up.
        // For these tests, the AddAuthentication in ConfigureServices should make it the default.
        // If specific user for specific test is needed, TestAuthHandler would need to be more dynamic or configured per test.
        // The current TestAuthHandler uses a hardcoded default user. For simplicity, we'll use that.
        // We rely on the global setup of TestAuthHandler.
        // If we needed to pass user ID/email to TestAuthHandler per client:
        // This would involve more complex setup, potentially custom WebApplicationFactory for each user type,
        // or a mechanism to signal the desired user to TestAuthHandler (e.g., via a header that the handler reads).
        // For now, the single default test user in TestAuthHandler will be used.
        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("TestScheme");
        return client;
    }
    
    private HttpClient GetUnauthenticatedClient()
    {
        return _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false,
        });
    }

    // --- DTOs used by controller (mirroring what's in PaymentController.cs or relevant DTO files) ---
    private class ClientSecretResponse
    {
        public string ClientSecret { get; set; }
    }

    private class SavePaymentMethodClientRequest // Name to avoid conflict with controller's internal DTO
    {
        public string PaymentMethodId { get; set; }
    }
    
    private class ErrorResponse
    {
        public string Message { get; set; }
    }


    // --- Tests for POST api/payment/setup-intent ---

    [Fact]
    public async Task SetupIntent_AuthenticatedUser_ReturnsOkWithClientSecret()
    {
        // Arrange
        var client = GetAuthenticatedClient();
        var expectedCustomerId = "cus_test123";
        var expectedClientSecret = "seti_testsecret123_clientsidedummy";

        _mockPaymentService.Setup(s => s.GetOrCreateStripeCustomerAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(expectedCustomerId);
        _mockPaymentService.Setup(s => s.CreateSetupIntentAsync(expectedCustomerId))
            .ReturnsAsync(expectedClientSecret);

        // Act
        var response = await client.PostAsync("/api/payment/setup-intent", null);

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        var responseData = await response.Content.ReadFromJsonAsync<ClientSecretResponse>();
        responseData.Should().NotBeNull();
        responseData.ClientSecret.Should().Be(expectedClientSecret);

        _mockPaymentService.Verify(s => s.GetOrCreateStripeCustomerAsync("test-user-id", "testuser@example.com"), Times.Once);
        _mockPaymentService.Verify(s => s.CreateSetupIntentAsync(expectedCustomerId), Times.Once);
    }

    [Fact]
    public async Task SetupIntent_ServiceThrowsStripeException_ReturnsInternalServerError()
    {
        // Arrange
        var client = GetAuthenticatedClient();
        var stripeError = new StripeError { Message = "Test Stripe Error" };
        var stripeException = new StripeException("Simulated Stripe Exception", stripeError);
        
        _mockPaymentService.Setup(s => s.GetOrCreateStripeCustomerAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync("cus_test123"); // This part succeeds
        _mockPaymentService.Setup(s => s.CreateSetupIntentAsync(It.IsAny<string>()))
            .ThrowsAsync(stripeException);

        // Act
        var response = await client.PostAsync("/api/payment/setup-intent", null);

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.InternalServerError);
        var errorData = await response.Content.ReadFromJsonAsync<ErrorResponse>();
        errorData.Should().NotBeNull();
        errorData.Message.Should().Contain("A payment provider error occurred: Test Stripe Error");
    }

    [Fact]
    public async Task SetupIntent_UnauthenticatedUser_ReturnsUnauthorized()
    {
        // Arrange
        var client = GetUnauthenticatedClient();

        // Act
        var response = await client.PostAsync("/api/payment/setup-intent", null);

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.Unauthorized);
    }

    // --- Tests for POST api/payment/save-payment-method ---

    [Fact]
    public async Task SavePaymentMethod_AuthenticatedUser_ValidInput_ReturnsOk()
    {
        // Arrange
        var client = GetAuthenticatedClient();
        var request = new SavePaymentMethodClientRequest { PaymentMethodId = "pm_test123" };
        var expectedCustomerId = "cus_test123";

        _mockPaymentService.Setup(s => s.GetOrCreateStripeCustomerAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(expectedCustomerId);
        _mockPaymentService.Setup(s => s.AttachPaymentMethodToCustomerAsync(expectedCustomerId, request.PaymentMethodId))
            .Returns(Task.CompletedTask);

        // Act
        var response = await client.PostAsJsonAsync("/api/payment/save-payment-method", request);

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        var responseData = await response.Content.ReadFromJsonAsync<ErrorResponse>(); // Assuming ErrorResponse can also parse success messages
        responseData.Should().NotBeNull();
        responseData.Message.Should().Be("Payment method saved successfully.");

        _mockPaymentService.Verify(s => s.GetOrCreateStripeCustomerAsync("test-user-id", "testuser@example.com"), Times.Once);
        _mockPaymentService.Verify(s => s.AttachPaymentMethodToCustomerAsync(expectedCustomerId, request.PaymentMethodId), Times.Once);
    }
    
    [Fact]
    public async Task SavePaymentMethod_ServiceThrowsStripeException_ReturnsBadRequest() // Controller maps StripeEx to 400 for this endpoint
    {
        // Arrange
        var client = GetAuthenticatedClient();
        var request = new SavePaymentMethodClientRequest { PaymentMethodId = "pm_problematic123" };
        var stripeError = new StripeError { Message = "Cannot attach this payment method." };
        var stripeException = new StripeException("Simulated Stripe Exception during attach", stripeError);

        _mockPaymentService.Setup(s => s.GetOrCreateStripeCustomerAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync("cus_test123");
        _mockPaymentService.Setup(s => s.AttachPaymentMethodToCustomerAsync(It.IsAny<string>(), request.PaymentMethodId))
            .ThrowsAsync(stripeException);

        // Act
        var response = await client.PostAsJsonAsync("/api/payment/save-payment-method", request);

        // Assert
        // Based on PaymentController logic: catch (StripeException stripeEx) returns StatusCode(StatusCodes.Status400BadRequest, ...)
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        var errorData = await response.Content.ReadFromJsonAsync<ErrorResponse>();
        errorData.Should().NotBeNull();
        errorData.Message.Should().Contain("A payment provider error occurred: Cannot attach this payment method.");
    }

    [Fact]
    public async Task SavePaymentMethod_AuthenticatedUser_InvalidInput_MissingPaymentMethodId_ReturnsBadRequest()
    {
        // Arrange
        var client = GetAuthenticatedClient();
        // Sending an empty object or one where PaymentMethodId is null/whitespace
        var request = new SavePaymentMethodClientRequest { PaymentMethodId = null }; 
        //var content = new StringContent("{}", Encoding.UTF8, "application/json"); // Alternative for empty body

        // Act
        var response = await client.PostAsJsonAsync("/api/payment/save-payment-method", request);

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        var errorData = await response.Content.ReadFromJsonAsync<ErrorResponse>();
        errorData.Should().NotBeNull();
        errorData.Message.Should().Be("PaymentMethodId is required.");
    }

    [Fact]
    public async Task SavePaymentMethod_UnauthenticatedUser_ReturnsUnauthorized()
    {
        // Arrange
        var client = GetUnauthenticatedClient();
        var request = new SavePaymentMethodClientRequest { PaymentMethodId = "pm_test123" };

        // Act
        var response = await client.PostAsJsonAsync("/api/payment/save-payment-method", request);

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.Unauthorized);
    }
}
