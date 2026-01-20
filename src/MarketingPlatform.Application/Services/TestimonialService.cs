using MarketingPlatform.Application.Interfaces;
using MarketingPlatform.Core.Entities;
using MarketingPlatform.Core.Interfaces.Repositories;
using Microsoft.Extensions.Logging;

namespace MarketingPlatform.Application.Services
{
    public class TestimonialService : ITestimonialService
    {
        private readonly IRepository<Testimonial> _repository;
        private readonly ILogger<TestimonialService> _logger;

        public TestimonialService(
            IRepository<Testimonial> repository,
            ILogger<TestimonialService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<IEnumerable<Testimonial>> GetAllActiveAsync()
        {
            var testimonials = await _repository.FindAsync(t => t.IsActive && !t.IsDeleted);
            return testimonials.OrderBy(t => t.DisplayOrder);
        }

        public async Task<Testimonial?> GetByIdAsync(int id)
        {
            var testimonial = await _repository.FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted);
            return testimonial;
        }
    }
}
