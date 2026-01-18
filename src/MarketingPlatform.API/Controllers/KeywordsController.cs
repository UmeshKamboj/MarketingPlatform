using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MarketingPlatform.Application.DTOs.Common;
using MarketingPlatform.Application.DTOs.Keyword;
using MarketingPlatform.Application.Interfaces;
using MarketingPlatform.Core.Enums;
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

        [HttpGet("status/{status}")]
        public async Task<ActionResult<ApiResponse<List<KeywordDto>>>> GetKeywordsByStatus(KeywordStatus status)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var keywords = await _keywordService.GetKeywordsByStatusAsync(userId, status);
            return Ok(ApiResponse<List<KeywordDto>>.SuccessResponse(keywords));
        }

        [HttpGet("check-availability")]
        public async Task<ActionResult<ApiResponse<bool>>> CheckAvailability([FromQuery] string keywordText)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            if (string.IsNullOrWhiteSpace(keywordText))
                return BadRequest(ApiResponse<bool>.ErrorResponse("Keyword text is required"));

            var isAvailable = await _keywordService.CheckKeywordAvailabilityAsync(keywordText, userId);
            return Ok(ApiResponse<bool>.SuccessResponse(isAvailable));
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
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Failed to create keyword");
                return BadRequest(ApiResponse<KeywordDto>.ErrorResponse(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating keyword");
                return BadRequest(ApiResponse<KeywordDto>.ErrorResponse("Failed to create keyword", new List<string> { ex.Message }));
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
                    return BadRequest(ApiResponse<bool>.ErrorResponse("Failed to update keyword. Keyword not found or is globally reserved."));

                return Ok(ApiResponse<bool>.SuccessResponse(true, "Keyword updated successfully"));
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Failed to update keyword");
                return BadRequest(ApiResponse<bool>.ErrorResponse(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating keyword");
                return BadRequest(ApiResponse<bool>.ErrorResponse("Failed to update keyword", new List<string> { ex.Message }));
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteKeyword(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var result = await _keywordService.DeleteKeywordAsync(userId, id);
            if (!result)
                return BadRequest(ApiResponse<bool>.ErrorResponse("Failed to delete keyword. Keyword not found or is globally reserved."));

            return Ok(ApiResponse<bool>.SuccessResponse(true, "Keyword deleted successfully"));
        }

        [HttpGet("{id}/activities")]
        public async Task<ActionResult<ApiResponse<PaginatedResult<KeywordActivityDto>>>> GetKeywordActivities(int id, [FromQuery] PagedRequest request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            try
            {
                var activities = await _keywordService.GetKeywordActivitiesAsync(id, userId, request);
                return Ok(ApiResponse<PaginatedResult<KeywordActivityDto>>.SuccessResponse(activities));
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Failed to get keyword activities");
                return BadRequest(ApiResponse<PaginatedResult<KeywordActivityDto>>.ErrorResponse(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting keyword activities");
                return BadRequest(ApiResponse<PaginatedResult<KeywordActivityDto>>.ErrorResponse("Failed to get keyword activities", new List<string> { ex.Message }));
            }
        }

        [HttpPost("process-inbound")]
        [AllowAnonymous] // This endpoint should be called by SMS provider webhooks
        public async Task<ActionResult<ApiResponse<KeywordActivityDto>>> ProcessInboundKeyword([FromBody] InboundSmsDto dto)
        {
            try
            {
                var activity = await _keywordService.ProcessInboundKeywordAsync(dto.PhoneNumber, dto.Message);
                return Ok(ApiResponse<KeywordActivityDto>.SuccessResponse(activity, "Inbound keyword processed successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing inbound keyword");
                return BadRequest(ApiResponse<KeywordActivityDto>.ErrorResponse("Failed to process inbound keyword", new List<string> { ex.Message }));
            }
        }
    }

    // DTO for inbound SMS webhook
    public class InboundSmsDto
    {
        public string PhoneNumber { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }
}
