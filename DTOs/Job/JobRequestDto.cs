using Ergasia_WebApp.DTOs.Worker;

namespace Ergasia_WebApp.DTOs.Job;

public class JobRequestDto
{
    public required WorkerDto WorkerDto { get; set; }
    public required JobDto JobDto { get; set; }
    public required DateTime ExpirationDate { get; set; }
    public string? Message { get; set; }
}