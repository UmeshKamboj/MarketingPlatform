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
    [Route("api/landingfaqs")]
    public class LandingFaqsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<LandingFaqsController> _logger;

        public LandingFaqsController(
            ApplicationDbContext context,
            ILogger<LandingFaqsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Get all active landing FAQs for display on landing page
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<LandingFaq>>>> GetLandingFaqs()
        {
            try
            {
                var faqs = await _context.LandingFaqs
                    .Where(f => f.IsActive && f.ShowOnLanding && !f.IsDeleted)
                    .OrderBy(f => f.DisplayOrder)
                    .ToListAsync();

                _logger.LogInformation("Retrieved {Count} landing FAQs", faqs.Count);

                return Ok(ApiResponse<List<LandingFaq>>.SuccessResponse(faqs));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving landing FAQs");
                return BadRequest(ApiResponse<List<LandingFaq>>.ErrorResponse(
                    "Failed to retrieve landing FAQs",
                    new List<string> { ex.Message }));
            }
        }

        /// <summary>
        /// Get a specific landing FAQ by ID (SuperAdmin only)
        /// </summary>
        [HttpGet("{id}")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<ActionResult<ApiResponse<LandingFaq>>> GetById(int id)
        {
            try
            {
                var faq = await _context.LandingFaqs
                    .FirstOrDefaultAsync(f => f.Id == id && !f.IsDeleted);

                if (faq == null)
                    return NotFound(ApiResponse<LandingFaq>.ErrorResponse("FAQ not found"));

                return Ok(ApiResponse<LandingFaq>.SuccessResponse(faq));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving FAQ {FaqId}", id);
                return BadRequest(ApiResponse<LandingFaq>.ErrorResponse(
                    "Failed to retrieve FAQ",
                    new List<string> { ex.Message }));
            }
        }

        /// <summary>
        /// Create a new landing FAQ (SuperAdmin only)
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<ActionResult<ApiResponse<LandingFaq>>> Create([FromBody] LandingFaq faq)
        {
            try
            {
                faq.CreatedAt = DateTime.UtcNow;
                _context.LandingFaqs.Add(faq);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Created landing FAQ: {Question}", faq.Question);

                return Ok(ApiResponse<LandingFaq>.SuccessResponse(
                    faq,
                    "FAQ created successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating landing FAQ");
                return BadRequest(ApiResponse<LandingFaq>.ErrorResponse(
                    "Failed to create FAQ",
                    new List<string> { ex.Message }));
            }
        }

        /// <summary>
        /// Update a landing FAQ (SuperAdmin only)
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<ActionResult<ApiResponse<LandingFaq>>> Update(int id, [FromBody] LandingFaq faq)
        {
            try
            {
                var existing = await _context.LandingFaqs
                    .FirstOrDefaultAsync(f => f.Id == id && !f.IsDeleted);

                if (existing == null)
                    return NotFound(ApiResponse<LandingFaq>.ErrorResponse("FAQ not found"));

                // Update properties
                existing.Question = faq.Question;
                existing.Answer = faq.Answer;
                existing.IconClass = faq.IconClass;
                existing.IconColor = faq.IconColor;
                existing.DisplayOrder = faq.DisplayOrder;
                existing.IsActive = faq.IsActive;
                existing.ShowOnLanding = faq.ShowOnLanding;
                existing.Category = faq.Category;
                existing.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                _logger.LogInformation("Updated landing FAQ: {Question}", faq.Question);

                return Ok(ApiResponse<LandingFaq>.SuccessResponse(
                    existing,
                    "FAQ updated successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating FAQ {FaqId}", id);
                return BadRequest(ApiResponse<LandingFaq>.ErrorResponse(
                    "Failed to update FAQ",
                    new List<string> { ex.Message }));
            }
        }

        /// <summary>
        /// Delete a landing FAQ (SuperAdmin only)
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<ActionResult<ApiResponse<bool>>> Delete(int id)
        {
            try
            {
                var faq = await _context.LandingFaqs
                    .FirstOrDefaultAsync(f => f.Id == id && !f.IsDeleted);

                if (faq == null)
                    return NotFound(ApiResponse<bool>.ErrorResponse("FAQ not found"));

                // Soft delete
                faq.IsDeleted = true;
                faq.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                _logger.LogInformation("Deleted landing FAQ: {Question}", faq.Question);

                return Ok(ApiResponse<bool>.SuccessResponse(true, "FAQ deleted successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting FAQ {FaqId}", id);
                return BadRequest(ApiResponse<bool>.ErrorResponse(
                    "Failed to delete FAQ",
                    new List<string> { ex.Message }));
            }
        }
    }
}
