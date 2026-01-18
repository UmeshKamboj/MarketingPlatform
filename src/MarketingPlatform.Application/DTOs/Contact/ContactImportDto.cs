namespace MarketingPlatform.Application.DTOs.Contact
{
    public class ContactImportDto
    {
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Country { get; set; }
        public string? City { get; set; }
        public string? PostalCode { get; set; }
    }

    public class ContactImportResultDto
    {
        public int TotalRows { get; set; }
        public int SuccessCount { get; set; }
        public int FailureCount { get; set; }
        public int DuplicateCount { get; set; }
        public List<string> Errors { get; set; } = new();
    }
}
