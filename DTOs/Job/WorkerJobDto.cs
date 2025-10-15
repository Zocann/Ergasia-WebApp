using Ergasia_WebApp.DTOs.Worker;

namespace Ergasia_WebApp.DTOs.Job;

public class WorkerJobDto
{
    public required WorkerDto WorkerDto { get; set; }
    public required JobDto JobDto { get; set; }
    public int? NumericalRating { get; set; }
    public string? VerbalRating { get; set; }
    public DateTime? DateOfRating { get; set; }
}