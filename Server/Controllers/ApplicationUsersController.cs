using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.AspNetCore.OData.Results;
using Microsoft.AspNetCore.OData.Routing.Attributes;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.Hosting;
using System.ComponentModel.DataAnnotations.Schema;

using MyVideoResume.Server.Data;
using MyVideoResume.Data.Models;
using DocumentFormat.OpenXml.Spreadsheet;

namespace MyVideoResume.Server.Controllers;

[Authorize]
[ODataRouteComponent("odata/Identity")]
public partial class ApplicationUsersController : ODataController
{
    private readonly ApplicationIdentityDbContext context;
    private readonly UserManager<ApplicationUser> userManager;
    private readonly ILogger<ApplicationUsersController> logger;

    public ApplicationUsersController(ApplicationIdentityDbContext context, UserManager<ApplicationUser> userManager, ILogger<ApplicationUsersController> logger)
    {
        this.context = context;
        this.userManager = userManager;
        this.logger = logger;
    }

    partial void OnUsersRead(ref IQueryable<ApplicationUser> users);

    [EnableQuery(PageSize = 10)]
    public IQueryable<ApplicationUser> Get()
    {
        var users = userManager.Users;
        OnUsersRead(ref users);
        return users;
    }

    [EnableQuery]
    public SingleResult<ApplicationUser> Get([FromRoute] string key)
    {
        try
        {
            var user = context.Users.Where(i => i.Id == key);
            return SingleResult.Create(user);
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message, ex);
            return null;
        }
    }

    partial void OnUserDeleted(ApplicationUser user);

    public async Task<IActionResult> Delete([FromRoute] string key)
    {
        var user = await userManager.FindByIdAsync(key);

        if (user == null)
        {
            return NotFound();
        }

        OnUserDeleted(user);

        var result = await userManager.DeleteAsync(user);

        if (!result.Succeeded)
        {
            return IdentityError(result);
        }

        return NoContent();
    }

    partial void OnUserUpdated(ApplicationUser user);

    public async Task<IActionResult> Patch([FromRoute] string key, [FromBody] Delta<ApplicationUser> patch)
    {
        var user = await userManager.FindByIdAsync(key);

        if (user == null)
        {
            return NotFound();
        }

        patch.Patch(user);
        OnUserUpdated(user);

        var result = await userManager.UpdateAsync(user);

        if (!result.Succeeded)
        {
            return IdentityError(result);
        }

        return Updated(user);
    }

    partial void OnUserCreated(ApplicationUser user);

    public async Task<IActionResult> Post([FromBody] ApplicationUser user)
    {
        user.UserName = user.Email;
        user.EmailConfirmed = true;
        var password = user.Password;
        var roles = user.Roles;
        user.Roles = null;
        IdentityResult result = await userManager.CreateAsync(user, password);

        if (result.Succeeded && roles != null)
        {
            result = await userManager.AddToRolesAsync(user, roles.Select(r => r.Name));
        }

        user.Roles = roles;

        if (result.Succeeded)
        {
            OnUserCreated(user);
            return Created(user);
        }
        else
        {
            return IdentityError(result);
        }
    }

    private IActionResult IdentityError(IdentityResult result)
    {
        var message = string.Join(", ", result.Errors.Select(error => error.Description));
        return BadRequest(new { error = new { message } });
    }
}