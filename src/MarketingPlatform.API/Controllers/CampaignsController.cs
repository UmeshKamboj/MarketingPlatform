using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MarketingPlatform.Application.DTOs.Common;
using MarketingPlatform.Application.DTOs.Campaign;
using MarketingPlatform.Application.Interfaces;
using MarketingPlatform.Core.Enums;
using System.Security.Claims;

namespace MarketingPlatform.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class CampaignsController : ControllerBase
    {
        private readonly ICampaignService _campaignService;
        private readonly ILogger<CampaignsController> _logger;

        public CampaignsController(ICampaignService campaignService, ILogger<CampaignsController> logger)
        {
            _campaignService = campaignService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<PaginatedResult<CampaignDto>>>> GetCampaigns([FromQuery] PagedRequest request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var campaigns = await _campaignService.GetCampaignsAsync(userId, request);
            return Ok(ApiResponse<PaginatedResult<CampaignDto>>.SuccessResponse(campaigns));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<CampaignDto>>> GetCampaign(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var campaign = await _campaignService.GetCampaignByIdAsync(userId, id);
            if (campaign == null)
                return NotFound(ApiResponse<CampaignDto>.ErrorResponse("Campaign not found"));

            return Ok(ApiResponse<CampaignDto>.SuccessResponse(campaign));
        }

        [HttpGet("status/{status}")]
        public async Task<ActionResult<ApiResponse<List<CampaignDto>>>> GetCampaignsByStatus(CampaignStatus status)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var campaigns = await _campaignService.GetCampaignsByStatusAsync(userId, status);
            return Ok(ApiResponse<List<CampaignDto>>.SuccessResponse(campaigns));
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<CampaignDto>>> CreateCampaign([FromBody] CreateCampaignDto dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            try
            {
                var campaign = await _campaignService.CreateCampaignAsync(userId, dto);
                return Ok(ApiResponse<CampaignDto>.SuccessResponse(campaign, "Campaign created successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating campaign");
                return BadRequest(ApiResponse<CampaignDto>.ErrorResponse("Failed to create campaign", new List<string> { ex.Message }));
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<bool>>> UpdateCampaign(int id, [FromBody] UpdateCampaignDto dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var result = await _campaignService.UpdateCampaignAsync(userId, id, dto);
            if (!result)
                return BadRequest(ApiResponse<bool>.ErrorResponse("Failed to update campaign. Campaign not found or not in Draft status."));

            return Ok(ApiResponse<bool>.SuccessResponse(true, "Campaign updated successfully"));
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteCampaign(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var result = await _campaignService.DeleteCampaignAsync(userId, id);
            if (!result)
                return BadRequest(ApiResponse<bool>.ErrorResponse("Failed to delete campaign. Campaign not found or is currently running."));

            return Ok(ApiResponse<bool>.SuccessResponse(true, "Campaign deleted successfully"));
        }

        [HttpPost("{id}/duplicate")]
        public async Task<ActionResult<ApiResponse<bool>>> DuplicateCampaign(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var result = await _campaignService.DuplicateCampaignAsync(userId, id);
            if (!result)
                return BadRequest(ApiResponse<bool>.ErrorResponse("Failed to duplicate campaign. Campaign not found."));

            return Ok(ApiResponse<bool>.SuccessResponse(true, "Campaign duplicated successfully"));
        }

        [HttpPost("{id}/schedule")]
        public async Task<ActionResult<ApiResponse<bool>>> ScheduleCampaign(int id, [FromBody] DateTime scheduledDate)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var result = await _campaignService.ScheduleCampaignAsync(userId, id, scheduledDate);
            if (!result)
                return BadRequest(ApiResponse<bool>.ErrorResponse("Failed to schedule campaign. Campaign not found or not in Draft status."));

            return Ok(ApiResponse<bool>.SuccessResponse(true, "Campaign scheduled successfully"));
        }

        [HttpPost("{id}/start")]
        public async Task<ActionResult<ApiResponse<bool>>> StartCampaign(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var result = await _campaignService.StartCampaignAsync(userId, id);
            if (!result)
                return BadRequest(ApiResponse<bool>.ErrorResponse("Failed to start campaign. Campaign not found or not in Draft/Scheduled status."));

            return Ok(ApiResponse<bool>.SuccessResponse(true, "Campaign started successfully"));
        }

        [HttpPost("{id}/pause")]
        public async Task<ActionResult<ApiResponse<bool>>> PauseCampaign(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var result = await _campaignService.PauseCampaignAsync(userId, id);
            if (!result)
                return BadRequest(ApiResponse<bool>.ErrorResponse("Failed to pause campaign. Campaign not found or not running."));

            return Ok(ApiResponse<bool>.SuccessResponse(true, "Campaign paused successfully"));
        }

        [HttpPost("{id}/resume")]
        public async Task<ActionResult<ApiResponse<bool>>> ResumeCampaign(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var result = await _campaignService.ResumeCampaignAsync(userId, id);
            if (!result)
                return BadRequest(ApiResponse<bool>.ErrorResponse("Failed to resume campaign. Campaign not found or not paused."));

            return Ok(ApiResponse<bool>.SuccessResponse(true, "Campaign resumed successfully"));
        }

        [HttpPost("{id}/cancel")]
        public async Task<ActionResult<ApiResponse<bool>>> CancelCampaign(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var result = await _campaignService.CancelCampaignAsync(userId, id);
            if (!result)
                return BadRequest(ApiResponse<bool>.ErrorResponse("Failed to cancel campaign. Campaign not found or already completed/failed."));

            return Ok(ApiResponse<bool>.SuccessResponse(true, "Campaign cancelled successfully"));
        }

        [HttpGet("{id}/stats")]
        public async Task<ActionResult<ApiResponse<CampaignStatsDto>>> GetCampaignStats(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            try
            {
                var stats = await _campaignService.GetCampaignStatsAsync(userId, id);
                return Ok(ApiResponse<CampaignStatsDto>.SuccessResponse(stats));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting campaign stats");
                return BadRequest(ApiResponse<CampaignStatsDto>.ErrorResponse("Failed to get campaign statistics", new List<string> { ex.Message }));
            }
        }

        [HttpPost("calculate-audience")]
        public async Task<ActionResult<ApiResponse<int>>> CalculateAudienceSize([FromBody] CampaignAudienceDto audience)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var size = await _campaignService.CalculateAudienceSizeAsync(userId, audience);
            return Ok(ApiResponse<int>.SuccessResponse(size));
        }
    }
}
