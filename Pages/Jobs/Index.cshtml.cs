using System.Net;
using Ergasia_WebApp.Data;
using Ergasia_WebApp.DTOs.Job;
using Ergasia_WebApp.Services.Model.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Ergasia_WebApp.Pages.Jobs;

public class Index(IJobService jobService,IWorkerJobService workerJobService,
    IEmployerRatingService employerRatingService) : PageModel
{
    private ClientData _clientData = new(new HttpContextAccessor());

    public required List<JobDto> Jobs { get; set; } = [];
    
    public Dictionary<string, decimal> AverageRating { get; set; } = new();

    public async Task<IActionResult> OnGet()
    {
        if (_clientData.AccessToken == null) return Unauthorized();

        var serviceResult = await jobService.GetAllUpcomingAsync();

        if (! serviceResult.IsSuccess)
        {
            if (serviceResult.StatusCode == HttpStatusCode.Unauthorized) return Unauthorized();
            return RedirectToPage("/Error");
        }
        
        foreach (var job in serviceResult.Data)
        {
            if (job.Id == null || job.EmployerId == null) continue;
            
            var workSpots = await GetAvailableWorkSpotsAsync(job.Id, _clientData.AccessToken);
            if (workSpots is null or <= 0) continue;
            
            Jobs.Add(job);

            var averageRating = await GetEmployersAverageRatingAsync(job.EmployerId);
            if (averageRating != null) AverageRating.Add(job.Id, (decimal)averageRating);
        }

        Jobs = OrderJobsByDate(Jobs);
        return Page();
    }

    private async Task<int?> GetAvailableWorkSpotsAsync(string jobId, string accessToken)
    {
        var serviceResult = await workerJobService.GetAvailableWorkSpotsAsync(jobId, accessToken);
        return serviceResult.Data;
    }

    private async Task<float?> GetEmployersAverageRatingAsync(string employerId)
    {
        var serviceResult = await employerRatingService.GetAverageRatingAsync(employerId);
        return serviceResult.Data;
    }

    private List<JobDto> OrderJobsByDate(List<JobDto> jobs)
    {
        return Jobs.OrderBy(j => j.DateOfBegin).ToList();
    }
}