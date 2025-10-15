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
    public required List<JobDto?> FinishedJobs { get; set; } = [];

    public required List<JobRequestDto> JobRequests { get; set; } = [];
    
    public async Task<IActionResult> OnGetAsync()
    {
        if (!_clientData.GetId()) return RedirectToPage("/Error");
        if (!_clientData.GetAccessToken()) return Unauthorized();

        var response = (await jobApiRepository.GetWorkerJobsByWorkerIdAsync(WorkerId, _clientData.AccessToken!));

        if (Response.StatusCode == 401) return Unauthorized();
        

        if (response != null)
        {
            var workerJobs = response.ToList();
            workerJobs.RemoveAll(wj => wj == null);
            FinishedJobs = workerJobs
                .Where(wj => wj!.JobDto.DateOfBegin.AddDays(wj.JobDto.Duration) < DateTime.UtcNow)
                .OrderByDescending(wj => wj!.JobDto.DateOfBegin).Select(wj => wj!.JobDto)
                .ToList();
            Jobs = workerJobs.Where(wj => wj.JobDto.DateOfBegin.AddDays(wj.JobDto.Duration) > DateTime.UtcNow)
                .OrderBy(wj => wj!.JobDto.DateOfBegin).Select(wj => wj!.JobDto).ToList();
        }
        

        var result = await jobApiRepository.GetJobRequestsByWorkerIdAsync(WorkerId, _clientData.AccessToken!);
        
        if (result != null)
        {
            var jobRequestDtos = result.ToList();
            jobRequestDtos.RemoveAll(jr => jr == null);
            JobRequests = jobRequestDtos.Where(jr => jr != null).OrderBy(jr => jr!.JobDto.DateOfBegin).ToList();
        }
        
        return Page();
    }
}