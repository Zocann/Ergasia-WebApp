using Ergasia_WebApp.ApiRepositories.Interfaces;
using Ergasia_WebApp.Data;
using Ergasia_WebApp.DTOs.Job;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Ergasia_WebApp.Pages.Jobs;

public class Application(IJobApiRepository jobApiRepository) : PageModel
{
    private ClientData _clientData = new (new HttpContextAccessor());
    
    [BindProperty(SupportsGet = true)] 
    public required string JobId { get; set; }
    public required JobDto JobDto { get; set; }
    public WorkerJobDto? WorkerJob { get; set; }
    public JobRequestDto? JobRequest { get; set; }

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
        WorkerJob = await jobApiRepository.GetWorkerJobAsync(JobId, _clientData.Id, _clientData.AccessToken);
        JobRequest = await jobApiRepository.GetJobRequestAsync(JobId, _clientData.Id, _clientData.AccessToken);

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(string message)
    {
        if (_clientData.AccessToken == null) return Unauthorized();
        if (_clientData.Id == null) return RedirectToPage("/Error");
        
        var jobRequest = await jobApiRepository.PostJobRequestAsync(JobId, _clientData.Id, message, _clientData.AccessToken);

        if (jobRequest == null)
        {
            if (Response.StatusCode == 401) return Unauthorized();
            return RedirectToPage("/Error");
        }
        
        return RedirectToPage("/Jobs/Application", new {jobId = JobId});
    }
}