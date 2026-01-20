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
    [Route("api/usecases")]
    public class UseCasesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UseCasesController> _logger;

        public UseCasesController(
            ApplicationDbContext context,
            ILogger<UseCasesController> logger)
        {
            _context = context;
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
                var useCases = await _context.UseCases
                    .Where(u => u.IsActive && !u.IsDeleted)
                    .OrderBy(u => u.DisplayOrder)
                    .ToListAsync();

                _logger.LogInformation("Retrieved {Count} use cases", useCases.Count);

                return Ok(ApiResponse<List<UseCase>>.SuccessResponse(useCases));
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
                var useCase = await _context.UseCases
                    .FirstOrDefaultAsync(u => u.Id == id && !u.IsDeleted);

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
                var useCases = await _context.UseCases
                    .Where(u => u.IsActive && !u.IsDeleted && u.Industry == industry)
                    .OrderBy(u => u.DisplayOrder)
                    .ToListAsync();

                return Ok(ApiResponse<List<UseCase>>.SuccessResponse(useCases));
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
