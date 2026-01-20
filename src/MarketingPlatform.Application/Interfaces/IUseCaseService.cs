using MarketingPlatform.Core.Entities;

namespace MarketingPlatform.Application.Interfaces
{
    public interface IUseCaseService
    {
        Task<IEnumerable<UseCase>> GetAllActiveAsync();
        Task<UseCase?> GetByIdAsync(int id);
        Task<IEnumerable<UseCase>> GetByIndustryAsync(string industry);
    }
}
