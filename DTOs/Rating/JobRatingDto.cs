namespace Ergasia_WebApp.DTOs.Rating;

public struct JobRatingDto(string jobId, string workerId, int numericalRating, string? verbalRating)
{
    public string WorkerId { get; set; } = workerId;
    public string JobId { get; set; } = jobId;
    public int NumericalRating { get; set; } = numericalRating;
    public string? VerbalRating { get; set; } = verbalRating;
}