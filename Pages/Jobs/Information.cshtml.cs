using System.Net;
using Ergasia_WebApp.Data;
using Ergasia_WebApp.DTOs.Job;
using Ergasia_WebApp.Services.Model.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Ergasia_WebApp.Pages.Jobs;

public class Information(IJobService jobService, IJobRatingService jobRatingService,
    IWorkerJobService workerJobService) : PageModel
{
    private ClientData _clientData = new(new HttpContextAccessor());
    
    [BindProperty(SupportsGet = true)]
    public required string JobId { get; set; }
    public required JobDto JobDto { get; set; }
    public required List<WorkerJobDto>? WorkerJobs { get; set; }
    public float? AverageRating { get; set; }
    
    public async Task<IActionResult> OnGetAsync()
    {
        if (_clientData.AccessToken == null) return Unauthorized();
        
        var serviceResult = await jobService.GetAsync(JobId, _clientData.AccessToken);
        if (! serviceResult.IsSuccess)
        {
            if (serviceResult.StatusCode == HttpStatusCode.Unauthorized) return Unauthorized();
            return RedirectToPage("/Error");
        }
        JobDto = serviceResult.Data;
        if (JobDto.Id == null) return Page();
        
        var workerJobServiceResult = await workerJobService.GetByJobIdAsync(JobDto.Id, _clientData.AccessToken);
        if (workerJobServiceResult.IsSuccess)
            WorkerJobs = RemoveWorkerJobsWithoutRating(workerJobServiceResult.Data);

        var averageRatingServiceResult = await jobRatingService.GetAverageRatingAsync(JobDto.Id);
        AverageRating = averageRatingServiceResult.Data;

        return Page();
    }

    private static List<WorkerJobDto> RemoveWorkerJobsWithoutRating(IEnumerable<WorkerJobDto> workerJobs)
    {
        var workerjobsList = workerJobs.ToList();
        workerjobsList.RemoveAll(wj => wj.NumericalRating == null);
        return workerjobsList;
    }
}