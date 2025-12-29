using ECom_Web_Api.Data;
using ECom_Web_Api.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ECom_Web_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewController : ControllerBase
    {
        private readonly ECommerceContext _context;
        private readonly IConfiguration _configuration;
        public ReviewController(ECommerceContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost("Reviews")]
        public async Task<ActionResult<Reviews>> AddReview([FromBody] Reviews review)
        {
            try
            {
                if (review == null)
                {
                    return BadRequest(new { message = "Review data is required", statusCode = 400 });
                }

                // Validate User and Service Ids
                if (!await _context.Users.AnyAsync(u => u.Id == review.UserId))
                {
                    return BadRequest(new { message = "Invalid User ID", statusCode = 400 });
                }

                if (!await _context.Service.AnyAsync(s => s.Id == review.ServiceId))
                {
                    return BadRequest(new { message = "Invalid Service ID", statusCode = 400 });
                }

                _context.Reviews.Add(review);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetReviewsForService), new { serviceId = review.ServiceId }, new
                {
                    message = "Review added successfully",
                    statusCode = 201,
                    review = review
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while adding the review", statusCode = 500, error = ex.Message });
            }
        }


        [HttpGet("Reviews/{serviceId}")]
        public async Task<ActionResult<IEnumerable<Reviews>>> GetReviewsForService(int serviceId)
        {
            try
            {
                var reviews = await _context.Reviews
                    .Where(r => r.ServiceId == serviceId)
                    .ToListAsync();

                if (reviews == null || reviews.Count == 0)
                {
                    return NotFound(new { message = "No reviews found for this service", statusCode = 404 });
                }

                return Ok(new { message = "Reviews retrieved successfully", statusCode = 200, reviews = reviews });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving reviews", statusCode = 500, error = ex.Message });
            }
        }

        [HttpDelete("Reviews/{id}")]
        public async Task<IActionResult> DeleteReview(int id)
        {
            try
            {
                var review = await _context.Reviews.FindAsync(id);

                if (review == null)
                {
                    return NotFound(new { message = "Review not found", statusCode = 404 });
                }

                _context.Reviews.Remove(review);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Review deleted successfully", statusCode = 200 });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the review", statusCode = 500, error = ex.Message });
            }
        }

        private bool ReviewExists(int id)
        {
            return _context.Reviews.Any(e => e.Id == id);
        }
    }
}
