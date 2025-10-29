using System.ComponentModel.DataAnnotations;
using Ergasia_WebApp.ApiRepositories.Interfaces;
using Ergasia_WebApp.Data;
using Ergasia_WebApp.DTOs.Rating;
using Ergasia_WebApp.DTOs.Worker;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Ergasia_WebApp.Pages.Workers;

public class AddReview(IWorkerApiRepository workerApiRepository, IRatingApiRepository ratingApiRepository) : PageModel
{
    private ClientData _clientData = new(new HttpContextAccessor());

    [BindProperty(SupportsGet = true)] public required string WorkerId { get; set; }
    public WorkerRatingDto? Rating { get; set; }
    public required WorkerDto Worker { get; set; }
    public string? Error;


    public async Task<IActionResult> OnGetAsync(string? error)
    {
        if (_clientData.AccessToken == null) return Unauthorized();
        if (_clientData.Id == null) return RedirectToPage("/Error");

        var worker = await workerApiRepository.GetAsync(WorkerId, _clientData.AccessToken);
        if (worker == null)
        {
            if (Response.StatusCode == 401) return Unauthorized();
            return RedirectToPage("/Error");
        }

        Worker = worker;
        Rating = await ratingApiRepository.GetWorkerRatingAsync(WorkerId, _clientData.Id, _clientData.AccessToken);
        Error = error;
        
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(string workerId, [Required][Range(1, 5)] string rating, 
        string verRating, string action, string? delete)
    {
        if (!ModelState.IsValid)
        {
            return RedirectToAction(nameof(OnGetAsync), new {error = "Please provide a valid rating"});
        }
        if (_clientData.AccessToken == null) return Unauthorized();
        if (_clientData.Id == null) return RedirectToPage("/Error");
        
        if (delete == "true")
        {
            if (!await ratingApiRepository.DeleteWorkerRatingAsync(WorkerId, _clientData.Id, _clientData.AccessToken))
            {
                if (Response.StatusCode == 401) return Unauthorized();
                return RedirectToPage("/Error");
            }
            return RedirectToAction(nameof(OnGetAsync)); 
        }
        
        WorkerRatingDto? workerRating;
        
        switch (action)
        {
            case "post":
                workerRating = await ratingApiRepository.PostWorkerRatingAsync(workerId, _clientData.Id, rating, verRating, _clientData.AccessToken);

                if (workerRating == null)
                {
                    if (Response.StatusCode == 401) return Unauthorized();
                    return RedirectToPage("/Error");
                }
                break;

            case "update":
                workerRating = await ratingApiRepository.PatchWorkerRatingAsync(workerId, _clientData.Id, rating, verRating, _clientData.AccessToken);

                if (workerRating == null)
                {
                    if (Response.StatusCode == 401) return Unauthorized();
                    return RedirectToPage("/Error");
                }
                break;
            default:
                return RedirectToPage("/Error");
        }

        return RedirectToAction(nameof(OnGetAsync));
    }
}