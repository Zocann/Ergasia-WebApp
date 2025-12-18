using System.Net;
using Ergasia_WebApp.DTOs.Job;
using Ergasia_WebApp.DTOs.Worker;
using Ergasia_WebApp.Services.Model.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Ergasia_WebApp.Pages;

[IgnoreAntiforgeryToken]
public class IndexModel(IJobService jobApiService, IEmployerRatingService employerRatingApiService) : PageModel
{
    public List<JobDto> Jobs { get; set; } = [];
    public Dictionary<string, float> AverageRatings { get; set; } = new();
    public string? Error;
    
    
    public async Task<IActionResult> OnGetAsync(string? error)
    {
        Dictionary<string, float> allAverageRatings = new();
        var serviceResult = await jobApiService.GetAllUpcomingAsync();

        if (serviceResult.StatusCode == HttpStatusCode.Unauthorized) return Unauthorized();
        
        if (serviceResult.IsSuccess)
        {
            var jobs = serviceResult.Data.ToList();
            foreach (var job in jobs)
            {
                if (job.EmployerId == null || job.Id == null) continue;
                var averageRating = await GetAverageRatingAsync(job.EmployerId);
                if (averageRating != null) allAverageRatings.Add(job.Id, (float)averageRating);
            }
            
            const int numberOfRatings = 4;
            AverageRatings = GetNumberOfBestRatings(numberOfRatings, allAverageRatings);

            
            foreach (var averageRating in AverageRatings)
            {
                var job = GetJobByJobId(jobs, averageRating.Key);
                if (job != null) Jobs.Add(job);
            }
        }

        Error = error;
        return Page();
    }

    private async Task<float?> GetAverageRatingAsync(string employerId)
    {
        var serviceResult = await employerRatingApiService.GetAverageRatingAsync(employerId);
        return serviceResult.Data;
    }

    private static Dictionary<string, float> GetNumberOfBestRatings(int numberOfRatings, Dictionary<string, float> allAverageRatings)
    {
        return allAverageRatings.OrderByDescending(ar => ar.Value).Take(numberOfRatings).ToDictionary();
    }

    private static JobDto? GetJobByJobId(List<JobDto> jobs, string jobId)
    {
        return jobs.FirstOrDefault(j => j.Id == jobId);
    }
}