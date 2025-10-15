using Ergasia_WebApp.DTOs.Employer;
using Ergasia_WebApp.DTOs.Worker;

namespace Ergasia_WebApp.DTOs.Rating;

public class EmployerRatingDto
{
    public required EmployerDto EmployerDto { get; set; }
    public required WorkerDto WorkerDto { get; set; }
    public required int NumericalRating { get; set; }
    public string? VerbalRating { get; set; }
    public required DateTime Date { get; set; }
}