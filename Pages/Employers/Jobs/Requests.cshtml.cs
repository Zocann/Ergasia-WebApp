using System.Net;
using Ergasia_WebApp.Data;
using Ergasia_WebApp.DTOs.Job;
using Ergasia_WebApp.Services.Model.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Ergasia_WebApp.Pages.Employers.Jobs;

public class Requests(IJobService jobService, 
    IJobRequestService jobRequestService, IWorkerJobService workerJobService) : PageModel
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
       
        var jobServiceResult = await jobService.GetAsync(JobId, _clientData.AccessToken);
        if (! jobServiceResult.IsSuccess)
        {
            if (jobServiceResult.StatusCode == HttpStatusCode.Unauthorized) return Unauthorized();
            return RedirectToPage("Error");
        }
        Job = jobServiceResult.Data;
        
        var jobRequestsServiceResult =  await jobRequestService.GetByJobIdAsync(JobId, _clientData.Id, _clientData.AccessToken);
        if (jobRequestsServiceResult.IsSuccess) JobRequests = jobRequestsServiceResult.Data.ToList();
        
        Error = error;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(string workerId, string method)
    {
        if (_clientData.AccessToken == null) return Unauthorized();
        if (_clientData.Id == null) return RedirectToPage("/Error");

        return method switch
        {
            "post" => await HandlePostAsync(_clientData.Id, workerId, _clientData.AccessToken),
            "delete" => await HandleDeleteAsync(_clientData.Id, workerId, _clientData.AccessToken),
            _ => RedirectToPage("/Error")
        };
    }

    private async Task<IActionResult> HandlePostAsync(string clientId, string workerId, string accessToken)
    {
        var jobServiceResult = await jobService.GetAsync(JobId, accessToken);
        if (! jobServiceResult.IsSuccess)
        {
            if (jobServiceResult.StatusCode == HttpStatusCode.Unauthorized) return Unauthorized();
            return RedirectToPage("Error");
        }
        
        var workspotsServiceResult = await workerJobService.GetAvailableWorkSpotsAsync(JobId, accessToken);
        if (workspotsServiceResult is { IsSuccess: true, Data: <= 0 }) 
            return RedirectToAction(nameof(OnGetAsync), new { error = "Job is at full capacity. Update job information to hire more workers"});

        var workerJobsServiceResult = await workerJobService.PostAsync(JobId, clientId, workerId, accessToken);
        if (workerJobsServiceResult.IsSuccess) return RedirectToAction(nameof(OnGetAsync));
        
        if (workerJobsServiceResult.StatusCode == HttpStatusCode.Unauthorized) return Unauthorized();
        return RedirectToPage("/Error");
    }

    private async Task<IActionResult> HandleDeleteAsync(string clientId, string workerId, string accessToken)
    {
        var serviceResult = await jobRequestService.DeleteAsync(JobId, clientId, workerId, accessToken);
        if (serviceResult.IsSuccess) return RedirectToAction(nameof(OnGetAsync));
        
        if (serviceResult.StatusCode == HttpStatusCode.Unauthorized) return Unauthorized();
        return RedirectToPage("/Error");
    }
}