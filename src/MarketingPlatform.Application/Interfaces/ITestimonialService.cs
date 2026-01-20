using MarketingPlatform.Core.Entities;

namespace MarketingPlatform.Application.Interfaces
{
    public interface ITestimonialService
    {
        Task<IEnumerable<Testimonial>> GetAllActiveAsync();
        Task<Testimonial?> GetByIdAsync(int id);
    }
}
