using System;
using API.Entities;
using API.Extentions;
using API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class AdminController(UserManager<AppUser> userManager, IUnitOfWork unitOfWork
        , IPhotoService photoService) : BaseApiController
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
                        Username = x.UserName,
                        Roles = x.UserRoles.Select(x => x.Role.Name).ToList()
                    }).ToListAsync();
        return Ok(users);
    }

    [Authorize(Policy = "RequiredAdminRole")]
    [HttpPost("edit-roles/{username}")]
    public async Task<ActionResult> EditRoles(string username, string roles)
    {
        if (string.IsNullOrEmpty(roles)) return BadRequest("you must select at least one role");

        var selectedRoles = roles.Split(",").ToArray();

        var user = await userManager.FindByNameAsync(username);

        if (user == null) return BadRequest("User not found");

        var userRoles = await userManager.GetRolesAsync(user);

        var result = await userManager.AddToRolesAsync(user, selectedRoles.Except(userRoles));

        if (!result.Succeeded) return BadRequest("Failed to add to roles");

        result = await userManager.RemoveFromRolesAsync(user, userRoles.Except(selectedRoles));

        if (!result.Succeeded) return BadRequest("Failed to remove from roles");

        return Ok(await userManager.GetRolesAsync(user));
    }


    [Authorize(Policy = "ModeratorPhotoRole")]
    [HttpGet("photos-to-moderate")]
    public async Task<ActionResult> GetPhotosForModerationAsync()
    {
        var photos = await unitOfWork.PhotoRepository.GetUnapprovedPhotos();
        return Ok(photos);
    }

    [Authorize(Policy = "ModeratorPhotoRole")]
    [HttpPost("approve-photo/{photoId}")]
    public async Task<ActionResult> ApprovePhoto(int photoId)
    {
        var photo = await unitOfWork.PhotoRepository.GetPhotoById(photoId);
        if (photo == null) return BadRequest("Could not get photo from db");
        photo.IsApproved = true;
        var user = await unitOfWork.UserRepository.GetUserByPhotoId(photoId);
        if (user == null) return BadRequest("Could not get user from db");
        if (!user.Photos.Any(x => x.IsMain)) photo.IsMain = true;
        await unitOfWork.Complete();
        return Ok();
    }

    [Authorize(Policy = "ModeratorPhotoRole")]
    [HttpPost("reject-photo/{photoId}")]
    public async Task<ActionResult> RejectPhoto(int photoId)
    {
        var photo = await unitOfWork.PhotoRepository.GetPhotoById(photoId);
        if (photo == null) return BadRequest("Could not get photo from db");
        if (photo.PublicId != null)
        {
            var result = await photoService.DeletePhotoAsync(photo.PublicId);
            if (result.Result == "ok")
            {
                unitOfWork.PhotoRepository.RemovePhoto(photo);
            }
        }
        else
        {
            unitOfWork.PhotoRepository.RemovePhoto(photo);
        }
        await unitOfWork.Complete();
        return Ok();
    }

    [HttpDelete("delete-photo/{photoId:int}")]
    public async Task<ActionResult> DeletePhoto(int photoId)
    {
        var user = await
        unitOfWork.UserRepository.GetUserByUsernameAsync(User.GetUserName());
        if (user == null) return BadRequest("User not found");
        var photo = await unitOfWork.PhotoRepository.GetPhotoById(photoId);
        if (photo == null || photo.IsMain) return BadRequest("This photo cannot be deleted");
        if (photo.PublicId != null)
        {
            var result = await photoService.DeletePhotoAsync(photo.PublicId);
            if (result.Error != null) return BadRequest(result.Error.Message);
        }
        user.Photos.Remove(photo);
        if (await unitOfWork.Complete()) return Ok();
        return BadRequest("Problem deleting photo");
    }
}
