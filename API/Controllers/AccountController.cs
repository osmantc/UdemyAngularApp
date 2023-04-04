using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace API.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly DataContext _context;
        private readonly IConfiguration _configuration;

        public AccountController(DataContext context, IConfiguration configuration)
        {
            this._context = context;
            this._configuration = configuration;

        }

        [HttpPost("Register")]
        public async Task<ActionResult<AppUser>> Register(AppUser user)
        {
            if (UserAlreadyExist(user)) return BadRequest("User name has already been taken");

            using var mac = new HMACSHA512();

            AppUser appUser = new AppUser
            {
                UserName = user.UserName,
                PasswordHash = mac.ComputeHash(Encoding.UTF8.GetBytes(user.Password)),
                PasswordSalt = mac.Key
            };

            await _context.AppUsers.AddAsync(appUser);
            await _context.SaveChangesAsync();

            return Ok(appUser);
        }

        [HttpPost("Login")]
        public async Task<ActionResult<AppUser>> Login(AppUser user)
        {
            if (!UserAlreadyExist(user)) return BadRequest("Invalid UserName");

            var returnedUser = _context.AppUsers.FirstOrDefault(u => u.UserName == user.UserName);

            using var mac = new HMACSHA512(user.PasswordSalt);

            var hashedPass = mac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(user.Password));

            for (var i = 0; i < hashedPass.Length; i++)
            {
                if (hashedPass[i] == user.PasswordHash[i])
                    continue;
                else
                {
                    return BadRequest("Invalid Password");
                    break;
                }
            }

            return Ok(user);

        }

        private bool UserAlreadyExist(AppUser user)
        {
            return _context.AppUsers.Any(x => x.UserName == user.UserName);
        }

        // private string WriteToken(AppUser user)
        // {
        //     List<Claim> claims = new List<Claim>(){
        //             new Claim(JwtRegisteredClaimNames.Sub,user.Id.ToString()),
        //             new Claim(JwtRegisteredClaimNames.Name,user.UserName),
        //         };

        //     SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["AuthKey"]));
        //     SigningCredentials credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
        //     var expires = DateTime.Now.AddMinutes(30);

        //     var token = new JwtSecurityToken()
        // }

    }
}