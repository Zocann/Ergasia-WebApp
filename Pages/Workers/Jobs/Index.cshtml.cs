using Ergasia_WebApp.ApiRepositories.Interfaces;
using Ergasia_WebApp.Data;
using Ergasia_WebApp.DTOs.Job;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Ergasia_WebApp.Pages.Workers.Jobs;

public class Index(IJobApiRepository jobApiRepository) : PageModel
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

        var workerJobs = (await jobApiRepository.GetWorkerJobsByWorkerIdAsync(WorkerId, _clientData.AccessToken));

        if (Response.StatusCode == 401) return Unauthorized();


        if (workerJobs != null)
        {
            FinishedJobs = workerJobs
                .Where(wj => wj.JobDto.DateOfBegin.AddDays(wj.JobDto.Duration) < DateTime.UtcNow)
                .OrderByDescending(wj => wj.JobDto.DateOfBegin).Select(wj => wj.JobDto)
                .ToList();
            Jobs = workerJobs.Where(wj => wj.JobDto.DateOfBegin.AddDays(wj.JobDto.Duration) > DateTime.UtcNow)
                .OrderBy(wj => wj.JobDto.DateOfBegin).Select(wj => wj.JobDto).ToList();
        }
        
        return Page();
    }
}