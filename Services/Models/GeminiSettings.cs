namespace Services.Models
{
    /// <summary>
    /// Gemini AI configuration settings
    /// </summary>
    public class GeminiSettings
    {
        public string ApiKey { get; set; } = string.Empty;
        public string Model { get; set; } = "gemini-pro";
        public string ApiEndpoint { get; set; } = string.Empty;
    }
}
