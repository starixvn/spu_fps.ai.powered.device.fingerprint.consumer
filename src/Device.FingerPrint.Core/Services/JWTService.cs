using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Device.FingerPrint.Core.Services
{
    public class JWTService
    {
        private readonly IAppSettings appSettings;
        private readonly byte[] keyJWT;

        public JWTService(IAppSettings appSettings)
        {
            this.appSettings = appSettings;
            keyJWT = Encoding.ASCII.GetBytes(appSettings.JWTPassword);
        }

        public byte[] GetJWTKey() => keyJWT;
        public string GenerateJwtToken()
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim("id", appSettings.ApiUrl) }),
                Expires = DateTime.UtcNow.AddMinutes(1),
                Issuer = appSettings.ApiUrl,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(keyJWT), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        public string GenerateJWT(DateTime expiredTime, string audience)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(appSettings.JWTPassword + "@" + audience));
            var tokenDescriptor = new SecurityTokenDescriptor
            {                
                Expires = expiredTime,               
                SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}
