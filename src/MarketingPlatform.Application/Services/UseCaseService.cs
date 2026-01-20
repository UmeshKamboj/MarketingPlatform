using MarketingPlatform.Application.Interfaces;
using MarketingPlatform.Core.Entities;
using MarketingPlatform.Core.Interfaces.Repositories;
using Microsoft.Extensions.Logging;

namespace MarketingPlatform.Application.Services
{
    public class UseCaseService : IUseCaseService
    {
        private readonly IRepository<UseCase> _repository;
        private readonly ILogger<UseCaseService> _logger;

        public UseCaseService(
            IRepository<UseCase> repository,
            ILogger<UseCaseService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<IEnumerable<UseCase>> GetAllActiveAsync()
        {
            var useCases = await _repository.FindAsync(u => u.IsActive && !u.IsDeleted);
            return useCases.OrderBy(u => u.DisplayOrder);
        }

        public async Task<UseCase?> GetByIdAsync(int id)
        {
            var useCase = await _repository.FirstOrDefaultAsync(u => u.Id == id && !u.IsDeleted);
            return useCase;
        }

        public async Task<IEnumerable<UseCase>> GetByIndustryAsync(string industry)
        {
            var useCases = await _repository.FindAsync(u => 
                u.IsActive && !u.IsDeleted && u.Industry == industry);
            return useCases.OrderBy(u => u.DisplayOrder);
        }
    }
}
