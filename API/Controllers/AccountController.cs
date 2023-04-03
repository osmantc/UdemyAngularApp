using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace API.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly DataContext _context;

        public AccountController(DataContext context)
        {
            this._context = context;

        }

        [HttpPost("Register")]
        public async Task<ActionResult<AppUser>> Register(AccountUser user)
        {
            if (UserAlreadyExist(user)) return BadRequest("User name has already been taken");

            using var mac = new HMACSHA512();

            AppUser appUser = new AppUser
            {
                UserName = user.UserName,
                PasswordHash = mac.ComputeHash(Encoding.UTF8.GetBytes(user.Password)),
                PasswordSalt = mac.Key
            };

            EntityEntry<AppUser> returnedUser = await _context.AppUsers.AddAsync(appUser);
            _context.SaveChangesAsync();

            return Ok(returnedUser.Entity);
        }

        private bool UserAlreadyExist(AccountUser user)
        {
            return _context.AppUsers.Any(x => x.UserName == user.UserName);
        }



    }
}