using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using API.DTOs;
using Microsoft.EntityFrameworkCore;
using API.Services;

namespace API.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly DataContext _context;
        private readonly ITokenService _tokenService;

        public AccountController(DataContext context, ITokenService tokenService)
        {
            this._context = context;
            this._tokenService = tokenService;
        }

        [HttpPost("Register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
        {
            if (await UserAlreadyExist(registerDto.UserName)) return BadRequest("User name has already been taken");

            using var mac = new HMACSHA512();

            AppUser user = new AppUser
            {
                UserName = registerDto.UserName.ToLower(), //gelen ismi her zaman küçük kaydediyoruz, casesensetive olmasın diye
                PasswordHash = mac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
                PasswordSalt = mac.Key
            };

            EntityEntry<AppUser> returnedUser = await _context.AppUsers.AddAsync(user);

            await _context.SaveChangesAsync();

            return new UserDto
            {
                UserName = returnedUser.Entity.UserName,
                Token = _tokenService.CreateToken(returnedUser.Entity)
            };
        }


        [HttpPost("Login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {

            AppUser user = await _context.AppUsers.FirstOrDefaultAsync(x => x.UserName == loginDto.UserName.ToLower());

            if (user == null)
                Unauthorized("invalid username");

            using var mac = new HMACSHA512(user.PasswordSalt);
            var computedHash = mac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

            for (int i = 0; i < user.PasswordHash.Length; i++)
            {
                if (user.PasswordHash[i] != computedHash[i])
                    return Unauthorized("invalid password");
            }

            //Everythings is ok, now generate a JWT and return 

            return new UserDto
            {
                UserName = user.UserName,
                Token = _tokenService.CreateToken(user)
            };
        }

        private async Task<bool> UserAlreadyExist(string userName)
        {
            return await _context.AppUsers.AnyAsync(x => x.UserName == userName.ToLower()); //tolower olarak aratmayı unutmuyoruz.
        }

    }
}