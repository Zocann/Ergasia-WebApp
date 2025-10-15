using Ergasia_WebApp.ApiRepositories.Interfaces;
using Ergasia_WebApp.Data;
using Ergasia_WebApp.DTOs.Job;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Ergasia_WebApp.Pages.Employers.Jobs;

public class Update(IJobApiRepository jobApiRepository) : PageModel
{
    private ClientData _clientData = new(new HttpContextAccessor());
    
    [BindProperty(SupportsGet = true)]
    public required string JobId { get; set; }
    public required JobDto JobDto { get; set; }
    public string? Error;
    
    public async Task<IActionResult> OnGetAsync(string? error)
    {
        if (!_clientData.GetAccessToken()) return Unauthorized();
        
        var job = await jobApiRepository.GetAsync(JobId, _clientData.AccessToken!);
        if (job == null || job.Id == null)
        {
            if (Response.StatusCode == 401) return Unauthorized();
            return RedirectToPage("/Error");
        }
        JobDto = job;
        Error = error;
        
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(JobDto jobDto, string date)
    {
        if (!DateTime.TryParse(date, out var dateOfBegin) || dateOfBegin < DateTime.Now) return RedirectToAction(nameof(OnGetAsync), new {error = "Invalid date of begin"});
        
        jobDto.DateOfBegin = dateOfBegin;
        
        if (!ModelState.IsValid)
        {
            return RedirectToAction(nameof(OnGetAsync), new {error = "Please follow form instructions."});
        }
        
        jobDto.DateOfBegin = DateTime.Parse(date);
        
        
        jobDto.EmployerId = Request.Cookies["userId"];
        if (jobDto.EmployerId == null || jobDto.Id == null) return RedirectToPage("/Error");
        
        if (! _clientData.GetAccessToken()) return Unauthorized();
        
        //Checking for existing workerJobs to not change date if there are already signed workers
        var workerJob = await jobApiRepository.GetWorkerJobsByJobIdAsync(jobDto.Id, _clientData.AccessToken!);
        if (workerJob != null && workerJob.TakeWhile(wj => wj != null).Any())
        {
            var j = await jobApiRepository.GetAsync(jobDto.Id, _clientData.AccessToken!);
            if (j == null)
            {
                if (Response.StatusCode == 401) return Unauthorized();
                return RedirectToPage("/Error");
            }
            if (j.DateOfBegin != jobDto.DateOfBegin) return RedirectToAction(nameof(OnGetAsync), new {error = "Cannot change date of beginning if workers are already registered to this job."});
        }

        var job = await jobApiRepository.PatchAsync(jobDto, _clientData.AccessToken!);

        if (job == null)
        {
            if (Response.StatusCode == 401) return Unauthorized();
            return RedirectToPage("/Error");
        }
        
        return RedirectToPage("/Employers/Jobs/Information", new { jobId = job.Id });
    }
}