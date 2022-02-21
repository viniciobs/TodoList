using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ToDoList.API.Services.TokenGenerator.Interfaces;
using ToDoList.API.Services.TokenGenerator.Models;
using ToDoList.UI.Configurations;

namespace ToDoList.API.Services.TokenGenerator
{
    public class TokenGenerator : ITokenGenerator
    {
        private readonly Authentication _authentication;

        public TokenGenerator(IOptions<Authentication> authentication)
        {
            if (authentication == null) throw new ArgumentNullException(nameof(authentication));

            _authentication = authentication.Value;
        }

        public string GenerateToken(ClaimsData claims)
        {
            if (claims == null) throw new ArgumentNullException(nameof(claims));

            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            byte[] key = Encoding.UTF8.GetBytes(_authentication.Secret);
            int expireMinutes = 30;

#if DEBUG
            expireMinutes = 60 * 24;
#endif
            SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                        new Claim(ClaimTypes.Sid, claims.UserId.ToString()),
                        new Claim(ClaimTypes.Role, claims.UserRole.ToString())
                }),
                Expires = DateTime.UtcNow.AddMinutes(expireMinutes),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}