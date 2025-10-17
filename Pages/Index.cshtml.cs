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

        var result = await jobApiRepository.GetAllUpcomingAsync();

        if (result != null)
        {
            if (Response.StatusCode == 401) return Unauthorized();

            var jobs = result.ToList();
            jobs.RemoveAll(j => j == null);

            foreach (var job in jobs)
            {
                if (job?.EmployerId == null || job.Id == null) break;

                var averageRating = await ratingApiRepository.GetEmployerAverageRating(job.EmployerId);
                if (averageRating == null) continue;

                AverageRatings.Add(job.Id, averageRating);
            }

            //Take only 6 best rated employers
            var ordered = AverageRatings.OrderBy(ar => ar.Value).Take(4).ToDictionary();

            Jobs = jobs.TakeWhile(j => AverageRatings.ContainsKey(j!.Id)).ToList();
        }

        Error = error;
        
        return Page();
    }
}