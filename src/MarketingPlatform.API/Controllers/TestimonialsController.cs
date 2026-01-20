using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MarketingPlatform.Application.DTOs.Common;
using MarketingPlatform.Application.Interfaces;
using MarketingPlatform.Core.Entities;

namespace MarketingPlatform.API.Controllers
{
    [AllowAnonymous]
    [ApiController]
    [Route("api/testimonials")]
    public class TestimonialsController : ControllerBase
    {
        private readonly ITestimonialService _testimonialService;
        private readonly ILogger<TestimonialsController> _logger;

        public TestimonialsController(
            ITestimonialService testimonialService,
            ILogger<TestimonialsController> logger)
        {
            _testimonialService = testimonialService;
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
                var testimonials = await _testimonialService.GetAllActiveAsync();
                var testimonialList = testimonials.ToList();

                _logger.LogInformation("Retrieved {Count} testimonials", testimonialList.Count);

                return Ok(ApiResponse<List<Testimonial>>.SuccessResponse(testimonialList));
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
                var testimonial = await _testimonialService.GetByIdAsync(id);

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
