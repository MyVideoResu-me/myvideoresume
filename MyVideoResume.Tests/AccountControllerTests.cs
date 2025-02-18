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
using MyVideoResume.Application.Business;
using MyVideoResume.Data;
using MyVideoResume.Data.Models;
using MyVideoResume.Data.Models.Account;
using MyVideoResume.Services;
using Xunit;

namespace MyVideoResume.Server.Controllers.Tests
{
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
            loggerMock = new Mock<ILogger<AccountController>>();
            emailServiceMock = new Mock<IEmailService>(new Mock<IConfiguration>().Object, new Mock<ILogger<IEmailService>>().Object);
            configurationMock = new Mock<IConfiguration>();
            accountServiceMock = new Mock<AccountService>(new Mock<DataContext>().Object, new Mock<ILogger<AccountService>>().Object, new Mock<IMapper>().Object, userManagerMock.Object, roleManagerMock.Object, new Mock<TaskService>(new Mock<DataContext>().Object, new Mock<ILogger<AccountService>>().Object, new Mock<IMapper>().Object).Object);

            controller = new AccountController(envMock.Object, signInManagerMock.Object, userManagerMock.Object, roleManagerMock.Object, loggerMock.Object, emailServiceMock.Object, new Mock<DataContextService>(new Mock<DataContext>().Object, new Mock<NavigationManager>().Object).Object, configurationMock.Object, accountServiceMock.Object);
        }

        //[Fact]
        //public async Task GenerateToken_ValidUser_ReturnsToken()
        //{
        //    // Arrange
        //    var userName = "testuser";
        //    var password = "password";
        //    var user = new ApplicationUser { UserName = userName };
        //    userManagerMock.Setup(um => um.FindByNameAsync(userName)).ReturnsAsync(user);
        //    userManagerMock.Setup(um => um.CheckPasswordAsync(user, password)).ReturnsAsync(true);
        //    configurationMock.Setup(c => c["Jwt:Key"]).Returns("supersecretkey");
        //    configurationMock.Setup(c => c["Jwt:Issuer"]).Returns("issuer");
        //    configurationMock.Setup(c => c["Jwt:Audience"]).Returns("audience");

        //    // Act
        //    var result = await controller.GenerateToken(userName, password);

        //    // Assert
        //    var okResult = Assert.IsType<OkObjectResult>(result);
        //    Assert.NotNull(okResult.Value);
        //}

        //[Fact]
        //public async Task Login_ValidUser_ReturnsRedirect()
        //{
        //    // Arrange
        //    var userName = "testuser";
        //    var password = "password";
        //    var redirectUrl = "~/";
        //    var user = new ApplicationUser { UserName = userName, Email = userName, EmailConfirmed = true };
        //    userManagerMock.Setup(um => um.FindByNameAsync(userName)).ReturnsAsync(user);
        //    userManagerMock.Setup(um => um.GetTwoFactorEnabledAsync(user)).ReturnsAsync(false);
        //    signInManagerMock.Setup(sm => sm.PasswordSignInAsync(userName, password, false, false)).ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success);

        //    // Act
        //    var result = await controller.Login(userName, password, redirectUrl);

        //    // Assert
        //    var redirectResult = Assert.IsType<RedirectResult>(result);
        //    Assert.Equal(redirectUrl, redirectResult.Url);
        //}

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

        //[Fact]
        //public async Task VerifySecurityCode_ValidCode_ReturnsRedirect()
        //{
        //    // Arrange
        //    var code = "123456";
        //    var redirectUrl = "~/";
        //    signInManagerMock.Setup(sm => sm.TwoFactorSignInAsync("Email", code, false, false)).ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success);
        //    var userId = "testuser";
        //    var user = new ApplicationUser { Id = userId };
        //    userManagerMock.Setup(um => um.FindByIdAsync(userId)).ReturnsAsync(user);
        //    controller.ControllerContext = new ControllerContext
        //    {
        //        HttpContext = new DefaultHttpContext
        //        {
        //            User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        //            {
        //                new Claim(ClaimTypes.NameIdentifier, userId)
        //            }))
        //        }
        //    };

        //    // Act
        //    var result = await controller.VerifySecurityCode(code, redirectUrl);

        //    // Assert
        //    var redirectResult = Assert.IsType<RedirectResult>(result);
        //    Assert.Equal(redirectUrl, redirectResult.Url);
        //}

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

        //[Fact]
        //public async Task CurrentUser_ReturnsAuthenticationState()
        //{
        //    // Arrange
        //    var userId = "testuser";
        //    var user = new ApplicationUser { Id = userId };
        //    userManagerMock.Setup(um => um.FindByIdAsync(userId)).ReturnsAsync(user);
        //    controller.ControllerContext = new ControllerContext
        //    {
        //        HttpContext = new DefaultHttpContext
        //        {
        //            User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        //            {
        //                new Claim(ClaimTypes.NameIdentifier, userId)
        //            }))
        //        }
        //    };

        //    // Act
        //    var result = await controller.CurrentUser();

        //    // Assert
        //    Assert.NotNull(result);
        //    Assert.Equal(userId, result.Name);
        //}

        [Fact]
        public async Task Logout_ReturnsRedirect()
        {
            // Act
            var result = await controller.Logout();

            // Assert
            var redirectResult = Assert.IsType<RedirectResult>(result);
            Assert.Equal("~/", redirectResult.Url);
        }

        //[Fact]
        //public async Task Register_ValidUser_ReturnsOk()
        //{
        //    // Arrange
        //    var userName = "testuser";
        //    var password = "password";
        //    var user = new ApplicationUser { UserName = userName, Email = userName };
        //    userManagerMock.Setup(um => um.CreateAsync(user, password)).ReturnsAsync(IdentityResult.Success);
        //    roleManagerMock.Setup(rm => rm.Roles).Returns(new List<ApplicationRole> { new ApplicationRole { Name = "AccountAdmin" }, new ApplicationRole { Name = "AccountOwner" } }.AsQueryable());

        //    // Act
        //    var result = await controller.Register(userName, password);

        //    // Assert
        //    Assert.IsType<OkResult>(result);
        //}

        //[Fact]
        //public async Task Register_InvalidUser_ReturnsBadRequest()
        //{
        //    // Arrange
        //    var userName = "testuser";
        //    var password = "password";
        //    var user = new ApplicationUser { UserName = userName, Email = userName };
        //    userManagerMock.Setup(um => um.CreateAsync(user, password)).ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Error" }));
        //    roleManagerMock.Setup(rm => rm.Roles).Returns(new List<ApplicationRole> { new ApplicationRole { Name = "AccountAdmin" }, new ApplicationRole { Name = "AccountOwner" } }.AsQueryable());

        //    // Act
        //    var result = await controller.Register(userName, password);

        //    // Assert
        //    var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        //    Assert.Equal("Error", badRequestResult.Value);
        //}

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

        //[Fact]
        //public async Task ResetPassword_ValidUser_ReturnsOk()
        //{
        //    // Arrange
        //    var userName = "testuser";
        //    var user = new ApplicationUser { UserName = userName, Email = userName };
        //    userManagerMock.Setup(um => um.FindByNameAsync(userName)).ReturnsAsync(user);
        //    userManagerMock.Setup(um => um.GeneratePasswordResetTokenAsync(user)).ReturnsAsync("code");

        //    // Act
        //    var result = await controller.ResetPassword(userName);

        //    // Assert
        //    Assert.IsType<OkResult>(result);
        //}

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
}
