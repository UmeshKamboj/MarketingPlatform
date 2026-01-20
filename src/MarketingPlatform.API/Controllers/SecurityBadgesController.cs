using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MarketingPlatform.Application.DTOs.Common;
using MarketingPlatform.Infrastructure.Data;
using MarketingPlatform.Core.Entities;

namespace MarketingPlatform.API.Controllers
{
    [ApiController]
    [Route("api/securitybadges")]
    public class SecurityBadgesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<SecurityBadgesController> _logger;

        public SecurityBadgesController(
            ApplicationDbContext context,
            ILogger<SecurityBadgesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Get all active security badges for landing page (Public endpoint)
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<List<SecurityBadge>>>> GetSecurityBadges()
        {
            try
            {
                var badges = await _context.SecurityBadges
                    .Where(b => b.IsActive && b.ShowOnLanding && !b.IsDeleted)
                    .OrderBy(b => b.DisplayOrder)
                    .ToListAsync();

                _logger.LogInformation("Retrieved {Count} security badges for landing page", badges.Count);

                return Ok(ApiResponse<List<SecurityBadge>>.SuccessResponse(badges));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving security badges");
                return BadRequest(ApiResponse<List<SecurityBadge>>.ErrorResponse(
                    "Failed to retrieve security badges",
                    new List<string> { ex.Message }));
            }
        }

        /// <summary>
        /// Get a specific security badge by ID (SuperAdmin only)
        /// </summary>
        [HttpGet("{id}")]
        [Authorize(Policy = "SuperAdminOnly")]
        public async Task<ActionResult<ApiResponse<SecurityBadge>>> GetById(int id)
        {
            try
            {
                var badge = await _context.SecurityBadges
                    .FirstOrDefaultAsync(b => b.Id == id && !b.IsDeleted);

                if (badge == null)
                    return NotFound(ApiResponse<SecurityBadge>.ErrorResponse("Security badge not found"));

                return Ok(ApiResponse<SecurityBadge>.SuccessResponse(badge));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving security badge {BadgeId}", id);
                return BadRequest(ApiResponse<SecurityBadge>.ErrorResponse(
                    "Failed to retrieve security badge",
                    new List<string> { ex.Message }));
            }
        }

        /// <summary>
        /// Create a new security badge (SuperAdmin only)
        /// </summary>
        [HttpPost]
        [Authorize(Policy = "SuperAdminOnly")]
        public async Task<ActionResult<ApiResponse<SecurityBadge>>> Create([FromBody] SecurityBadge badge)
        {
            try
            {
                badge.CreatedAt = DateTime.UtcNow;
                badge.UpdatedAt = DateTime.UtcNow;

                _context.SecurityBadges.Add(badge);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Created security badge: {Title}", badge.Title);

                return Ok(ApiResponse<SecurityBadge>.SuccessResponse(
                    badge,
                    "Security badge created successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating security badge");
                return BadRequest(ApiResponse<SecurityBadge>.ErrorResponse(
                    "Failed to create security badge",
                    new List<string> { ex.Message }));
            }
        }

        /// <summary>
        /// Update a security badge (SuperAdmin only)
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Policy = "SuperAdminOnly")]
        public async Task<ActionResult<ApiResponse<SecurityBadge>>> Update(int id, [FromBody] SecurityBadge badge)
        {
            try
            {
                var existingBadge = await _context.SecurityBadges
                    .FirstOrDefaultAsync(b => b.Id == id && !b.IsDeleted);

                if (existingBadge == null)
                    return NotFound(ApiResponse<SecurityBadge>.ErrorResponse("Security badge not found"));

                existingBadge.Title = badge.Title;
                existingBadge.Subtitle = badge.Subtitle;
                existingBadge.IconUrl = badge.IconUrl;
                existingBadge.Description = badge.Description;
                existingBadge.DisplayOrder = badge.DisplayOrder;
                existingBadge.IsActive = badge.IsActive;
                existingBadge.ShowOnLanding = badge.ShowOnLanding;
                existingBadge.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                _logger.LogInformation("Updated security badge: {Title}", existingBadge.Title);

                return Ok(ApiResponse<SecurityBadge>.SuccessResponse(
                    existingBadge,
                    "Security badge updated successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating security badge {BadgeId}", id);
                return BadRequest(ApiResponse<SecurityBadge>.ErrorResponse(
                    "Failed to update security badge",
                    new List<string> { ex.Message }));
            }
        }

        /// <summary>
        /// Delete a security badge (SuperAdmin only - soft delete)
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Policy = "SuperAdminOnly")]
        public async Task<ActionResult<ApiResponse<bool>>> Delete(int id)
        {
            try
            {
                var badge = await _context.SecurityBadges
                    .FirstOrDefaultAsync(b => b.Id == id && !b.IsDeleted);

                if (badge == null)
                    return NotFound(ApiResponse<bool>.ErrorResponse("Security badge not found"));

                badge.IsDeleted = true;
                badge.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                _logger.LogInformation("Deleted security badge: {Title}", badge.Title);

                return Ok(ApiResponse<bool>.SuccessResponse(
                    true,
                    "Security badge deleted successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting security badge {BadgeId}", id);
                return BadRequest(ApiResponse<bool>.ErrorResponse(
                    "Failed to delete security badge",
                    new List<string> { ex.Message }));
            }
        }
    }
}
