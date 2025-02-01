using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyVideoResume.Data.Models;
using MyVideoResume.Application;
using MyVideoResume.Services;
using DocumentFormat.OpenXml.EMMA;
using DocumentFormat.OpenXml.Spreadsheet;
using MyVideoResume.Data.Models.Account;
using MyVideoResume.Application.Account;
using MyVideoResume.Abstractions.Core;

namespace MyVideoResume.Server.Controllers;

public enum ErrorCodes
{
    InValidUser = 0,
    AccountUnconfirmed = 1,
    InValidSecurityCode = 3
}

[Route("Account/[action]")]
public partial class AccountController : Controller
{
    private readonly SignInManager<ApplicationUser> signInManager;
    private readonly UserManager<ApplicationUser> userManager;
    private readonly RoleManager<ApplicationRole> roleManager;
    private readonly IWebHostEnvironment env;
    private readonly ILogger<AccountController> logger;
    private readonly EmailService emailService;
    private readonly IConfiguration configuration;
    private readonly AccountService accountService;

    public AccountController(IWebHostEnvironment env, SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager, ILogger<AccountController> logger, EmailService emailService, DataContextService dataContext, IConfiguration configuration, AccountService accountService)
    {
        this.signInManager = signInManager;
        this.userManager = userManager;
        this.roleManager = roleManager;
        this.env = env;
        this.logger = logger;
        this.emailService = emailService;
        this.configuration = configuration;
        this.accountService = accountService;
    }

    #region Security
    private IActionResult RedirectWithError(string error, ErrorCodes errorCode, string redirectUrl = null)
    {
        if (!string.IsNullOrEmpty(redirectUrl))
        {
            return Redirect($"~/Login?error={error}&ec={errorCode}&redirectUrl={Uri.EscapeDataString(redirectUrl.Replace("~", ""))}");
        }
        else
        {
            return Redirect($"~/Login?error={error}&ec={errorCode}");
        }
    }

    [HttpGet]
    public async Task<IActionResult> Login(string returnUrl)
    {
        if (returnUrl != "/" && !string.IsNullOrEmpty(returnUrl))
        {
            return Redirect($"~/Login?redirectUrl={Uri.EscapeDataString(returnUrl)}");
        }

        return Redirect("~/Login");
    }

    [HttpPost]
    public async Task<IActionResult> Login(string userName, string password, string redirectUrl)
    {
        redirectUrl = string.IsNullOrEmpty(redirectUrl) ? "~/" : redirectUrl.StartsWith("/") ? redirectUrl : $"~/{redirectUrl}";

        if (env.EnvironmentName == "Development" && userName == "admin" && password == "admin")
        {
            logger.LogDebug("Admin Login");

            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, "Admin"),
                new Claim(ClaimTypes.Email, "Admin")
            };

            roleManager.Roles.ToList().ForEach(r => claims.Add(new Claim(ClaimTypes.Role, r.Name)));
            await signInManager.SignInWithClaimsAsync(new ApplicationUser { UserName = userName, Email = userName }, isPersistent: false, claims);

