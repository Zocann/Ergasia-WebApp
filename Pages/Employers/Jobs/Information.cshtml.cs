using Ergasia_WebApp.ApiRepositories.Interfaces;
using Ergasia_WebApp.Data;
using Ergasia_WebApp.DTOs.Job;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Ergasia_WebApp.Pages.Employers.Jobs;
public class Information(IJobApiRepository jobApiRepository) : PageModel
{
    private ClientData _clientData = new (new HttpContextAccessor());
    
    [BindProperty(SupportsGet = true)]
    public required string JobId { get; set; }
    
    
    public required JobDto JobDto { get; set; }
    public List<WorkerJobDto>? WorkerJobs { get; set; }

    public required int WorkSpots { get; set; }
    
    public async Task<IActionResult> OnGetAsync()
    {
        if (_clientData.AccessToken == null) return Unauthorized();
        if (_clientData.Id == null) return RedirectToPage("/Error");

        var job = await jobApiRepository.GetAsync(JobId, _clientData.AccessToken);
        if (job?.Id == null)
        {
            if (Response.StatusCode == 401) return Unauthorized();
            return RedirectToPage("/Error");
        }
        JobDto = job;
        
        var workSpots = await jobApiRepository.GetAvailableWorkSpotsAsync(job.Id, _clientData.AccessToken);
        if (workSpots == null) return RedirectToPage("/Error");
        WorkSpots = (int)workSpots;
        
        WorkerJobs = (await jobApiRepository.GetWorkerJobsByJobIdAsync(job.Id, _clientData.AccessToken));
        
        return Page();
    }
}