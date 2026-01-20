using MarketingPlatform.Application.Interfaces;
using MarketingPlatform.Core.Entities;
using MarketingPlatform.Core.Interfaces.Repositories;
using Microsoft.Extensions.Logging;

namespace MarketingPlatform.Application.Services
{
    public class TrustedCompanyService : ITrustedCompanyService
    {
        private readonly IRepository<TrustedCompany> _repository;
        private readonly ILogger<TrustedCompanyService> _logger;

        public TrustedCompanyService(
            IRepository<TrustedCompany> repository,
            ILogger<TrustedCompanyService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<IEnumerable<TrustedCompany>> GetAllActiveAsync()
        {
            var companies = await _repository.FindAsync(c => c.IsActive && !c.IsDeleted);
            return companies.OrderBy(c => c.DisplayOrder);
        }

        public async Task<TrustedCompany?> GetByIdAsync(int id)
        {
            var company = await _repository.FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);
            return company;
        }
    }
}
