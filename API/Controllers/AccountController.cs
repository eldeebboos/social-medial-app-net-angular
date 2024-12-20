
using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{

    public class AccountController(
            UserManager<AppUser> userManager,
            ITokenService tokenService,
            IMapper mapper
            ) : BaseApiController
    {
        //account/regsiter
        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
        {
            if (await UserExist(registerDto.UserName))
                return BadRequest("Username already exists");

            var user = mapper.Map<AppUser>(registerDto);
            user.UserName = registerDto.UserName.ToLower();

            var result = await userManager.CreateAsync(user, registerDto.Password);

            return new UserDto()
            {
                Username = registerDto.UserName,
                Token = await tokenService.CreateToken(user),
                KnownAs = user.KnownAs,
                Gender = user.Gender
            };
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            var user = await userManager.Users
                .Include(u => u.Photos)
                .FirstOrDefaultAsync(x =>
                    x.NormalizedUserName == loginDto.UserName.ToUpper());

            if (user == null || user.UserName == null)
                return Unauthorized("invalid username");

            return new UserDto
            {
                Username = user.UserName,
                KnownAs = user.KnownAs,
                Token = await tokenService.CreateToken(user),
                Gender = user.Gender,
                PhotoUrl = user.Photos.FirstOrDefault(p => p.IsMain)?.Url
            };
        }

        private async Task<bool> UserExist(string username)
        {
            return await userManager.Users
            .AnyAsync(x => x.NormalizedUserName == username.ToUpper());
        }
    }
}
