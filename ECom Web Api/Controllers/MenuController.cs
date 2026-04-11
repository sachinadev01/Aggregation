using ECom_Web_Api.Data;
using ECom_Web_Api.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ECom_Web_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MenuController : ControllerBase
    {
        private readonly ECommerceContext _context;
        public MenuController(ECommerceContext context) => _context = context;

        [HttpGet]
        public async Task<IActionResult> GetMenus()
            => Ok(await _context.Menus.Include(m => m.Children).ToListAsync());

        [HttpPost]
        public async Task<IActionResult> CreateMenu(Menu menu)
        {
            _context.Menus.Add(menu);
            await _context.SaveChangesAsync();
            return Ok(menu);
        }

    }
}
