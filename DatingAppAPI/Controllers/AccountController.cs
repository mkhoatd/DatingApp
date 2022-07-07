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

namespace DatingAppAPI.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly ILogger<AccountController> _logger;
        private readonly DataContext _context;
        public AccountController(ILogger<AccountController> logger, DataContext context)
        {
            _logger = logger;
            _context = context;
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
        public async Task<ActionResult<AppUser>> Register(RegisterDTO registerDTO)
        {
            if (await UserExists(registerDTO.Username))
            {
                return BadRequest("User already exists");
            }
            var username = registerDTO.Username;
            var password = registerDTO.Password;
            using var hmac = new HMACSHA512();
            var user = new AppUser
            {
                UserName = username,
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password)),
                PasswordSalt = hmac.Key
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }
    }
}