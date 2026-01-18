using MarketingPlatform.Core.Enums;

namespace MarketingPlatform.Application.DTOs.Keyword
{
    public class UpdateKeywordDto
    {
        public string? Description { get; set; }
        public KeywordStatus Status { get; set; }
        public string? ResponseMessage { get; set; }
    }
}
