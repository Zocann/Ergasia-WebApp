using Ergasia_WebApp.ApiRepositories.Interfaces;
using Ergasia_WebApp.Data;
using Ergasia_WebApp.DTOs.Job;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Ergasia_WebApp.Pages.Employers.Jobs;

public class Add(IJobApiRepository jobApiRepository) : PageModel
{
    private ClientData _clientData = new (new HttpContextAccessor());
    
    public string? Error;
    
    public void OnGet(string? error)
    {
        Error = error;
    }

    public async Task<IActionResult> OnPostAsync(JobDto jobDto, string date)
    {
        if (!DateTime.TryParse(date, out var dateOfBegin) || dateOfBegin < DateTime.Now) return RedirectToAction(nameof(OnGet), new {error = "Invalid date of begin"});
        if (!ModelState.IsValid)
        {
            return RedirectToAction(nameof(OnGet), new {error = "Please follow form instructions."});
        }
        if (!_clientData.GetAccessToken()) return Unauthorized();
        
        jobDto.DateOfBegin = dateOfBegin;
        jobDto.EmployerId = Request.Cookies["userId"];
        
        if (jobDto.EmployerId == null) RedirectToPage("/Error");
        
        var job = await jobApiRepository.AddAsync(jobDto, _clientData.AccessToken);
        if (job == null)
        {
            if (Response.StatusCode == 401) return Unauthorized();
            return RedirectToPage("/Error");
        }
        
        return RedirectToPage("/Employers/Jobs/Information", new { jobId = job.Id });
    }
}