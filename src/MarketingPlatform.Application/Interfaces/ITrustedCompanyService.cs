using MarketingPlatform.Core.Entities;

namespace MarketingPlatform.Application.Interfaces
{
    public interface ITrustedCompanyService
    {
        Task<IEnumerable<TrustedCompany>> GetAllActiveAsync();
        Task<TrustedCompany?> GetByIdAsync(int id);
    }
}
