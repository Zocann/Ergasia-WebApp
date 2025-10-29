using Ergasia_WebApp.ApiRepositories.Interfaces;
using Ergasia_WebApp.Data;
using Ergasia_WebApp.DTOs.Job;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Ergasia_WebApp.Pages.Jobs;

public class Information(IJobApiRepository jobApiRepository, IRatingApiRepository ratingApiRepository) : PageModel
{
    private ClientData _clientData = new(new HttpContextAccessor());
    
    [BindProperty(SupportsGet = true)]
    public required string JobId { get; set; }
    public required JobDto JobDto { get; set; }
    public required List<WorkerJobDto>? WorkerJobs { get; set; }
    public decimal? AverageRating { get; set; }
    
    public async Task<IActionResult> OnGetAsync()
    {
        if (_clientData.AccessToken == null) return Unauthorized();
        
        var job = await jobApiRepository.GetAsync(JobId, _clientData.AccessToken);
        if (job?.Id == null)
        {
            if (Response.StatusCode == 401) return Unauthorized();
            return RedirectToPage("/Error");
        }
        JobDto = job;

        var workerJobs = await jobApiRepository.GetWorkerJobsByJobIdAsync(job.Id, _clientData.AccessToken);
        if (workerJobs != null)
        {
            workerJobs.RemoveAll(j => j.NumericalRating == null);
            WorkerJobs = workerJobs;
        }
        
        AverageRating = await ratingApiRepository.GetJobAverageRating(job.Id);
        
        return Page();
    }
}