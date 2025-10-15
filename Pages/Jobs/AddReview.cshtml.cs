using System.ComponentModel.DataAnnotations;
using Ergasia_WebApp.ApiRepositories.Interfaces;
using Ergasia_WebApp.Data;
using Ergasia_WebApp.DTOs.Job;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Ergasia_WebApp.Pages.Jobs;

public class AddReview(IJobApiRepository jobApiRepository, IRatingApiRepository ratingApiRepository) : PageModel
{
    private ClientData _clientData = new(new HttpContextAccessor());

    [BindProperty(SupportsGet = true)] public required string JobId { get; set; }
    public WorkerJobDto? Rating { get; set; }
    public required JobDto Job { get; set; }
    public string? Error;


    public async Task<IActionResult> OnGetAsync(string? error)
    {
        if (!_clientData.GetAccessToken()) return Unauthorized();
        if (!_clientData.GetId()) return RedirectToPage("/Error");

        var job = await jobApiRepository.GetAsync(JobId, _clientData.AccessToken!);
        if (job == null)
        {
            if (Response.StatusCode == 401) return Unauthorized();
            return RedirectToPage("/Error");
        }

        Job = job;
        var rating = await jobApiRepository.GetWorkerJobAsync(JobId, _clientData.Id!, _clientData.AccessToken!);

        Rating = rating;
        Error = error;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(string employerId, [Required][Range(1, 5)]string rating,
        string verRating, string? delete)
    {
        if (!ModelState.IsValid)
        {
            return RedirectToAction(nameof(OnGetAsync), new {error = "Please provide a valid rating"});
        }
        
        if (!_clientData.GetAccessToken()) return Unauthorized();
        if (!_clientData.GetId()) return RedirectToPage("/Error");

        if (delete == "true")
        {
            if (!await ratingApiRepository.DeleteJobRatingAsync(employerId, JobId, _clientData.Id!, _clientData.AccessToken!))
            {
                if (Response.StatusCode == 401) return Unauthorized();
                return RedirectToPage("/Error");
            }

            return RedirectToAction(nameof(OnGetAsync));
        }

        var workerJob = await ratingApiRepository.PatchJobRatingAsync(employerId, JobId, _clientData.Id!, rating, verRating, _clientData.AccessToken!);

        if (workerJob == null)
        {
            if (Response.StatusCode == 401) return Unauthorized();
            return RedirectToPage("/Error");
        }

        return RedirectToAction(nameof(OnGetAsync));
    }
}