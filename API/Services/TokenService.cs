using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using API.Entities;
using Microsoft.IdentityModel.Tokens;

namespace API.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;
        public TokenService(IConfiguration configuration)
        {
            this._configuration = configuration;

        }

        public string CreateToken(AppUser appUser)
        {
            List<Claim> claims = new List<Claim>(){

                    new Claim(JwtRegisteredClaimNames.Sub,appUser.UserName),
                    new Claim("UserId", appUser.Id.ToString())
        };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["TokenKey"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            // var expiresIn = DateTime.Now.AddMinutes(30);
            // var tokenDescription = new JwtSecurityToken(issuer: "test", audience: "test", claims: claims, expires: expiresIn, signingCredentials: credentials);

            // string token = new JwtSecurityTokenHandler().WriteToken(tokenDescription);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(7),
                SigningCredentials = credentials
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

    }
}