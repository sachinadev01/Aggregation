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
    public class PaymentsController : ControllerBase
    {
        private readonly ECommerceContext _context;
        private readonly IConfiguration _configuration;
        public PaymentsController(ECommerceContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }


        // POST: api/user/initiatePayment
        [HttpPost("initiatePayment")]
        public async Task<ActionResult<Payment>> InitiatePayment(Payment request)
        {
            try
            {
                // Validate the request
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { message = "Invalid payment request", statusCode = 400, errors = ModelState });
                }

                // Check if User and Booking exist
                var user = await _context.Users.FindAsync(request.UserId);
                if (user == null)
                {
                    return BadRequest(new { message = "Invalid UserId", statusCode = 400 });
                }

                var booking = await _context.Booking.FindAsync(request.BookingId);
                if (booking == null)
                {
                    return BadRequest(new { message = "Invalid BookingId", statusCode = 400 });
                }

                // Create a new Payment object
                var payment = new Payment
                {
                    UserId = request.UserId,
                    BookingId = request.BookingId,
                    Amount = request.Amount,
                    PaymentMethod = request.PaymentMethod,
                    PaymentStatus = "Pending"
                };

                _context.Payment.Add(payment);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetPaymentStatus), new { transactionId = payment.TransactionId }, new
                {
                    message = "Payment initiated successfully",
                    statusCode = 201,
                    payment
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred during payment initiation", statusCode = 500 });
            }
        }


        [HttpGet("paymentStatus/{transactionId}")]
        public async Task<ActionResult<Payment>> GetPaymentStatus(string transactionId)
        {
            try
            {
                var payment = await _context.Payment.FirstOrDefaultAsync(p => p.TransactionId == transactionId);

                if (payment == null)
                {
                    return NotFound(new { message = "Payment not found", statusCode = 404 });
                }

                return Ok(new
                {
                    message = "Payment status retrieved successfully",
                    statusCode = 200,
                    payment
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving payment status", statusCode = 500 });
            }
        }


        [HttpGet("transactions")]
        public async Task<ActionResult<IEnumerable<Payment>>> GetUserTransactions()
        {
            try
            {
                // Get the User ID from the JWT token claims (RECOMMENDED FOR SECURITY)
                var userIdClaim = User.FindFirst("userId")?.Value;  // Assuming you put userId in the token
                if (userIdClaim == null || !int.TryParse(userIdClaim, out int userId))
                {
                    return BadRequest(new { message = "Invalid or missing UserId in token", statusCode = 400 });
                }

                var payments = await _context.Payment
                                              .Where(p => p.UserId == userId)
                                              .ToListAsync();

                return Ok(new
                {
                    message = "User transactions retrieved successfully",
                    statusCode = 200,
                    payments
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving user transactions", statusCode = 500 });
            }
        }


    }
}
