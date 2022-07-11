using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using DatingAppAPI.Entities;
using DatingAppAPI.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace DatingAppAPI.Services
{
    public class TokenService : ITokenService
    {
        private readonly SymmetricSecurityKey _key;
        private readonly ILogger<TokenService> _logger;
        public TokenService(IConfiguration config, ILogger<TokenService> logger)
        {
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["TokenKey"]));
            _logger = logger;
        }
        public string CreateToken(AppUser user)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.NameId, user.UserName)
            };
            _logger.LogDebug("Creating token for user {user.UserName} with claim {claims}", user.UserName, JsonSerializer.Serialize(claims));
            var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(777),
                SigningCredentials = creds
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            _logger.LogDebug("Token for user {user.UserName}: {token}", user.UserName, JsonSerializer.Serialize(token));
            return tokenHandler.WriteToken(token);
        }
    }
}