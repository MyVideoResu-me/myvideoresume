using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using MyVideoResume.Application;
using MyVideoResume.Application.Account;
using MyVideoResume.Application.Productivity;
using MyVideoResume.Data;
using MyVideoResume.Data.Models;
using MyVideoResume.Data.Models.Account;
using MyVideoResume.Server.Controllers;
using MyVideoResume.Services;
using Xunit;

namespace MyVideoResume.Tests;

public class AccountControllerTests
{
    private readonly Mock<SignInManager<ApplicationUser>> signInManagerMock;
    private readonly Mock<UserManager<ApplicationUser>> userManagerMock;
    private readonly Mock<RoleManager<ApplicationRole>> roleManagerMock;
    private readonly Mock<IWebHostEnvironment> envMock;
    private readonly Mock<ILogger<AccountController>> loggerMock;
    private readonly Mock<IEmailService> emailServiceMock;
    private readonly Mock<IConfiguration> configurationMock;
    private readonly Mock<AccountService> accountServiceMock;
    private readonly AccountController controller;

    public AccountControllerTests()
    {
        // Mock dependencies for ProductivityService
        var dataContextMock = new Mock<DataContext>();
        var loggerMock = new Mock<ILogger<ProductivityService>>();
        var mapperMock = new Mock<IMapper>();

        // Create a mock for ProductivityService with valid constructor arguments
        var productivityServiceMock = new Mock<ProductivityService>(
            dataContextMock.Object,
            loggerMock.Object,
            mapperMock.Object
        );

        // Other mocks
        signInManagerMock = new Mock<SignInManager<ApplicationUser>>(
            new Mock<UserManager<ApplicationUser>>(
                new Mock<IUserStore<ApplicationUser>>().Object,
                null, null, null, null, null, null, null, null).Object,
            new Mock<IHttpContextAccessor>().Object,
            new Mock<IUserClaimsPrincipalFactory<ApplicationUser>>().Object,
            null, null, null, null);

        userManagerMock = new Mock<UserManager<ApplicationUser>>(
            new Mock<IUserStore<ApplicationUser>>().Object,
            null, null, null, null, null, null, null, null);

        roleManagerMock = new Mock<RoleManager<ApplicationRole>>(
            new Mock<IRoleStore<ApplicationRole>>().Object,
            null, null, null, null);

        envMock = new Mock<IWebHostEnvironment>();
        var loggerAccountMock = new Mock<ILogger<AccountController>>();
        emailServiceMock = new Mock<IEmailService>();
        configurationMock = new Mock<IConfiguration>();

        // Pass the mock ProductivityService to AccountService
        accountServiceMock = new Mock<AccountService>(
            dataContextMock.Object,
            new Mock<ILogger<AccountService>>().Object,
            mapperMock.Object,
            userManagerMock.Object,
            roleManagerMock.Object,
            productivityServiceMock.Object // Use the mock here
        );

        controller = new AccountController(
            envMock.Object,
            signInManagerMock.Object,
            userManagerMock.Object,
            roleManagerMock.Object,
            loggerAccountMock.Object,
            emailServiceMock.Object,
            new Mock<DataContextService>(dataContextMock.Object, new Mock<NavigationManager>().Object).Object,
            configurationMock.Object,
            accountServiceMock.Object
        );
    }

    [Fact]
    public async Task Login_InvalidUser_ReturnsRedirectWithError()
    {
        // Arrange
        var userName = "testuser";
        var password = "password";
        var redirectUrl = "~/";
        userManagerMock.Setup(um => um.FindByNameAsync(userName)).ReturnsAsync((ApplicationUser)null);

        // Act
        var result = await controller.Login(userName, password, redirectUrl);

        // Assert
        var redirectResult = Assert.IsType<RedirectResult>(result);
        Assert.Contains("error=Invalid user or password", redirectResult.Url);
    }

    [Fact]
    public async Task VerifySecurityCode_InvalidCode_ReturnsRedirectWithError()
    {
        // Arrange
        var code = "123456";
        signInManagerMock.Setup(sm => sm.TwoFactorSignInAsync("Email", code, false, false)).ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Failed);

        // Act
        var result = await controller.VerifySecurityCode(code, null);

