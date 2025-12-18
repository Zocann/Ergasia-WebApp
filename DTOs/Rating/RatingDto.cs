namespace Ergasia_WebApp.DTOs.Rating;

public struct RatingDto(string employerId, string workerId, int numericalRating, string? verbalRating)
{
    public string EmployerId { get; set; } = employerId;
    public string WorkerId { get; set; } = workerId;
    public int NumericalRating { get; set; } = numericalRating;
    public string? VerbalRating { get; set; } = verbalRating;
}