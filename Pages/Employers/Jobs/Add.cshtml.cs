using System.Net;
using Ergasia_WebApp.Data;
using Ergasia_WebApp.DTOs.Job;
using Ergasia_WebApp.Services.Model.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Ergasia_WebApp.Pages.Employers.Jobs;

public class Add(IJobService jobService) : PageModel
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
        if (!ModelState.IsValid) return RedirectToAction(nameof(OnGet), new {error = "Please follow form instructions."});
        if (_clientData.AccessToken == null) return Unauthorized();
        
        jobDto.DateOfBegin = dateOfBegin;
        jobDto.EmployerId = Request.Cookies["userId"];
        
        if (jobDto.EmployerId == null) RedirectToPage("/Error");
        
        var serviceResult = await jobService.AddAsync(jobDto, _clientData.AccessToken);
        if (serviceResult.IsSuccess)
            return RedirectToPage("/Employers/Jobs/Information", new { jobId = serviceResult.Data.Id });
        
        if (serviceResult.StatusCode == HttpStatusCode.Unauthorized) return Unauthorized();
        return RedirectToPage("/Error");

    }
}