using System.Net;
using Ergasia_WebApp.Data;
using Ergasia_WebApp.DTOs.Job;
using Ergasia_WebApp.Services.Model.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Ergasia_WebApp.Pages.Jobs;

public class Application(
    IJobService jobService,
    IWorkerJobService workerJobService,
    IJobRequestService jobRequestService) : PageModel
{
    private ClientData _clientData = new(new HttpContextAccessor());

    [BindProperty(SupportsGet = true)] public required string JobId { get; set; }
    public required JobDto JobDto { get; set; }
    public WorkerJobDto? WorkerJob { get; set; }
    public JobRequestDto? JobRequest { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        if (_clientData.AccessToken == null) return Unauthorized();
        if (_clientData.Id == null) return RedirectToPage("/Error");

        var serviceResult = await jobService.GetAsync(JobId, _clientData.AccessToken);
        if (!serviceResult.IsSuccess)
        {
            if (serviceResult.StatusCode == HttpStatusCode.Unauthorized) return Unauthorized();
            return RedirectToPage("/Error");
        }

        JobDto = serviceResult.Data;

        var workerJobServiceResult = await workerJobService.GetAsync(JobId, _clientData.Id, _clientData.AccessToken);
        WorkerJob = workerJobServiceResult.Data;

        var jobRequestServiceResult = await jobRequestService.GetAsync(JobId, _clientData.Id, _clientData.AccessToken);
        JobRequest = jobRequestServiceResult.Data;

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(string message)
    {
        if (_clientData.AccessToken == null) return Unauthorized();
        if (_clientData.Id == null) return RedirectToPage("/Error");

        var jobRequestServiceResult =
            await jobRequestService.PostAsync(JobId, _clientData.Id, message, _clientData.AccessToken);
        if (jobRequestServiceResult.IsSuccess) return RedirectToPage("/Jobs/Application", new { jobId = JobId });

        if (jobRequestServiceResult.StatusCode == HttpStatusCode.Unauthorized) return Unauthorized();
        return RedirectToPage("/Error");
    }
}