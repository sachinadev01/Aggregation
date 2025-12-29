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
    public class BookingController : ControllerBase
    {
        private readonly ECommerceContext _context;
        private readonly IConfiguration _configuration;
        public BookingController(ECommerceContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost("Bookings")]
        public async Task<ActionResult<Booking>> CreateBooking(Booking booking)
        {
            try
            {
                if (booking == null)
                {
                    return BadRequest(new { message = "Booking data is required", statusCode = 422 });
                }

                // Get the user ID from the JWT claim (customer)
                var userIdClaim = User.FindFirst("userId")?.Value;
                if (userIdClaim == null || !int.TryParse(userIdClaim, out int userId))
                {
                    return Unauthorized(new { message = "Invalid user ID in the JWT claim", statusCode = 401 });
                }

                // Set the user ID (customer) for the booking
                booking.UserId = userId;

                // Get the service
                var service = await _context.Service.FindAsync(booking.ServiceId);
                if (service == null)
                {
                    return NotFound(new { message = "Service not found", statusCode = 404 });
                }

                // Set the provider id
                //booking.ProviderId = service.ProviderId;  // Service mein provider id save kiye the.

                _context.Booking.Add(booking);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetBookingById), new { id = booking.Id }, new
                {
                    message = "Booking created successfully",
                    statusCode = 201,
                    booking = booking
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating the booking", statusCode = 500, error = ex.Message });
            }
        }

        // GET: api/User/Bookings (Get all bookings)
        [HttpGet("Bookings")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Booking>>> GetBookings()
        {
            try
            {
                // Get the user ID from the JWT claim
                var userIdClaim = User.FindFirst("userId")?.Value;
                if (userIdClaim == null || !int.TryParse(userIdClaim, out int userId))
                {
                    return Unauthorized(new { message = "Invalid user ID in the JWT claim", statusCode = 401 });
                }

                // Retrieve bookings for the specific user
                var bookings = await _context.Booking.Where(b => b.UserId == userId).ToListAsync();

                if (bookings == null || bookings.Count == 0)
                {
                    return NotFound(new { message = "No bookings found for this user", statusCode = 404 });
                }

                return Ok(new { message = "Bookings retrieved successfully", statusCode = 200, bookings = bookings });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving bookings", statusCode = 500, error = ex.Message });
            }
        }

        [HttpGet("Bookings/{id}")]
        public async Task<ActionResult<Booking>> GetBookingById(int id)
        {
            try
            {
                var userIdClaim = User.FindFirst("userId")?.Value;
                if (userIdClaim == null || !int.TryParse(userIdClaim, out int userId))
                {
                    return Unauthorized(new { message = "Invalid user ID in the JWT claim", statusCode = 401 });
                }

                var booking = await _context.Booking.FindAsync(id);

                if (booking == null)
                {
                    return NotFound(new { message = "Booking not found", statusCode = 404 });
                }

                if (booking.UserId != userId)
                {
                    return BadRequest(new { message = "You are not authorized to view this booking", statusCode = 403 });
                }

                return Ok(new { message = "Booking retrieved successfully", statusCode = 200, booking = booking });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the booking", statusCode = 500, error = ex.Message });
            }
        }

        [HttpPut("Bookings/{id}")]
        public async Task<IActionResult> UpdateBooking(int id, Booking booking)
        {
            try
            {
                if (id != booking.Id)
                {
                    return BadRequest(new { message = "ID mismatch", statusCode = 400 });
                }

                var userIdClaim = User.FindFirst("userId")?.Value;
                if (userIdClaim == null || !int.TryParse(userIdClaim, out int userId))
                {
                    return Unauthorized(new { message = "Invalid user ID in the JWT claim", statusCode = 401 });
                }

                // Check if the booking belongs to the user making the request
                var existingBooking = await _context.Booking.FindAsync(id);
                if (existingBooking == null)
                {
                    return NotFound(new { message = "Booking not found", statusCode = 404 });
                }
                if (existingBooking.UserId != userId)
                {
                    return BadRequest(new { message = "You are not authorized to update this booking", statusCode = 403 });
                }

                _context.Entry(booking).State = EntityState.Modified;

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookingExists(id))
                    {
                        return NotFound(new { message = "Booking not found", statusCode = 404 });
                    }
                    else
                    {
                        throw;
                    }
                }

                return Ok(new { message = "Booking updated successfully", statusCode = 200 });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the booking", statusCode = 500, error = ex.Message });
            }
        }

        // DELETE: api/User/Bookings/{id} (Delete a booking)
        [HttpDelete("Bookings/{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteBooking(int id)
        {
            try
            {
                // Get the user ID from the JWT claim
                var userIdClaim = User.FindFirst("userId")?.Value;
                if (userIdClaim == null || !int.TryParse(userIdClaim, out int userId))
                {
                    return Unauthorized(new { message = "Invalid user ID in the JWT claim", statusCode = 401 });
                }

                // Check if the booking belongs to the user making the request
                var booking = await _context.Booking.FindAsync(id);
                if (booking == null)
                {
                    return NotFound(new { message = "Booking not found", statusCode = 404 });
                }
                if (booking.UserId != userId)
                {
                    return BadRequest(new { message = "You are not authorized to delete this booking", statusCode = 403 });
                }

                _context.Booking.Remove(booking);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Booking deleted successfully", statusCode = 200 });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the booking", statusCode = 500, error = ex.Message });
            }
        }

        private bool BookingExists(int id)
        {
            return _context.Booking.Any(e => e.Id == id);
        }
    }




}
