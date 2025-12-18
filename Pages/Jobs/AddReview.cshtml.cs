using System.ComponentModel.DataAnnotations;
using System.Net;
using Ergasia_WebApp.Data;
using Ergasia_WebApp.DTOs.Job;
using Ergasia_WebApp.DTOs.Rating;
using Ergasia_WebApp.Services.Model.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.RazorPages.Infrastructure;

namespace Ergasia_WebApp.Pages.Jobs;

public class AddReview(IJobService jobService, IWorkerJobService workerJobService, IJobRatingService jobRatingService) : PageModel
{
    private ClientData _clientData = new(new HttpContextAccessor());

    [BindProperty(SupportsGet = true)] public required string JobId { get; set; }
    public WorkerJobDto? Rating { get; set; }
    public required JobDto Job { get; set; }
    public string? Error;


    public async Task<IActionResult> OnGetAsync(string? error)
    {
        if (_clientData.AccessToken == null) return Unauthorized();
        if (_clientData.Id == null) return RedirectToPage("/Error");

        var serviceResult = await jobService.GetAsync(JobId, _clientData.AccessToken);
        if (! serviceResult.IsSuccess)
        {
            if (serviceResult.StatusCode == HttpStatusCode.Unauthorized) return Unauthorized();
            return RedirectToPage("/Error");
        }

        Job = serviceResult.Data;
        
        var ratingsServiceResult = await workerJobService.GetAsync(JobId, _clientData.Id, _clientData.AccessToken);
        Rating = ratingsServiceResult.Data;
        Error = error;
        
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(string employerId, [Required][Range(1, 5)]int rating,
        string verRating, string? delete)
    {
        if (!ModelState.IsValid) return RedirectToAction(nameof(OnGetAsync), new {error = "Please provide a valid rating"});
        if (_clientData.AccessToken == null) return Unauthorized();
        if (_clientData.Id == null) return RedirectToPage("/Error");

        if (delete == "true") return await HandleDeleteAsync(employerId, JobId, _clientData.Id, _clientData.AccessToken);

        return await HandleUpdateAsync(new JobRatingDto(JobId, _clientData.Id, rating, verRating), employerId, _clientData.AccessToken);
    }

    private async Task<IActionResult> HandleDeleteAsync(string employerId, string jobId, string clientId, string accessToken)
    {
        var serviceResult = await jobRatingService.DeleteAsync(employerId, jobId, clientId, accessToken);
        if (serviceResult.IsSuccess) return RedirectToAction(nameof(OnGetAsync));
        
        if (serviceResult.StatusCode == HttpStatusCode.Unauthorized) return Unauthorized();
        return RedirectToPage("/Error");
    }
    
    private async Task<IActionResult> HandleUpdateAsync(JobRatingDto ratingDto, string employerId, string accessToken)
    {
        var serviceResult = await jobRatingService.PatchAsync(ratingDto, employerId, accessToken);
        if (serviceResult.IsSuccess) return RedirectToAction(nameof(OnGetAsync));
        
        if (serviceResult.StatusCode == HttpStatusCode.Unauthorized) return Unauthorized();
        return RedirectToPage("/Error");
    }
}