using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MarketingPlatform.Application.DTOs.Common;
using MarketingPlatform.Application.Interfaces;
using MarketingPlatform.Core.Entities;

namespace MarketingPlatform.API.Controllers
{
    [AllowAnonymous]
    [ApiController]
    [Route("api/usecases")]
    public class UseCasesController : ControllerBase
    {
        private readonly IUseCaseService _useCaseService;
        private readonly ILogger<UseCasesController> _logger;

        public UseCasesController(
            IUseCaseService useCaseService,
            ILogger<UseCasesController> logger)
        {
            _useCaseService = useCaseService;
            _logger = logger;
        }

        /// <summary>
        /// Get all active use cases for landing page
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<UseCase>>>> GetUseCases()
        {
            try
            {
                var useCases = await _useCaseService.GetAllActiveAsync();
                var useCaseList = useCases.ToList();

                _logger.LogInformation("Retrieved {Count} use cases", useCaseList.Count);

                return Ok(ApiResponse<List<UseCase>>.SuccessResponse(useCaseList));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving use cases");
                return BadRequest(ApiResponse<List<UseCase>>.ErrorResponse(
                    "Failed to retrieve use cases",
                    new List<string> { ex.Message }));
            }
        }

        /// <summary>
        /// Get a specific use case by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<UseCase>>> GetById(int id)
        {
            try
            {
                var useCase = await _useCaseService.GetByIdAsync(id);

                if (useCase == null)
                    return NotFound(ApiResponse<UseCase>.ErrorResponse("Use case not found"));

                return Ok(ApiResponse<UseCase>.SuccessResponse(useCase));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving use case {UseCaseId}", id);
                return BadRequest(ApiResponse<UseCase>.ErrorResponse(
                    "Failed to retrieve use case",
                    new List<string> { ex.Message }));
            }
        }

        /// <summary>
        /// Get use cases by industry
        /// </summary>
        [HttpGet("industry/{industry}")]
        public async Task<ActionResult<ApiResponse<List<UseCase>>>> GetByIndustry(string industry)
        {
            try
            {
                var useCases = await _useCaseService.GetByIndustryAsync(industry);
                var useCaseList = useCases.ToList();

                return Ok(ApiResponse<List<UseCase>>.SuccessResponse(useCaseList));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving use cases for industry {Industry}", industry);
                return BadRequest(ApiResponse<List<UseCase>>.ErrorResponse(
                    "Failed to retrieve use cases",
                    new List<string> { ex.Message }));
            }
        }
    }
}
