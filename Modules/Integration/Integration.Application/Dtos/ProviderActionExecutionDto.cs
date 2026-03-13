namespace Integration.Application.Dtos;

public sealed class ProviderActionExecutionDto
{
    public Guid JobId { get; set; }
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? ExternalJobId { get; set; }
}
