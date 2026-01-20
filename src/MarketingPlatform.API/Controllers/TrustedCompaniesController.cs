using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MarketingPlatform.Application.DTOs.Common;
using MarketingPlatform.Application.Interfaces;
using MarketingPlatform.Core.Entities;

namespace MarketingPlatform.API.Controllers
{
    [AllowAnonymous]
    [ApiController]
    [Route("api/trustedcompanies")]
    public class TrustedCompaniesController : ControllerBase
    {
        private readonly ITrustedCompanyService _trustedCompanyService;
        private readonly ILogger<TrustedCompaniesController> _logger;

        public TrustedCompaniesController(
            ITrustedCompanyService trustedCompanyService,
            ILogger<TrustedCompaniesController> logger)
        {
            _trustedCompanyService = trustedCompanyService;
            _logger = logger;
        }

        /// <summary>
        /// Get all active trusted companies for landing page
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<TrustedCompany>>>> GetTrustedCompanies()
        {
            try
            {
                var companies = await _trustedCompanyService.GetAllActiveAsync();
                var companyList = companies.ToList();

                _logger.LogInformation("Retrieved {Count} trusted companies", companyList.Count);

                return Ok(ApiResponse<List<TrustedCompany>>.SuccessResponse(companyList));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving trusted companies");
                return BadRequest(ApiResponse<List<TrustedCompany>>.ErrorResponse(
                    "Failed to retrieve trusted companies",
                    new List<string> { ex.Message }));
            }
        }

        /// <summary>
        /// Get a specific trusted company by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<TrustedCompany>>> GetById(int id)
        {
            try
            {
                var company = await _trustedCompanyService.GetByIdAsync(id);

                if (company == null)
                    return NotFound(ApiResponse<TrustedCompany>.ErrorResponse("Company not found"));

                return Ok(ApiResponse<TrustedCompany>.SuccessResponse(company));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving company {CompanyId}", id);
                return BadRequest(ApiResponse<TrustedCompany>.ErrorResponse(
                    "Failed to retrieve company",
                    new List<string> { ex.Message }));
            }
        }
    }
}
