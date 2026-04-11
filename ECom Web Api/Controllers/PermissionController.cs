using ECom_Web_Api.Data;
using ECom_Web_Api.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ECom_Web_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PermissionController : ControllerBase
    {
        private readonly ECommerceContext _context;
        public PermissionController(ECommerceContext context) => _context = context;

        [HttpPost("Assign")]
        public async Task<IActionResult> AssignPermission(RolePermission permission)
        {
            _context.RolePermissions.Add(permission);
            await _context.SaveChangesAsync();
            return Ok(permission);
        }

        [HttpGet("GetByRole/{roleId}")]
        public async Task<IActionResult> GetPermissions(int roleId)
        {
            var permissions = await _context.RolePermissions
                .Include(rp => rp.Menu)
                .Where(rp => rp.RoleId == roleId)
                .ToListAsync();

            return Ok(permissions);
        }

    }
}
