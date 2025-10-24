using Ergasia_WebApp.ApiRepositories.Interfaces;
using Ergasia_WebApp.Data;
using Ergasia_WebApp.DTOs.Job;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Ergasia_WebApp.Pages.Employers.Jobs;

public class Index(IJobApiRepository jobApiRepository) : PageModel
{
    private ClientData _clientData = new(new HttpContextAccessor());

    public required List<JobDto> Jobs { get; set; } = [];
    public required List<JobDto> FinishedJobs { get; set; } = [];
    
    public Dictionary<string, int> JobRequestCount { get; set; } = new();
    public Dictionary<string, int> WorkerJobsCount { get; set; } = new();
    
    public async Task<IActionResult> OnGetAsync()
    {
        if (!_clientData.GetId()) return RedirectToPage("/Error");
        if (!_clientData.GetAccessToken()) return Unauthorized();

        var jobs = await jobApiRepository.GetFromEmployerAsync(_clientData.Id, _clientData.AccessToken);
        if (jobs == null)
        {
            if (Response.StatusCode == 401) return Unauthorized();
            return RedirectToPage("/Error");
        }
        
        FinishedJobs = jobs.Where(j => j.DateOfBegin.AddDays(j.Duration) < DateTime.UtcNow).OrderByDescending(j => j.DateOfBegin).ToList();
        Jobs = jobs.Where(j => j.DateOfBegin.AddDays(j.Duration) > DateTime.UtcNow).OrderBy(j => j.DateOfBegin).ToList();
            
        foreach (var job in Jobs)
        {
            if (job.Id == null) continue;
            
            var workerJobs = await jobApiRepository.GetWorkerJobsByJobIdAsync(job.Id, _clientData.AccessToken);
            if (workerJobs != null) WorkerJobsCount.Add(job.Id, workerJobs.Count);
            
            var jobRequests = await jobApiRepository.GetJobRequestsByJobIdAsync(job.Id, _clientData.Id, _clientData.AccessToken);
            if (jobRequests != null) JobRequestCount.Add(job.Id, jobRequests.Count);
        }

        return Page();
    }
}