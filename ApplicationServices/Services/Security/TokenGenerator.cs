using Domains;
using Domains.Services.Security;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ApplicationServices.Services.Security
{
    public class TokenGenerator : ITokenGenerator
    {
        public string GenerateToken(ClaimsData claims)
        {
            if (claims == null) throw new ArgumentNullException(nameof(claims));

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(AppSettings.Authentication.Secret);
            var expireMinutes = 30;

#if DEBUG
            expireMinutes = 60 * 24;
#endif
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                        new Claim(ClaimTypes.Sid, claims.UserId.ToString()),
                        new Claim(ClaimTypes.Role, claims.UserRole.ToString())
                }),
                Expires = DateTime.UtcNow.AddMinutes(expireMinutes),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}