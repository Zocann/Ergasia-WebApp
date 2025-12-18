using System.Net;
using Ergasia_WebApp.Data;
using Ergasia_WebApp.DTOs.Job;
using Ergasia_WebApp.Services.Model.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Ergasia_WebApp.Pages.Employers.Jobs;
public class Information(IJobService jobService, IWorkerJobService workerJobService) : PageModel
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

        var serviceResult = await jobService.GetAsync(JobId, _clientData.AccessToken);
        if (! serviceResult.IsSuccess)
        {
            if (serviceResult.StatusCode == HttpStatusCode.Unauthorized) return Unauthorized();
            return RedirectToPage("/Error");
        }
        
        JobDto = serviceResult.Data;
        if (JobDto.Id == null) return RedirectToPage("/Error");
        
        var workSpotsServiceResult = await workerJobService.GetAvailableWorkSpotsAsync(JobDto.Id, _clientData.AccessToken);
        if (! workSpotsServiceResult.IsSuccess) return RedirectToPage("/Error");
        
        WorkSpots = workSpotsServiceResult.Data;

        var workerJobsServiceResult = await workerJobService.GetByJobIdAsync(JobDto.Id, _clientData.AccessToken);
        if (workerJobsServiceResult.IsSuccess) WorkerJobs = workerJobsServiceResult.Data.ToList();
        
        return Page();
    }
}