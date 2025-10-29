using Ergasia_WebApp.ApiRepositories.Interfaces;
using Ergasia_WebApp.DTOs.Job;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Ergasia_WebApp.Pages;

[IgnoreAntiforgeryToken]
public class IndexModel(IJobApiRepository jobApiRepository, IRatingApiRepository ratingApiRepository) : PageModel
{
    public List<JobDto> Jobs { get; set; } = [];
    public Dictionary<string, decimal?> AverageRatings { get; set; } = new();
    public string? Error;
    
    
    public async Task<IActionResult> OnGetAsync(string? error)
    {
        Dictionary<string, decimal?> allAverageRatings = new();
        var result = await jobApiRepository.GetAllUpcomingAsync();
        
        if (result != null)
        {
            if (Response.StatusCode == 401) return Unauthorized();

            var jobs = result.ToList();
            foreach (var job in jobs)
            {
                if (job.EmployerId == null || job.Id == null) break;

                var averageRating = await ratingApiRepository.GetEmployerAverageRating(job.EmployerId);
                if (averageRating == null) continue;

                allAverageRatings.Add(job.Id, averageRating);
            }

            //Take only 4 best rated employers
            AverageRatings = allAverageRatings.OrderBy(ar => ar.Value).Take(4).ToDictionary();

            Jobs = jobs.TakeWhile(j => j.Id != null && AverageRatings.ContainsKey(j.Id)).ToList();
        }

        Error = error;
        
        return Page();
    }
}