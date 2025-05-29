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
using MyVideoResume.Data.Models.Account;

namespace MyVideoResume.Server.Controllers;

[Authorize]
[ODataRouteComponent("odata/Identity")]
public partial class ApplicationRolesController : ODataController
{
    private readonly RoleManager<ApplicationRole> roleManager;

    public ApplicationRolesController(RoleManager<ApplicationRole> roleManager)
    {
        this.roleManager = roleManager;
    }

    partial void OnRolesRead(ref IQueryable<ApplicationRole> roles);

    [EnableQuery(PageSize = 10)]
    public IQueryable<ApplicationRole> Get()
    {
        var roles = roleManager.Roles;
        OnRolesRead(ref roles);
        return roles;
    }

    [EnableQuery]
    public SingleResult<ApplicationRole> Get([FromRoute] string key)
    {
        var result = roleManager.Roles.Where(r => r.Id == key);
        return SingleResult.Create(result);
    }

    partial void OnRoleCreated(ApplicationRole role);

    public async Task<IActionResult> Post([FromBody] ApplicationRole role)
    {
        if (role == null)
        {
            return BadRequest();
        }

        OnRoleCreated(role);

        var result = await roleManager.CreateAsync(role);

        if (!result.Succeeded)
        {
            var message = string.Join(", ", result.Errors.Select(error => error.Description));
            return BadRequest(new { error = new { message }});
        }

        return Created(role);
    }

    partial void OnRoleDeleted(ApplicationRole role);

    public async Task<IActionResult> Delete([FromRoute] string key)
    {
        var role = await roleManager.FindByIdAsync(key);

        if (role == null)
        {
            return NotFound();
        }

        OnRoleDeleted(role);

        var result = await roleManager.DeleteAsync(role);

        if (!result.Succeeded)
        {
            var message = string.Join(", ", result.Errors.Select(error => error.Description));
            return BadRequest(new { error = new { message }});
        }

        return NoContent();
    }

    public async Task<IActionResult> Patch([FromRoute] string key, [FromBody] Delta<ApplicationRole> patch)
    {
        var role = await roleManager.FindByIdAsync(key);

        if (role == null)
        {
            return NotFound();
        }

        patch.Patch(role);

        var result = await roleManager.UpdateAsync(role);

        if (!result.Succeeded)
        {
            var message = string.Join(", ", result.Errors.Select(error => error.Description));
            return BadRequest(new { error = new { message }});
        }

        return Updated(role);
    }
}