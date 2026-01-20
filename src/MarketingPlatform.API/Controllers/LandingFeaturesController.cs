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
    [Route("api/landingfeatures")]
    public class LandingFeaturesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<LandingFeaturesController> _logger;

        public LandingFeaturesController(
            ApplicationDbContext context,
            ILogger<LandingFeaturesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Get all active landing features for display on landing page
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<LandingFeature>>>> GetLandingFeatures()
        {
            try
            {
                var features = await _context.LandingFeatures
                    .Where(f => f.IsActive && f.ShowOnLanding && !f.IsDeleted)
                    .OrderBy(f => f.DisplayOrder)
                    .ToListAsync();

                _logger.LogInformation("Retrieved {Count} landing features", features.Count);

                return Ok(ApiResponse<List<LandingFeature>>.SuccessResponse(features));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving landing features");
                return BadRequest(ApiResponse<List<LandingFeature>>.ErrorResponse(
                    "Failed to retrieve landing features",
                    new List<string> { ex.Message }));
            }
        }

        /// <summary>
        /// Get a specific landing feature by ID (SuperAdmin only)
        /// </summary>
        [HttpGet("{id}")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<ActionResult<ApiResponse<LandingFeature>>> GetById(int id)
        {
            try
            {
                var feature = await _context.LandingFeatures
                    .FirstOrDefaultAsync(f => f.Id == id && !f.IsDeleted);

                if (feature == null)
                    return NotFound(ApiResponse<LandingFeature>.ErrorResponse("Feature not found"));

                return Ok(ApiResponse<LandingFeature>.SuccessResponse(feature));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving feature {FeatureId}", id);
                return BadRequest(ApiResponse<LandingFeature>.ErrorResponse(
                    "Failed to retrieve feature",
                    new List<string> { ex.Message }));
            }
        }

        /// <summary>
        /// Create a new landing feature (SuperAdmin only)
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<ActionResult<ApiResponse<LandingFeature>>> Create([FromBody] LandingFeature feature)
        {
            try
            {
                feature.CreatedAt = DateTime.UtcNow;
                _context.LandingFeatures.Add(feature);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Created landing feature: {Title}", feature.Title);

                return Ok(ApiResponse<LandingFeature>.SuccessResponse(
                    feature,
                    "Feature created successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating landing feature");
                return BadRequest(ApiResponse<LandingFeature>.ErrorResponse(
                    "Failed to create feature",
                    new List<string> { ex.Message }));
            }
        }

        /// <summary>
        /// Update a landing feature (SuperAdmin only)
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<ActionResult<ApiResponse<LandingFeature>>> Update(int id, [FromBody] LandingFeature feature)
        {
            try
            {
                var existing = await _context.LandingFeatures
                    .FirstOrDefaultAsync(f => f.Id == id && !f.IsDeleted);

                if (existing == null)
                    return NotFound(ApiResponse<LandingFeature>.ErrorResponse("Feature not found"));

                // Update properties
                existing.Title = feature.Title;
                existing.ShortDescription = feature.ShortDescription;
                existing.DetailedDescription = feature.DetailedDescription;
                existing.IconClass = feature.IconClass;
                existing.ColorClass = feature.ColorClass;
                existing.DisplayOrder = feature.DisplayOrder;
                existing.IsActive = feature.IsActive;
                existing.ShowOnLanding = feature.ShowOnLanding;
                existing.StatTitle1 = feature.StatTitle1;
                existing.StatValue1 = feature.StatValue1;
                existing.StatTitle2 = feature.StatTitle2;
                existing.StatValue2 = feature.StatValue2;
                existing.StatTitle3 = feature.StatTitle3;
                existing.StatValue3 = feature.StatValue3;
                existing.CallToActionText = feature.CallToActionText;
                existing.CallToActionUrl = feature.CallToActionUrl;
                existing.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                _logger.LogInformation("Updated landing feature: {Title}", feature.Title);

                return Ok(ApiResponse<LandingFeature>.SuccessResponse(
                    existing,
                    "Feature updated successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating feature {FeatureId}", id);
                return BadRequest(ApiResponse<LandingFeature>.ErrorResponse(
                    "Failed to update feature",
                    new List<string> { ex.Message }));
            }
        }

        /// <summary>
        /// Delete a landing feature (SuperAdmin only)
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<ActionResult<ApiResponse<bool>>> Delete(int id)
        {
            try
            {
                var feature = await _context.LandingFeatures
                    .FirstOrDefaultAsync(f => f.Id == id && !f.IsDeleted);

                if (feature == null)
                    return NotFound(ApiResponse<bool>.ErrorResponse("Feature not found"));

                // Soft delete
                feature.IsDeleted = true;
                feature.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                _logger.LogInformation("Deleted landing feature: {Title}", feature.Title);

                return Ok(ApiResponse<bool>.SuccessResponse(true, "Feature deleted successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting feature {FeatureId}", id);
                return BadRequest(ApiResponse<bool>.ErrorResponse(
                    "Failed to delete feature",
                    new List<string> { ex.Message }));
            }
        }
    }
}