        // Assert
        var redirectResult = Assert.IsType<RedirectResult>(result);
        Assert.Contains("error=Invalid security code", redirectResult.Url);
    }

    [Fact]
    public async Task ChangePassword_ValidPassword_ReturnsOk()
    {
        // Arrange
        var oldPassword = "oldpassword";
        var newPassword = "newpassword";
        var userId = "testuser";
        var user = new ApplicationUser { Id = userId };
        userManagerMock.Setup(um => um.FindByIdAsync(userId)).ReturnsAsync(user);
        userManagerMock.Setup(um => um.ChangePasswordAsync(user, oldPassword, newPassword)).ReturnsAsync(IdentityResult.Success);
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, userId)
                }))
            }
        };

        // Act
        var result = await controller.ChangePassword(oldPassword, newPassword);

        // Assert
        Assert.IsType<OkResult>(result);
    }

    [Fact]
    public async Task ChangePassword_InvalidPassword_ReturnsBadRequest()
    {
        // Arrange
        var oldPassword = "oldpassword";
        var newPassword = "newpassword";
        var userId = "testuser";
        var user = new ApplicationUser { Id = userId };
        userManagerMock.Setup(um => um.FindByIdAsync(userId)).ReturnsAsync(user);
        userManagerMock.Setup(um => um.ChangePasswordAsync(user, oldPassword, newPassword)).ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Error" }));
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, userId)
                }))
            }
        };

        // Act
        var result = await controller.ChangePassword(oldPassword, newPassword);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Error", badRequestResult.Value);
    }

    [Fact]
    public async Task Logout_ReturnsRedirect()
    {
        // Act
        var result = await controller.Logout();

        // Assert
        var redirectResult = Assert.IsType<RedirectResult>(result);
        Assert.Equal("~/", redirectResult.Url);
    }

    [Fact]
    public async Task ConfirmEmail_ValidCode_ReturnsRedirect()
    {
        // Arrange
        var userId = "testuser";
        var code = "code";
        var user = new ApplicationUser { Id = userId };
        userManagerMock.Setup(um => um.FindByIdAsync(userId)).ReturnsAsync(user);
        userManagerMock.Setup(um => um.ConfirmEmailAsync(user, code)).ReturnsAsync(IdentityResult.Success);

        // Act
        var result = await controller.ConfirmEmail(userId, code);

        // Assert
        var redirectResult = Assert.IsType<RedirectResult>(result);
        Assert.Contains("info=Your registration has been confirmed", redirectResult.Url);
    }

    [Fact]
    public async Task ConfirmEmail_InvalidCode_ReturnsRedirectWithError()
    {
        // Arrange
        var userId = "testuser";
        var code = "code";
        var user = new ApplicationUser { Id = userId };
        userManagerMock.Setup(um => um.FindByIdAsync(userId)).ReturnsAsync(user);
        userManagerMock.Setup(um => um.ConfirmEmailAsync(user, code)).ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Error" }));

        // Act
        var result = await controller.ConfirmEmail(userId, code);

        // Assert
        var redirectResult = Assert.IsType<RedirectResult>(result);
        Assert.Contains("error=Invalid user or confirmation code", redirectResult.Url);
    }

    [Fact]
    public async Task ResetPassword_InvalidUser_ReturnsBadRequest()
    {
        // Arrange
        var userName = "testuser";
        userManagerMock.Setup(um => um.FindByNameAsync(userName)).ReturnsAsync((ApplicationUser)null);

        // Act
        var result = await controller.ResetPassword(userName);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Invalid user name.", badRequestResult.Value);
    }

    [Fact]
    public async Task ConfirmPasswordReset_ValidCode_ReturnsRedirect()
    {
        // Arrange
        var userId = "testuser";
        var code = "code";
        var user = new ApplicationUser { Id = userId, Email = "testuser@example.com" };
        userManagerMock.Setup(um => um.FindByIdAsync(userId)).ReturnsAsync(user);
        userManagerMock.Setup(um => um.ResetPasswordAsync(user, code, It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);

        // Act
        var result = await controller.ConfirmPasswordReset(userId, code);

        // Assert
        var redirectResult = Assert.IsType<RedirectResult>(result);
        Assert.Contains("info=Password reset successful", redirectResult.Url);
    }

    [Fact]
    public async Task ConfirmPasswordReset_InvalidCode_ReturnsRedirectWithError()
    {
        // Arrange
        var userId = "testuser";
        var code = "code";
        var user = new ApplicationUser { Id = userId };
        userManagerMock.Setup(um => um.FindByIdAsync(userId)).ReturnsAsync(user);
        userManagerMock.Setup(um => um.ResetPasswordAsync(user, code, It.IsAny<string>())).ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Error" }));

        // Act
        var result = await controller.ConfirmPasswordReset(userId, code);

        // Assert
        var redirectResult = Assert.IsType<RedirectResult>(result);
        Assert.Contains("error=Invalid user or confirmation code", redirectResult.Url);
    }
}
