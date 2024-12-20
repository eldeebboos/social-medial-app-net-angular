using System;
using API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class AdminController(UserManager<AppUser> userManager) : BaseApiController
{
    [Authorize(Policy = "RequiredAdminRole")]
    [HttpGet("users-with-roles")]
    public async Task<ActionResult> GetUsersWithRoles()
    {
        var users = await userManager.Users
                    .OrderBy(x => x.UserName)
                    .Select(x => new
                    {
                        x.Id,
                        Usename = x.UserName,
                        Roles = x.UserRoles.Select(x => x.Role.Name).ToList()
                    }).ToListAsync();
        return Ok(users);
    }

    [Authorize(Policy = "RequiredAdminRole")]
    [HttpPost("edit-roles/{username}")]
    public async Task<ActionResult> EditRoles(string username, string roles)
    {
        if (string.IsNullOrEmpty(roles)) return BadRequest("You must select at least one role.");

        var selectedRoles = roles.Split(",").ToArray();

        var user = await userManager.FindByNameAsync(username);
        if (user == null) return BadRequest("User not found");

        var userRoles = await userManager.GetRolesAsync(user);

        var results = await userManager.AddToRolesAsync(user, selectedRoles.Except(userRoles));

        if (!results.Succeeded) return BadRequest("Failed to add to roles");

        results = await userManager.RemoveFromRolesAsync(user, selectedRoles.Except(selectedRoles));

        if (!results.Succeeded) return BadRequest("Failed to remove to roles");
        return Ok(await userManager.GetRolesAsync(user));

    }


    [Authorize(Policy = "ModeratorPhotoRole")]
    [HttpGet("photos-to-moderate")]
    public async Task<ActionResult> GetPhotosForModeration()
    {
        return Ok("Only Admins or Moderators can see this.");
    }
}
