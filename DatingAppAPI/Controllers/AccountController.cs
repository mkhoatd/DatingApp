using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatingAppAPI.Data;
using DatingAppAPI.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using DatingAppAPI.DTOs;
using DatingAppAPI.Interfaces;

namespace DatingAppAPI.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly ILogger<AccountController> _logger;
        private readonly DataContext _context;
        private readonly ITokenService _tokenService;

        public AccountController(ILogger<AccountController> logger, DataContext context, ITokenService tokenService)
        {
            _logger = logger;
            _context = context;
            _tokenService = tokenService;
        }
        private async Task<bool> UserExists(string username)
        {
            var isExist = await _context.Users.AnyAsync(x => x.UserName.ToLower() == username.ToLower());
            if (isExist)
            {
                _logger.LogInformation($"User {username} already exists", username);
            }
            return isExist;
        }
        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
        {
            if (await UserExists(registerDto.Username))
            {
                return BadRequest("User already exists");
            }
            var username = registerDto.Username;
            var password = registerDto.Password;
            using var hmac = new HMACSHA512();
            var user = new AppUser
            {
                UserName = username,
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password)),
                PasswordSalt = hmac.Key
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return new UserDto
            {
                Username = user.UserName,
                Token = _tokenService.CreateToken(user)
            };
        }
        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.UserName == loginDto.Username);
            if (user == null)
            {
                _logger.LogInformation("user with username {loginDto.Username} not exist"
                    , loginDto.Username);
                return Unauthorized("Invalid username");
            }
            using var hmac = new HMACSHA512(user.PasswordSalt);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));
            _logger.LogDebug("Password sent has hash: {hash}", JsonConvert.SerializeObject(computedHash));
            var databaseHash = user.PasswordHash;
            _logger.LogDebug("Hash in database: {hash}", JsonConvert.SerializeObject(databaseHash));
            for (int i = 0; i < computedHash.Length; i++)
            {
                if (computedHash[i] != databaseHash[i]) return Unauthorized("Invalid password");
            }
            return new UserDto
            {
                Username = user.UserName,
                Token = _tokenService.CreateToken(user)
            };

        }
    }
}