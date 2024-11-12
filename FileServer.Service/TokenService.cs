using FileServer.Core.Services_Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace FileServer.Service
{
    public class TokenService : ITokenService
    {
        private readonly JwtConfigurations _jwt;
        public TokenService(IOptions<JwtConfigurations> options)
        {
                _jwt = options.Value; 
        }
        public async Task<string> CreateToken(IdentityUser user)
        {
            var claims = new List<Claim>
            {
                new Claim (JwtRegisteredClaimNames.NameId , user.Id),
                new Claim (JwtRegisteredClaimNames.Name , user.UserName),
                new Claim (JwtRegisteredClaimNames.Email, user.Email)
            };
            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
            var signingCredintials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha512Signature);

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _jwt.Issuer,
                audience: _jwt.Audience,
                claims: claims,
                expires: DateTime.Now.AddDays(double.Parse(_jwt.ExpiresOn.ToString())),
                signingCredentials: signingCredintials
                );

            return new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
        }
    }
}
