using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MarketingPlatform.Application.DTOs.Common;
using MarketingPlatform.Infrastructure.Data;
using MarketingPlatform.Core.Entities;

namespace MarketingPlatform.API.Controllers
{
    [AllowAnonymous]
    [ApiController]
    [Route("api/testimonials")]
    public class TestimonialsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<TestimonialsController> _logger;

        public TestimonialsController(
            ApplicationDbContext context,
            ILogger<TestimonialsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Get all active testimonials for landing page
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<Testimonial>>>> GetTestimonials()
        {
            try
            {
                var testimonials = await _context.Testimonials
                    .Where(t => t.IsActive && !t.IsDeleted)
                    .OrderBy(t => t.DisplayOrder)
                    .ToListAsync();

                _logger.LogInformation("Retrieved {Count} testimonials", testimonials.Count);

                return Ok(ApiResponse<List<Testimonial>>.SuccessResponse(testimonials));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving testimonials");
                return BadRequest(ApiResponse<List<Testimonial>>.ErrorResponse(
                    "Failed to retrieve testimonials",
                    new List<string> { ex.Message }));
            }
        }

        /// <summary>
        /// Get a specific testimonial by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<Testimonial>>> GetById(int id)
        {
            try
            {
                var testimonial = await _context.Testimonials
                    .FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted);

                if (testimonial == null)
                    return NotFound(ApiResponse<Testimonial>.ErrorResponse("Testimonial not found"));

                return Ok(ApiResponse<Testimonial>.SuccessResponse(testimonial));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving testimonial {TestimonialId}", id);
                return BadRequest(ApiResponse<Testimonial>.ErrorResponse(
                    "Failed to retrieve testimonial",
                    new List<string> { ex.Message }));
            }
        }
    }
}
