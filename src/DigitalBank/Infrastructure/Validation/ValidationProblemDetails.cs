namespace DigitalBank.Infrastructure.Validation;

public class ValidationProblemDetails
{
    public string Title { get; set; } = string.Empty;
    public int Status { get; set; }
    public string? Detail { get; set; }
    public Dictionary<string, List<string>> Errors { get; set; } = new();
}