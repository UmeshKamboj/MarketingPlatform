using MarketingPlatform.Application.DTOs.Auth;

namespace MarketingPlatform.Application.Interfaces
{
    public interface IOAuth2Service
    {
        /// <summary>
        /// Get authorization URL for OAuth2 flow
        /// </summary>
        Task<string> GetAuthorizationUrlAsync(string providerName, string? redirectUri = null, string? state = null);
        
        /// <summary>
        /// Handle OAuth2 callback and exchange code for tokens
        /// </summary>
        Task<AuthResponseDto> HandleCallbackAsync(string providerName, OAuth2CallbackDto callback);
        
        /// <summary>
        /// Get user information from external provider
        /// </summary>
        Task<ExternalUserInfoDto> GetUserInfoAsync(string providerName, string accessToken);
        
        /// <summary>
        /// Link external account to existing user
        /// </summary>
        Task<bool> LinkExternalAccountAsync(string userId, string providerName, string providerUserId, OAuth2TokenResponseDto tokens);
        
        /// <summary>
        /// Unlink external account from user
        /// </summary>
        Task<bool> UnlinkExternalAccountAsync(string userId, string providerName);
        
        /// <summary>
        /// Refresh external provider access token
        /// </summary>
        Task<OAuth2TokenResponseDto> RefreshExternalTokenAsync(string userId, string providerName);
        
        /// <summary>
        /// Get all enabled external authentication providers
        /// </summary>
        Task<IEnumerable<ExternalAuthProviderDto>> GetEnabledProvidersAsync();
    }
}