            return Redirect(redirectUrl);
        }

        if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(password))
        {

            var user = await userManager.FindByNameAsync(userName);

            if (user == null)
            {
                return RedirectWithError("Invalid user or password", ErrorCodes.InValidUser, redirectUrl);
            }

            if (!user.EmailConfirmed)
            {
                await SendConfirmationEmail(user);
                return RedirectWithError("User email not confirmed", ErrorCodes.AccountUnconfirmed, redirectUrl);
            }

            var isTenantsAdmin = userName == "tenantsadmin";
            var isTwoFactor = await userManager.GetTwoFactorEnabledAsync(user);
            if (!isTwoFactor && !isTenantsAdmin)
            {
                await userManager.SetTwoFactorEnabledAsync(user, true);
            }
            var result = await signInManager.PasswordSignInAsync(userName, password, false, false);


            if (result.RequiresTwoFactor && !isTenantsAdmin)
            {
                var code = await userManager.GenerateTwoFactorTokenAsync(user, "Email");

                var mfaEnabled = configuration.GetValue<bool>("Security:IsMFAEnabled");
                if (mfaEnabled)
                {
                    var text = $@"Hi, <br /> <br />
We received your request for a single-use code to use with your MyVideoResu.ME account. <br /> <br />
Your single-use code is: {code} <br /> <br />
If you didn't request this code, you can safely ignore this email. Someone else might have typed your email address by mistake.";

                    await emailService.SendEmailAsync(user.Email, "Your single-use code", text);

                    return Redirect($"~/SecurityCode?email={Uri.EscapeDataString(user.Email)}&redirectUrl={redirectUrl}");
                }
                else
                {
                    result = await signInManager.TwoFactorSignInAsync("Email", code, false, false);
                }
            }

            if (result.Succeeded)
            {
                return Redirect(redirectUrl);
            }
        }

        return RedirectWithError("Invalid user or password", ErrorCodes.InValidUser, redirectUrl);
    }

    [HttpPost]
    public async Task<IActionResult> VerifySecurityCode(string code, string redirectUrl)
    {
        var result = await signInManager.TwoFactorSignInAsync("Email", code, false, false);

        if (!result.Succeeded)
        {
            return RedirectWithError("Invalid security code", ErrorCodes.InValidSecurityCode);
        }
        else
        {
            //Why do this here?
            //Lots of users (bots) can create accounts; When a user get's to this point they have a valid token and thus a real user.
            //Verify they have a User Profile
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            await accountService.CreateAccount(userId);
        }

        if (!string.IsNullOrWhiteSpace(redirectUrl))
            return Redirect(redirectUrl);

        return Redirect("~/");
    }
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> ChangePassword(string oldPassword, string newPassword)
    {
        if (string.IsNullOrEmpty(oldPassword) || string.IsNullOrEmpty(newPassword))
        {
            return BadRequest("Invalid password");
        }

        var id = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var user = await userManager.FindByIdAsync(id);
        var result = await userManager.ChangePasswordAsync(user, oldPassword, newPassword);

        if (result.Succeeded)
        {
            return Ok();
        }

        var message = string.Join(", ", result.Errors.Select(error => error.Description));

        return BadRequest(message);
    }

    [HttpPost]
    public async Task<ApplicationAuthenticationState> CurrentUser()
    {
        var id = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var user = await userManager.FindByIdAsync(id);
        var roles = string.Empty;
        if (user != null)
            if (user.Roles != null)
                roles = string.Join(",", user.Roles);

        return new ApplicationAuthenticationState
        {
            IsAuthenticated = User.Identity.IsAuthenticated,
            Name = User.Identity.Name,
            Claims = User.Claims.Select(c => new ApplicationClaim { Type = c.Type, Value = c.Value }),
            Roles = roles
        };
    }

    public async Task<IActionResult> Logout()
    {
        await signInManager.SignOutAsync();

        return Redirect("~/");
    }

    [HttpPost]
    public async Task<IActionResult> Register(string userName, string password)
    {
        if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
        {
            return BadRequest("Invalid user name or password.");
        }

        var roles = roleManager.Roles.ToDictionary(z => z.Name, y => y);
        var roleList = new List<ApplicationRole>
        {
            roles[Enum.GetName(MyVideoResumeRoles.AccountAdmin)],
            roles[Enum.GetName(MyVideoResumeRoles.AccountOwner)]
        };

        var user = new ApplicationUser { UserName = userName, Email = userName, Roles = roleList };
        var result = await userManager.CreateAsync(user, password);

        if (result.Succeeded)
        {
            try
            {
                await SendConfirmationEmail(user);

                return Ok();
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message, ex);
                return BadRequest(ex.Message);
            }
        }

        var message = string.Join(", ", result.Errors.Select(error => error.Description));

        return BadRequest(message);
    }

    private async Task SendConfirmationEmail(ApplicationUser user)
    {
        var code = await userManager.GenerateEmailConfirmationTokenAsync(user);

        var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code }, protocol: Request.Scheme);

        var text = $@"Hi, <br /> <br />
