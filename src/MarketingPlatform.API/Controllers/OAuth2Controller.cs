using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MarketingPlatform.Application.DTOs.Auth;
using MarketingPlatform.Application.Interfaces;
using MarketingPlatform.Core.Interfaces.Repositories;

namespace MarketingPlatform.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OAuth2Controller : ControllerBase
    {
        private readonly IOAuth2Service _oauth2Service;
        private readonly IExternalAuthProviderRepository _providerRepository;
        private readonly ILogger<OAuth2Controller> _logger;

        public OAuth2Controller(
            IOAuth2Service oauth2Service,
            IExternalAuthProviderRepository providerRepository,
            ILogger<OAuth2Controller> logger)
        {
            _oauth2Service = oauth2Service;
            _providerRepository = providerRepository;
            _logger = logger;
        }

        /// <summary>
        /// Get list of enabled OAuth2/SSO providers
        /// </summary>
        [HttpGet("providers")]
        [AllowAnonymous]
        public async Task<IActionResult> GetProviders()
        {
            try
            {
                var providers = await _oauth2Service.GetEnabledProvidersAsync();
                return Ok(providers);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting OAuth2 providers");
                return StatusCode(500, new { message = "Error retrieving providers" });
            }
        }

        /// <summary>
        /// Get authorization URL to initiate OAuth2 flow
        /// </summary>
        [HttpGet("authorize/{providerName}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAuthorizationUrl(string providerName, [FromQuery] string? redirectUri = null)
        {
            try
            {
                var state = Guid.NewGuid().ToString();
                var authUrl = await _oauth2Service.GetAuthorizationUrlAsync(providerName, redirectUri, state);
                
                return Ok(new { authorizationUrl = authUrl, state });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating authorization URL for {Provider}", providerName);
                return StatusCode(500, new { message = "Error generating authorization URL" });
            }
        }

        /// <summary>
        /// Handle OAuth2 callback and complete authentication
        /// </summary>
        [HttpPost("callback/{providerName}")]
        [AllowAnonymous]
        public async Task<IActionResult> HandleCallback(string providerName, [FromBody] OAuth2CallbackDto callback)
        {
            try
            {
                var authResponse = await _oauth2Service.HandleCallbackAsync(providerName, callback);
                return Ok(authResponse);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling OAuth2 callback for {Provider}", providerName);
                return StatusCode(500, new { message = "Authentication failed" });
            }
        }

        /// <summary>
        /// Link an external account to the current user
        /// </summary>
        [HttpPost("link/{providerName}")]
        [Authorize]
        public async Task<IActionResult> LinkExternalAccount(
            string providerName, 
            [FromBody] OAuth2CallbackDto callback)
        {
            try
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }

                // Handle callback to get tokens
                var authResponse = await _oauth2Service.HandleCallbackAsync(providerName, callback);
                
                return Ok(new { message = "External account linked successfully", providerName });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error linking external account for {Provider}", providerName);
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Unlink an external account from the current user
        /// </summary>
        [HttpDelete("unlink/{providerName}")]
        [Authorize]
        public async Task<IActionResult> UnlinkExternalAccount(string providerName)
        {
            try
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }

                var result = await _oauth2Service.UnlinkExternalAccountAsync(userId, providerName);
                
                if (result)
                {
                    return Ok(new { message = "External account unlinked successfully" });
                }
                
                return NotFound(new { message = "External account not found" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error unlinking external account for {Provider}", providerName);
                return StatusCode(500, new { message = "Error unlinking account" });
            }
        }

        /// <summary>
        /// Create or update OAuth2 provider configuration (Admin only)
        /// </summary>
        [HttpPost("admin/providers")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateProvider([FromBody] CreateExternalAuthProviderDto dto)
        {
            try
            {
                var provider = new Core.Entities.ExternalAuthProvider
                {
                    Name = dto.Name,
                    DisplayName = dto.DisplayName,
                    ProviderType = dto.ProviderType,
                    ClientId = dto.ClientId,
                    ClientSecret = dto.ClientSecret,
                    Authority = dto.Authority,
                    TenantId = dto.TenantId,
                    Domain = dto.Domain,
                    Region = dto.Region,
                    UserPoolId = dto.UserPoolId,
                    CallbackPath = dto.CallbackPath,
                    Scopes = dto.Scopes,
                    IsEnabled = dto.IsEnabled,
                    ConfigurationJson = dto.ConfigurationJson,
                    CreatedAt = DateTime.UtcNow
                };

                await _providerRepository.AddAsync(provider);
                await _providerRepository.SaveChangesAsync();

                return Ok(new { message = "Provider created successfully", providerId = provider.Id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating OAuth2 provider");
                return StatusCode(500, new { message = "Error creating provider" });
            }
        }

        /// <summary>
        /// Update OAuth2 provider configuration (Admin only)
        /// </summary>
        [HttpPut("admin/providers/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateProvider(int id, [FromBody] UpdateExternalAuthProviderDto dto)
        {
            try
            {
                var provider = await _providerRepository.GetByIdAsync(id);
                if (provider == null)
                {
                    return NotFound(new { message = "Provider not found" });
                }

                if (dto.DisplayName != null) provider.DisplayName = dto.DisplayName;
                if (dto.ClientId != null) provider.ClientId = dto.ClientId;
                if (dto.ClientSecret != null) provider.ClientSecret = dto.ClientSecret;
                if (dto.Authority != null) provider.Authority = dto.Authority;
                if (dto.TenantId != null) provider.TenantId = dto.TenantId;
                if (dto.Domain != null) provider.Domain = dto.Domain;
                if (dto.Region != null) provider.Region = dto.Region;
                if (dto.UserPoolId != null) provider.UserPoolId = dto.UserPoolId;
                if (dto.CallbackPath != null) provider.CallbackPath = dto.CallbackPath;
                if (dto.Scopes != null) provider.Scopes = dto.Scopes;
                if (dto.IsEnabled.HasValue) provider.IsEnabled = dto.IsEnabled.Value;
                if (dto.ConfigurationJson != null) provider.ConfigurationJson = dto.ConfigurationJson;
                
                provider.UpdatedAt = DateTime.UtcNow;

                await _providerRepository.UpdateAsync(provider);
                await _providerRepository.SaveChangesAsync();

                return Ok(new { message = "Provider updated successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating OAuth2 provider {Id}", id);
                return StatusCode(500, new { message = "Error updating provider" });
            }
        }

        /// <summary>
        /// Get OAuth2 provider by ID (Admin only)
        /// </summary>
        [HttpGet("admin/providers/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetProvider(int id)
        {
            try
            {
                var provider = await _providerRepository.GetByIdAsync(id);
                if (provider == null)
                {
                    return NotFound(new { message = "Provider not found" });
                }

                // Don't expose sensitive data
                return Ok(new
                {
                    provider.Id,
                    provider.Name,
                    provider.DisplayName,
                    provider.ProviderType,
                    provider.Authority,
                    provider.TenantId,
                    provider.Domain,
                    provider.Region,
                    provider.UserPoolId,
                    provider.CallbackPath,
                    provider.Scopes,
                    provider.IsEnabled,
                    provider.IsDefault,
                    provider.CreatedAt,
                    provider.UpdatedAt
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting OAuth2 provider {Id}", id);
                return StatusCode(500, new { message = "Error retrieving provider" });
            }
        }

        /// <summary>
        /// Delete OAuth2 provider (Admin only)
        /// </summary>
        [HttpDelete("admin/providers/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteProvider(int id)
        {
            try
            {
                var provider = await _providerRepository.GetByIdAsync(id);
                if (provider == null)
                {
                    return NotFound(new { message = "Provider not found" });
                }

                await _providerRepository.DeleteAsync(provider);
                await _providerRepository.SaveChangesAsync();

                return Ok(new { message = "Provider deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting OAuth2 provider {Id}", id);
                return StatusCode(500, new { message = "Error deleting provider" });
            }
        }
    }
}
