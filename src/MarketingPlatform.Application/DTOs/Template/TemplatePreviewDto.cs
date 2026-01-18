namespace MarketingPlatform.Application.DTOs.Template
{
    public class TemplatePreviewDto
    {
        public string? Subject { get; set; }
        public string MessageBody { get; set; } = string.Empty;
        public string? HTMLContent { get; set; }
        public List<string>? MediaUrls { get; set; }
        public List<string> MissingVariables { get; set; } = new();
    }
}