We received your registration request for MyVideoResume. <br /> <br />
To confirm your registration please click the following link: <a href=""{callbackUrl}"">confirm your registration</a> <br /> <br />
If you didn't request this registration, you can safely ignore this email. Someone else might have typed your email address by mistake.";

        await emailService.SendEmailAsync(user.Email, "Confirm your registration", text);
    }

    public async Task<IActionResult> ConfirmEmail(string userId, string code)
    {
        var user = await userManager.FindByIdAsync(userId);

        var result = await userManager.ConfirmEmailAsync(user, code);

        if (result.Succeeded)
        {
            return Redirect("~/Login?info=Your registration has been confirmed");
        }

        return RedirectWithError("Invalid user or confirmation code", ErrorCodes.InValidUser);
    }

    public async Task<IActionResult> ResetPassword(string userName)
    {
        var user = await userManager.FindByNameAsync(userName);

        if (user == null)
        {
            return BadRequest("Invalid user name.");
        }

        try
        {
            var code = await userManager.GeneratePasswordResetTokenAsync(user);

            var callbackUrl = Url.Action("ConfirmPasswordReset", "Account", new { userId = user.Id, code }, protocol: Request.Scheme);

            var body = string.Format(@"<a href=""{0}"">{1}</a>", callbackUrl, "Please confirm your password reset.");

            await emailService.SendEmailAsync(user.Email, "Confirm your password reset", body);

            return Ok();
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message, ex);
            return BadRequest(ex.Message);
        }
    }

    public async Task<IActionResult> ConfirmPasswordReset(string userId, string code)
    {
        var user = await userManager.FindByIdAsync(userId);

        if (user == null)
        {
            return Redirect("~/Login?error=Invalid user");
        }

        var password = GenerateRandomPassword();

        var result = await userManager.ResetPasswordAsync(user, code, password);

        if (result.Succeeded)
        {
            await emailService.SendEmailAsync(user.Email, "New password", $"<p>Your new password is: {password}</p><p>Please change it after login.</p>");

            return Redirect("~/Login?info=Password reset successful. You will receive an email with your new password.");
        }

        return Redirect("~/Login?error=Invalid user or confirmation code");
    }

    private static string GenerateRandomPassword()
    {
        var options = new PasswordOptions
        {
            RequiredLength = 8,
            RequiredUniqueChars = 4,
            RequireDigit = true,
            RequireLowercase = true,
            RequireNonAlphanumeric = true,
            RequireUppercase = true
        };

        var randomChars = new[] {
            "ABCDEFGHJKLMNOPQRSTUVWXYZ",
            "abcdefghijkmnopqrstuvwxyz",
            "0123456789",
            "!@$?_-"
        };

        var rand = new Random(Environment.TickCount);
        var chars = new List<char>();

        if (options.RequireUppercase)
        {
            chars.Insert(rand.Next(0, chars.Count), randomChars[0][rand.Next(0, randomChars[0].Length)]);
        }

        if (options.RequireLowercase)
        {
            chars.Insert(rand.Next(0, chars.Count), randomChars[1][rand.Next(0, randomChars[1].Length)]);
        }

        if (options.RequireDigit)
        {
            chars.Insert(rand.Next(0, chars.Count), randomChars[2][rand.Next(0, randomChars[2].Length)]);
        }

        if (options.RequireNonAlphanumeric)
        {
            chars.Insert(rand.Next(0, chars.Count), randomChars[3][rand.Next(0, randomChars[3].Length)]);
        }

        for (int i = chars.Count; i < options.RequiredLength || chars.Distinct().Count() < options.RequiredUniqueChars; i++)
        {
            string rcs = randomChars[rand.Next(0, randomChars.Length)];
            chars.Insert(rand.Next(0, chars.Count), rcs[rand.Next(0, rcs.Length)]);
        }

        return new string(chars.ToArray());
    }


    #endregion

}
