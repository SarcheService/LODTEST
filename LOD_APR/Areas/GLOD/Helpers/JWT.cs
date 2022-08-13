using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Web;

namespace LOD_APR.Areas.GLOD.Helpers
{
    public class JWT
    {
        public string GenerateJwtToken(string secret, string purpose, string entity, string run)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(secret);
            string notAfter = DateTime.Now.AddMinutes(20).ToString("yyyy-MM-ddTHH:mm:ss");
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim("purpose", purpose), new Claim("entity", entity), new Claim("run", run), new Claim("expiration", notAfter) }),
                Expires = DateTime.Now.AddMinutes(20),
                NotBefore = DateTime.Now,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }

}