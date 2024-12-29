using System.Security.Claims;
using API.DTOs;
using API.Entities;
using API.Extentions;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Authorize]
public class UsersController(IUnitOfWork unitOfWork, IMapper mapper, IPhotoService photoService) : BaseApiController
{

    // [Authorize(Roles = "Admin")]
    // [AllowAnonymous]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers([FromQuery] UserParams userParams)
    {
        userParams.CurrentUsername = User.GetUserName();
        var users = await unitOfWork.UserRepository.GetMembersAsync(userParams);

        Response.AddPageinationHeader(users);

        return Ok(users);
    }

    // [Authorize]
    [HttpGet("{id:int}")]
    public async Task<ActionResult<MemberDto>> GetUser(int id)
    {
        var user = await unitOfWork.UserRepository.GetMemberByIdAsync(id);

        if (user == null)
            return NotFound();

        return Ok(user);

    }

    // [Authorize(Roles = "Member")]
    [HttpGet("{username}")]
    public async Task<ActionResult<MemberDto>> GetUser(string username)
    {
        var currentUsername = User.GetUserName();

        var user = await unitOfWork.UserRepository.GetMemberAsync(username,
                       isCurrentUser: currentUsername == username
                       );

        if (user == null)
            return NotFound();

        return Ok(user);
    }

    [HttpPut]
    public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto)
    {
        //User.GetUserName() is a custom extintion method
        var user = await unitOfWork.UserRepository.GetUserByUsernameAsync(User.GetUserName());
        if (user == null) return BadRequest("Coundlnt's find user");

        mapper.Map(memberUpdateDto, user);

        if (await unitOfWork.Complete()) return NoContent();

        return BadRequest("Failed to update user");

    }


    [HttpPost("add-photo")]
    public async Task<ActionResult<PhotoDto>> AddPhoto(IFormFile file)
    {
        var user = await unitOfWork.UserRepository.GetUserByUsernameAsync(User.GetUserName());

        if (user == null) return BadRequest("Coundlnt's find user");

        var result = await photoService.addPhotoAsync(file);
        if (result.Error != null) return BadRequest(result.Error.Message);

        var photo = new Photo()
        {
            Url = result.SecureUrl.AbsoluteUri,
            PublicId = result.PublicId,
        };

        user.Photos.Add(photo);

        if (await unitOfWork.Complete())
            // return mapper.Map<PhotoDto>(photo);
            //return an action as result
            return CreatedAtAction(nameof(GetUser),
                new { username = user.UserName },
                mapper.Map<PhotoDto>(photo));

        return BadRequest("Failed to add photo");

    }

    [HttpPut("set-main-photo/{photoId:int}")]
    public async Task<ActionResult> SetMainPhoto(int photoId)
    {
        var user = await unitOfWork.UserRepository.GetUserByUsernameAsync(User.GetUserName());

        if (user == null) return BadRequest("Coundlnt's find user");

        var photo = user.Photos.FirstOrDefault(p => p.Id == photoId);

        if (photo == null || photo.IsMain) return BadRequest("Cannot use this as main photo");

        var currentMainPhoto = user.Photos.FirstOrDefault(p => p.IsMain);
        if (currentMainPhoto != null) currentMainPhoto.IsMain = false;

        photo.IsMain = true;

        if (await unitOfWork.Complete()) return NoContent();

        return BadRequest("Proplem with Setting main photo");
    }

    [HttpDelete("delete-photo/{photoId:int}")]
    public async Task<ActionResult> DeletePhoto(int photoId)
    {
        var user = await unitOfWork.UserRepository.GetUserByUsernameAsync(User.GetUserName());

        if (user == null) return BadRequest("Coundlnt's find user");

        var photo = user.Photos.FirstOrDefault(p => p.Id == photoId);

        if (photo == null || photo.IsMain) return BadRequest("This Photo cannot be deleted");

        if (photo.PublicId != null)
        {
            var result = await photoService.DeletePhotoAsync(photo.PublicId);
            if (result.Error != null) return BadRequest(result.Error.Message);
        }

        user.Photos.Remove(photo);

        if (await unitOfWork.Complete()) return Ok();

        return BadRequest("Proplem deleteing photo");

    }

}
