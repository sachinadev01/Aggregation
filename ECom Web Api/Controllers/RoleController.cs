using ECom_Web_Api.Data;
using ECom_Web_Api.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ECom_Web_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly ECommerceContext _context;
        public RoleController(ECommerceContext context) => _context = context;

        [HttpGet]
        public async Task<IActionResult> GetRoles()
            => Ok(await _context.Roles.ToListAsync());

        [HttpPost]
        public async Task<IActionResult> CreateRole(Role role)
        {
            _context.Roles.Add(role);
            await _context.SaveChangesAsync();
            return Ok(role);
        }

    }
}
