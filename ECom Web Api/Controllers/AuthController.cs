using ECom_Web_Api.Data;
using ECom_Web_Api.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ECom_Web_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ECommerceContext _context;
        public AuthController(ECommerceContext context) => _context = context;

        [HttpPost("Register")]
        public async Task<IActionResult> Register(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return Ok(user);
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var user = await _context.AccessUser
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Username == dto.Username && u.PasswordHash == dto.Password);

            if (user == null) return Unauthorized("Invalid credentials");

            var menus = await _context.RolePermissions
                .Include(rp => rp.Menu)
                .Where(rp => rp.RoleId == user.RoleId && rp.IsVisible)
                .Select(rp => rp.Menu)
                .ToListAsync();

            return Ok(new { User = user.Username, Role = user.Role.RoleName, Menus = menus });
        }
    }

    public class LoginDto
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
