using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Project.Core.Entities;
using Project.Core.Interfaces;

namespace Project.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration configuration;

        public TokenService(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        public async Task<string> CreateTokenAsync(AppUser User, UserManager<AppUser> usermanager)
        {
            var Authclaims = new List<Claim>()
            {
                new Claim(ClaimTypes.GivenName, User.FullName),
                new Claim(ClaimTypes.Email, User.Email),
            };

            var UserRoles = await usermanager.GetRolesAsync(User);
            foreach (var Role in UserRoles)
            {
                Authclaims.Add(new Claim(ClaimTypes.Role, Role));
            }

            var AuthKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Key"]));

            var Token = new JwtSecurityToken(
                   issuer: configuration["JWT:ValidIssuer"],
                   audience: configuration["JWT:ValidAudiance"],
                   expires: DateTime.Now.AddDays(double.Parse(configuration["JWT:DurationInDays"])),
                   claims: Authclaims,
                   signingCredentials: new SigningCredentials(AuthKey, SecurityAlgorithms.HmacSha256Signature)
                );

            return new JwtSecurityTokenHandler().WriteToken(Token);
        }
    }
}
