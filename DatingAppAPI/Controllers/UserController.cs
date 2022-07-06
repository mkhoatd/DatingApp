using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatingAppAPI.Data;
using DatingAppAPI.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DatingAppAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly DataContext _context;

        public UserController(ILogger<UserController> logger, DataContext context)
        {
            _logger = logger;
            _context = context;
        }
        [HttpGet("UserList")]
        public async Task<ActionResult<IEnumerable<AppUser>>> GetUsersAsync() => await _context.Users.ToListAsync();
        [HttpGet("{id}")]
        public async Task<ActionResult<AppUser>> GetUserAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                _logger.LogDebug("User with {id} not found", id);
                return (ActionResult<AppUser>)NotFound();
            }
            return user;
        }
    }
}