using ECom_Web_Api.Data;
using ECom_Web_Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ECom_Web_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceController : ControllerBase
    {
        private readonly ECommerceContext _context;
        private readonly IConfiguration _configuration; 
        public ServiceController(ECommerceContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost("Services")]
        public async Task<ActionResult<Service>> CreateService([FromBody] Service request) 
        {
            try
            {
                if (request == null)
                {
                    return BadRequest(new { message = "Service data is required", statusCode = 422 });
                }

                //var user = await _context.Users.FindAsync(request.ProviderId);

                //if (user == null) return Unauthorized(new { message = "User not found", statusCode = 401 });
                //if (user.role != "Provider")
                //{
                //    return BadRequest(new { message = "Only providers can create services", statusCode = 403 }); 
                //}

                var service = new Service
                {
                    //ProviderId = request.ProviderId, 
                    Title = request.Title,
                    Description = request.Description,
                    Category = request.Category,
                    Price = request.Price,
                    Name = request.Name,
                    userId = request.userId,
                   // Rating = request.Rating
                };

                _context.Service.Add(service);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetServiceById), new { id = service.Id }, new
                {
                    message = "Service created successfully",
                    statusCode = 201,
                    service = service
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating the service", statusCode = 500, error = ex.Message });
            }
        }

        [HttpGet("CustomerServices")]
        public async Task<ActionResult<IEnumerable<Service>>> GetServices()
        {
            try
            {

                var services = await _context.Service.ToListAsync();

                if (services == null || services.Count == 0)
                {
                    return NotFound(new { message = "No services found", statusCode = 404 });
                }

                return Ok(new { message = "Services retrieved successfully", statusCode = 200, services = services });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving services", statusCode = 500, error = ex.Message });
            }
        }

        [HttpGet("Services")]
        public async Task<ActionResult<IEnumerable<Service>>> GetServices([FromQuery] string userId)
        {
            try
            {
                var services = await _context.Service
                    .Where(s => s.userId == userId)
                    .ToListAsync();

                if (services == null || services.Count == 0)
                {
                    return NotFound(new { message = "No services found for the provided userId", statusCode = 404 });
                }

                return Ok(new { message = "Services retrieved successfully", statusCode = 200, services = services });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving services", statusCode = 500, error = ex.Message });
            }
        }



        [HttpGet("Services/{id}")]
        public async Task<ActionResult<Service>> GetServiceById(int id)
        {
            try
            {
                var service = await _context.Service.FindAsync(id);

                if (service == null)
                {
                    return NotFound(new { message = "Service not found", statusCode = 404 });
                }

                return Ok(new { message = "Service retrieved successfully", statusCode = 200, service = service });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the service", statusCode = 500, error = ex.Message });
            }
        }


        [HttpPut("Services/{id}")]
        public async Task<IActionResult> UpdateService(int id, [FromBody] Service request)
        {
            try
            {
                if (id != request.Id)
                {
                    return BadRequest(new { message = "ID mismatch", statusCode = 400 });
                }

                //var user = await _context.Users.FindAsync(request.ProviderId);
                //if (user == null)
                //{
                //    return BadRequest(new { message = "Invalid provider ID", statusCode = 400 });
                //}
                //if (user.role != "Provider")
                //{
                //    return BadRequest(new { message = "User is not a provider", statusCode = 403 });
                //}

                var existingService = await _context.Service.FindAsync(id);
                if (existingService == null)
                {
                    return NotFound(new { message = "Service not found", statusCode = 404 });
                }


                existingService.ProviderId = request.ProviderId;
                existingService.Title = request.Title;
                existingService.Description = request.Description;
                existingService.Category = request.Category;
                existingService.Price = request.Price;
                existingService.Name = request.Name;
                existingService.userId = request.userId;
                //existingService.Rating = request.Rating;

                _context.Entry(existingService).State = EntityState.Modified; // Important

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ServiceExists(id))
                    {
                        return NotFound(new { message = "Service not found", statusCode = 404 });
                    }
                    else
                    {
                        throw;
                    }
                }

                return Ok(new { message = "Service updated successfully", statusCode = 200 });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the service", statusCode = 500, error = ex.Message });
            }
        }

        [HttpDelete("Services/{id}")]
        public async Task<IActionResult> DeleteService(int id, [FromQuery] int userId)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId);
                if (user == null)
                {
                    return BadRequest(new { message = "Invalid provider ID", statusCode = 400 });
                }
                //if (user.role != "Provider")
                //{
                //    return BadRequest(new { message = "User is not a provider", statusCode = 403 });
                //}

                var service = await _context.Service.FindAsync(id);
                //if (service == null)
                //{
                //    return NotFound(new { message = "Service not found", statusCode = 404 });
                //}

                //if (service.ProviderId != userId)
                //{
                //    return BadRequest(new { message = "You are not the owner of the service", statusCode = 403 });
                //}

                _context.Service.Remove(service);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Service deleted successfully", statusCode = 200 });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the service", statusCode = 500, error = ex.Message });
            }
        }



        private bool ServiceExists(int id)
        {
            return _context.Service.Any(e => e.Id == id);
        }
    }



}
