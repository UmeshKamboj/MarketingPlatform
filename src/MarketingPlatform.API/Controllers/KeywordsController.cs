using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MarketingPlatform.Application.DTOs.Common;
using MarketingPlatform.Application.DTOs.Keyword;
using MarketingPlatform.Application.Interfaces;
using System.Security.Claims;

namespace MarketingPlatform.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class KeywordsController : ControllerBase
    {
        private readonly IKeywordService _keywordService;
        private readonly ILogger<KeywordsController> _logger;

        public KeywordsController(IKeywordService keywordService, ILogger<KeywordsController> logger)
        {
            _keywordService = keywordService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<PaginatedResult<KeywordDto>>>> GetKeywords([FromQuery] PagedRequest request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var keywords = await _keywordService.GetKeywordsAsync(userId, request);
            return Ok(ApiResponse<PaginatedResult<KeywordDto>>.SuccessResponse(keywords));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<KeywordDto>>> GetKeyword(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var keyword = await _keywordService.GetKeywordByIdAsync(userId, id);
            if (keyword == null)
                return NotFound(ApiResponse<KeywordDto>.ErrorResponse("Keyword not found"));

            return Ok(ApiResponse<KeywordDto>.SuccessResponse(keyword));
        }

        [HttpGet("active/list")]
        public async Task<ActionResult<ApiResponse<List<KeywordDto>>>> GetActiveKeywords()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var keywords = await _keywordService.GetActiveKeywordsAsync(userId);
            return Ok(ApiResponse<List<KeywordDto>>.SuccessResponse(keywords));
        }

        [HttpGet("check/{keywordText}")]
        public async Task<ActionResult<ApiResponse<bool>>> CheckKeywordAvailability(string keywordText)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var isAvailable = await _keywordService.CheckKeywordAvailabilityAsync(keywordText, userId);
            return Ok(ApiResponse<bool>.SuccessResponse(isAvailable, isAvailable ? "Keyword is available" : "Keyword is not available"));
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<KeywordDto>>> CreateKeyword([FromBody] CreateKeywordDto dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            try
            {
                var keyword = await _keywordService.CreateKeywordAsync(userId, dto);
                return Ok(ApiResponse<KeywordDto>.SuccessResponse(keyword, "Keyword created successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating keyword");
                return BadRequest(ApiResponse<KeywordDto>.ErrorResponse(ex.Message));
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<bool>>> UpdateKeyword(int id, [FromBody] UpdateKeywordDto dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            try
            {
                var result = await _keywordService.UpdateKeywordAsync(userId, id, dto);
                if (!result)
                    return NotFound(ApiResponse<bool>.ErrorResponse("Keyword not found"));

                return Ok(ApiResponse<bool>.SuccessResponse(result, "Keyword updated successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating keyword");
                return BadRequest(ApiResponse<bool>.ErrorResponse(ex.Message));
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteKeyword(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            try
            {
                var result = await _keywordService.DeleteKeywordAsync(userId, id);
                if (!result)
                    return NotFound(ApiResponse<bool>.ErrorResponse("Keyword not found"));

                return Ok(ApiResponse<bool>.SuccessResponse(result, "Keyword deleted successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting keyword");
                return BadRequest(ApiResponse<bool>.ErrorResponse(ex.Message));
            }
        }

        [HttpPost("{id}/link-campaign/{campaignId}")]
        public async Task<ActionResult<ApiResponse<bool>>> LinkToCampaign(int id, int campaignId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            try
            {
                var result = await _keywordService.LinkToCampaignAsync(userId, id, campaignId);
                if (!result)
                    return NotFound(ApiResponse<bool>.ErrorResponse("Keyword not found"));

                return Ok(ApiResponse<bool>.SuccessResponse(result, "Keyword linked to campaign successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error linking keyword to campaign");
                return BadRequest(ApiResponse<bool>.ErrorResponse(ex.Message));
            }
        }

        [HttpDelete("{id}/unlink-campaign")]
        public async Task<ActionResult<ApiResponse<bool>>> UnlinkFromCampaign(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            try
            {
                var result = await _keywordService.UnlinkFromCampaignAsync(userId, id);
                if (!result)
                    return NotFound(ApiResponse<bool>.ErrorResponse("Keyword not found"));

                return Ok(ApiResponse<bool>.SuccessResponse(result, "Keyword unlinked from campaign successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error unlinking keyword from campaign");
                return BadRequest(ApiResponse<bool>.ErrorResponse(ex.Message));
            }
        }

        [HttpPost("{id}/link-group/{groupId}")]
        public async Task<ActionResult<ApiResponse<bool>>> LinkToGroup(int id, int groupId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            try
            {
                var result = await _keywordService.LinkToGroupAsync(userId, id, groupId);
                if (!result)
                    return NotFound(ApiResponse<bool>.ErrorResponse("Keyword not found"));

                return Ok(ApiResponse<bool>.SuccessResponse(result, "Keyword linked to group successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error linking keyword to group");
                return BadRequest(ApiResponse<bool>.ErrorResponse(ex.Message));
            }
        }

        [HttpDelete("{id}/unlink-group")]
        public async Task<ActionResult<ApiResponse<bool>>> UnlinkFromGroup(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            try
            {
                var result = await _keywordService.UnlinkFromGroupAsync(userId, id);
                if (!result)
                    return NotFound(ApiResponse<bool>.ErrorResponse("Keyword not found"));

                return Ok(ApiResponse<bool>.SuccessResponse(result, "Keyword unlinked from group successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error unlinking keyword from group");
                return BadRequest(ApiResponse<bool>.ErrorResponse(ex.Message));
            }
        }

        [HttpGet("{id}/activities")]
        public async Task<ActionResult<ApiResponse<PaginatedResult<KeywordActivityDto>>>> GetKeywordActivities(int id, [FromQuery] PagedRequest request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var activities = await _keywordService.GetActivitiesAsync(userId, id, request);
            return Ok(ApiResponse<PaginatedResult<KeywordActivityDto>>.SuccessResponse(activities));
        }
    }
}
