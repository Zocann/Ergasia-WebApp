using Ergasia_WebApp.ApiRepositories.Interfaces;
using Ergasia_WebApp.Data;
using Ergasia_WebApp.DTOs.Job;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Ergasia_WebApp.Pages.Jobs;

public class Index(IJobApiRepository jobApiRepository, IRatingApiRepository ratingApiRepository) : PageModel
{
    private ClientData _clientData = new(new HttpContextAccessor());

    public required List<JobDto> Jobs { get; set; } = [];
    
    public Dictionary<string, decimal> AverageRating { get; set; } = new();

    public async Task<IActionResult> OnGet()
    {
        if (!_clientData.GetAccessToken()) return Unauthorized();

        var jobs = await jobApiRepository.GetAllUpcomingAsync();

        if (jobs == null)
        {
            if (Response.StatusCode == 401) return Unauthorized();
            return RedirectToPage("/Error");
        }
        
        foreach (var job in jobs)
        {
            if (job.Id == null || job.EmployerId == null) continue;
            if (await jobApiRepository.GetAvailableWorkSpotsAsync(job.Id, _clientData.AccessToken) <= 0) continue;
            Jobs.Add(job);
            
            var averageRating = await ratingApiRepository.GetEmployerAverageRating(job.EmployerId);
            if (averageRating != null) AverageRating.Add(job.Id, (decimal)averageRating);
        }
        Jobs = Jobs.OrderBy(j => j.DateOfBegin).ToList();
        
        return Page();
    }
}