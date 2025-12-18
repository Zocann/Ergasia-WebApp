using System.Net;
using Ergasia_WebApp.Data;
using Ergasia_WebApp.DTOs.Job;
using Ergasia_WebApp.Services.Model.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Ergasia_WebApp.Pages.Workers.Jobs;

public class Index(IWorkerJobService workerJobService) : PageModel
{
    private ClientData _clientData = new (new HttpContextAccessor());
    
    [BindProperty(SupportsGet = true)]
    public required string WorkerId { get; set; }

    public required List<JobDto> Jobs { get; set; } = [];
    public required List<JobDto> FinishedJobs { get; set; } = [];
    
    public async Task<IActionResult> OnGetAsync()
    {
        if (_clientData.AccessToken == null) return Unauthorized();
        if (_clientData.Id == null) return RedirectToPage("/Error");

        var serviceResult = (await workerJobService.GetByWorkerIdAsync(WorkerId, _clientData.AccessToken));

        if (!serviceResult.IsSuccess)
        {
            if (serviceResult.StatusCode == HttpStatusCode.Unauthorized) return Unauthorized();
            return RedirectToPage("/Error");
        }
        
        FinishedJobs = GetFinishedWorkerJobs(serviceResult.Data);
        Jobs = GetUpcomingWorkerJobs(serviceResult.Data);
        
        return Page();
    }
    
    private static List<JobDto> GetFinishedWorkerJobs(IEnumerable<WorkerJobDto> allWorkerJobs)
    {
        return allWorkerJobs
            .Where(wj => wj.JobDto.DateOfBegin.AddDays(wj.JobDto.Duration) < DateTime.UtcNow)
            .OrderByDescending(wj => wj.JobDto.DateOfBegin).Select(wj => wj.JobDto)
            .ToList();
    }
    
    private static List<JobDto> GetUpcomingWorkerJobs(IEnumerable<WorkerJobDto> allWorkerJobs)
    {
        return allWorkerJobs.Where(wj => wj.JobDto.DateOfBegin.AddDays(wj.JobDto.Duration) > DateTime.UtcNow)
            .OrderBy(wj => wj.JobDto.DateOfBegin).Select(wj => wj.JobDto).ToList();
    }
}