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
            if (await UserExists(registerDto.UserName))
            {
                return BadRequest("User already exists");
            }
            var username = registerDto.UserName;
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
                UserName = user.UserName,
                Token = _tokenService.CreateToken(user)
            };
        }
        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.UserName == loginDto.UserName);
            if (user == null)
            {
                _logger.LogInformation("user with username {loginDto.Username} not exist"
                    , loginDto.UserName);
                return Unauthorized("Invalid username");
            }
            using var hmac = new HMACSHA512(user.PasswordSalt);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));
            _logger.LogDebug("Password sent has hash: {hash}", JsonSerializer.Serialize(computedHash));
            var databaseHash = user.PasswordHash;
            _logger.LogDebug("Hash in database: {hash}", JsonSerializer.Serialize(databaseHash));
            for (int i = 0; i < computedHash.Length; i++)
            {
                if (computedHash[i] != databaseHash[i]) return Unauthorized("Invalid password");
            }
            return new UserDto
            {
                UserName = user.UserName,
                Token = _tokenService.CreateToken(user)
            };
        }
    }
}