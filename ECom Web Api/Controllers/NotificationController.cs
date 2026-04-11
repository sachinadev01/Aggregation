using ECom_Web_Api.Data;
using ECom_Web_Api.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ECom_Web_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly ECommerceContext _context;
        private readonly IConfiguration _configuration;
        public NotificationController(ECommerceContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost("CreateNotification")]
        public async Task<ActionResult<Notification>> CreateNotification([FromBody] Notification request)
        {
            try
            {
                if (request == null)
                {
                    return BadRequest(new { message = "Notification data is required", statusCode = 422 });
                }

                var existingNotification = await _context.Notification
                    .FirstOrDefaultAsync(n => n.ServiceUserId == request.ServiceUserId);

                if (existingNotification != null)
                {
                    // Update existing notification
                    existingNotification.servicetitle = request.servicetitle;
                    existingNotification.loginid = request.loginid;
                    existingNotification.name = request.name;
                    existingNotification.username = request.username;
                    existingNotification.email = request.email;
                    existingNotification.phone = request.phone;
                    existingNotification.address = request.address;
                    existingNotification.CreatedDate = DateTime.UtcNow;
                    existingNotification.status = "Active";
                    existingNotification.userId = request.userId;

                    await _context.SaveChangesAsync();

                    return Ok(new
                    {
                        message = "Notification updated successfully",
                        statusCode = 200,
                        notification = existingNotification
                    });
                }
                else
                {
                    // Create new notification
                    var notification = new Notification
                    {
                        ServiceUserId = request.ServiceUserId,
                        servicetitle = request.servicetitle,
                        loginid = request.loginid,
                        name = request.name,
                        username = request.username,
                        email = request.email,
                        phone = request.phone,
                        address = request.address,
                        CreatedDate = DateTime.UtcNow,
                        status = "Active",
                    };

                    _context.Notification.Add(notification);
                    await _context.SaveChangesAsync();

                    return CreatedAtAction(nameof(GetNotificationById), new { id = notification.Id }, new
                    {
                        message = "Notification created successfully",
                        statusCode = 201,
                        notification = notification
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "An error occurred while creating or updating the notification",
                    statusCode = 500,
                    error = ex.Message
                });
            }
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<Notification>> GetNotificationById(int id)
        {
            var notification = await _context.Notification.FindAsync(id);

            if (notification == null)
            {
                return NotFound(new { message = "Notification not found", statusCode = 404 });
            }

            return Ok(notification);
        }

        [HttpPut("CancelNotification/{serviceUserId}")]
        public async Task<IActionResult> CancelNotificationByServiceUserId(int serviceUserId)
        {
            try
            {
                var notification = await _context.Notification
                    .FirstOrDefaultAsync(n => n.ServiceUserId == serviceUserId);

                if (notification == null)
                {
                    return NotFound(new { message = "Notification not found for the given ServiceUserId", statusCode = 404 });
                }

                notification.status = "Cancel";
                await _context.SaveChangesAsync();

                return Ok(new { message = "Notification status updated to Cancel", statusCode = 200 });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Error while updating notification status",
                    statusCode = 500,
                    error = ex.Message
                });
            }
        }

        [HttpGet("CustomerNotification")]
        public async Task<ActionResult<IEnumerable<Notification>>> GetNotification(int loginid)
        {
            try
            {
                var notify = await _context.Notification
                    .Where(s => s.userId == loginid)
                    .ToListAsync();

                if (notify == null || !notify.Any())
                {
                    return NotFound(new { message = "No Notification found for the provided userId", statusCode = 404 });
                }

                return Ok(new { message = "Notification retrieved successfully", statusCode = 200, notification = notify });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving notify", statusCode = 500, error = ex.Message });
            }
        }

    }
}
