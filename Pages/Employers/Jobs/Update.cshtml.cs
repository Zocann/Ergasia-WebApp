using System.Net;
using Ergasia_WebApp.Data;
using Ergasia_WebApp.DTOs.Job;
using Ergasia_WebApp.Services.Model.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Ergasia_WebApp.Pages.Employers.Jobs;

public class Update(IJobService jobService, IWorkerJobService workerJobService) : PageModel
{
    private ClientData _clientData = new(new HttpContextAccessor());
    
    [BindProperty(SupportsGet = true)]
    public required string JobId { get; set; }
    public required JobDto JobDto { get; set; }
    public string? Error;
    
    public async Task<IActionResult> OnGetAsync(string? error)
    {
        if (_clientData.AccessToken == null) return Unauthorized();
        
        var jobServiceResult = await jobService.GetAsync(JobId, _clientData.AccessToken);
        if (! jobServiceResult.IsSuccess)
        {
            if (jobServiceResult.StatusCode == HttpStatusCode.Unauthorized) return Unauthorized();
            return RedirectToPage("/Error");
        }
        JobDto = jobServiceResult.Data;
        Error = error;
        
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(JobDto jobDto, string date)
    {
        var dateOfBegin = ParseDateFromString(date);
        if (dateOfBegin == null || !IsValidDate(dateOfBegin))
            return RedirectToAction(nameof(OnGetAsync), new {error = "Invalid date of begin"});
        if (!ModelState.IsValid) 
            return RedirectToAction(nameof(OnGetAsync), new {error = "Please follow form instructions."});
        
        jobDto.DateOfBegin = (DateTime)dateOfBegin;
        jobDto.EmployerId = Request.Cookies["userId"];
        if (jobDto.EmployerId == null || jobDto.Id == null) return RedirectToPage("/Error");
        
        if (_clientData.AccessToken == null) return Unauthorized();
        
        //Checking for existing workerJobs to not change date if there are already signed workers
        var workerJobServiceResult = await workerJobService.GetByJobIdAsync(jobDto.Id, _clientData.AccessToken);
        if (workerJobServiceResult.IsSuccess)
        {
            var jobServiceResult = await jobService.GetAsync(jobDto.Id, _clientData.AccessToken);
            if (! jobServiceResult.IsSuccess)
            {
                if (jobServiceResult.StatusCode == HttpStatusCode.Unauthorized) return Unauthorized();
                return RedirectToPage("/Error");
            }
            if (! JobsDateOfBeginMatches(jobServiceResult.Data, JobDto)) 
                return RedirectToAction(nameof(OnGetAsync), new {error = "Cannot change date of beginning if workers are already registered to this job."});
        }

        var updateJobServiceResult = await jobService.PatchAsync(jobDto, _clientData.AccessToken);

        if (updateJobServiceResult.IsSuccess)
            return RedirectToPage("/Employers/Jobs/Information", new { jobId = updateJobServiceResult.Data.Id });
        
        if (updateJobServiceResult.StatusCode == HttpStatusCode.Unauthorized) return Unauthorized();
        return RedirectToPage("/Error");

    }

    private static bool JobsDateOfBeginMatches(JobDto firstJob, JobDto secondJob)
    {
        return firstJob.DateOfBegin == secondJob.DateOfBegin;
    }

    private static DateTime? ParseDateFromString(string dateOfBegin)
    {
        if (DateTime.TryParse(dateOfBegin, out var date)) return date;
        return null;
    }

    private static bool IsValidDate(DateTime? date)
    {
        return date > DateTime.UtcNow;
    }
}