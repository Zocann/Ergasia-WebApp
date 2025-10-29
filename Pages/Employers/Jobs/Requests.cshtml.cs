using Ergasia_WebApp.ApiRepositories.Interfaces;
using Ergasia_WebApp.Data;
using Ergasia_WebApp.DTOs.Job;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Ergasia_WebApp.Pages.Employers.Jobs;

public class Requests(IJobApiRepository jobApiRepository) : PageModel
{
    private ClientData _clientData = new (new HttpContextAccessor());
    
    [BindProperty(SupportsGet = true)]
    public required string JobId { get; set; }

    public required JobDto Job { get; set; }
    public List<JobRequestDto>? JobRequests { get; set; } = [];
    public string? Error { get; set; }
    
    public async Task<IActionResult> OnGetAsync(string? error)
    { 
        if (_clientData.AccessToken == null) return Unauthorized();
        if (_clientData.Id == null) return RedirectToPage("/Error");
       
        var job = await jobApiRepository.GetAsync(JobId, _clientData.AccessToken);
        if (job == null)
        {
            if (Response.StatusCode == 401) return Unauthorized();
            return RedirectToPage("Error");
        }
        Job = job;
        
        JobRequests = await jobApiRepository.GetJobRequestsByJobIdAsync(JobId, _clientData.Id, _clientData.AccessToken);
        
        Error = error;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(string workerId, string method)
    {
        if (_clientData.AccessToken == null) return Unauthorized();
        if (_clientData.Id == null) return RedirectToPage("/Error");

        switch (method)
        {
            case "post":
                var job = await jobApiRepository.GetAsync(JobId, _clientData.AccessToken);
                if (job == null)
                {
                    if (Response.StatusCode == 401) return Unauthorized();
                    return RedirectToPage("Error");
                }
                var workspots = await jobApiRepository.GetAvailableWorkSpotsAsync(JobId, _clientData.AccessToken);
                if (workspots <= 0) return RedirectToAction(nameof(OnGetAsync), new { error = "Job is at full capacity. Update job information to hire more workers"});
                
                var workerJob = await jobApiRepository.PostWorkerJobAsync(JobId, _clientData.Id, workerId, _clientData.AccessToken);

                if (workerJob == null)
                {
                    if (Response.StatusCode == 401) return Unauthorized();
                    return RedirectToPage("/Error");
                }

                return RedirectToAction(nameof(OnGetAsync));
            
            case "delete":
                var deleted = await jobApiRepository.DeleteJobRequest(JobId, _clientData.Id, workerId, _clientData.AccessToken);
                if (!deleted)
                {
                    if (Response.StatusCode == 401) return Unauthorized();
                    return RedirectToPage("/Error");
                }

                return RedirectToAction(nameof(OnGetAsync));
            
            default:
                return RedirectToPage("/Error");
        }
    }
}