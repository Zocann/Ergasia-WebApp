using System.ComponentModel.DataAnnotations;
using System.Net;
using Ergasia_WebApp.Data;
using Ergasia_WebApp.DTOs.Rating;
using Ergasia_WebApp.DTOs.Worker;
using Ergasia_WebApp.Services.Model.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Ergasia_WebApp.Pages.Workers;

public class AddReview(IWorkerService workerService, IWorkerRatingService workerRatingService) : PageModel
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

        var serviceResult = await workerService.GetAsync(WorkerId, _clientData.AccessToken);

        if (! serviceResult.IsSuccess)
        {
            if (serviceResult.StatusCode == HttpStatusCode.Unauthorized) return Unauthorized();
            return RedirectToPage("/Error");
        }

        Worker = serviceResult.Data;
        
        var ratingServiceResult = await workerRatingService.GetAsync(WorkerId, _clientData.Id, _clientData.AccessToken);
        Rating = ratingServiceResult.Data;
        
        Error = error;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(string workerId, [Required][Range(1, 5)] int rating, 
        string verRating, string action, string? delete)
    {
        if (!ModelState.IsValid) return RedirectToAction(nameof(OnGetAsync), new {error = "Please provide valid rating"});
        if (_clientData.AccessToken == null) return Unauthorized();
        if (_clientData.Id == null) return RedirectToPage("/Error");
        
        if (delete == "true")
        {
            return await HandleDelete(_clientData.Id, _clientData.AccessToken);
        }
        
        var ratingDto = new RatingDto(_clientData.Id, workerId, rating, verRating);

        return action switch
        {
            "post" => await HandlePost(ratingDto, _clientData.AccessToken),
            "update" => await HandleUpdate(ratingDto, _clientData.AccessToken),
            _ => RedirectToPage("/Error")
        };
    }

    private async Task<IActionResult> HandleDelete(string clientId, string accessToken)
    {
        var serviceResult = await workerRatingService.DeleteAsync(WorkerId, clientId, accessToken);
        
        if (serviceResult.IsSuccess) return RedirectToAction(nameof(OnGetAsync));
        
        if (serviceResult.StatusCode == HttpStatusCode.Unauthorized) return Unauthorized();
        return RedirectToPage("/Error");
    }

    private async Task<IActionResult> HandlePost(RatingDto ratingDto, string accessToken)
    {
        var serviceResult = await workerRatingService.PostAsync(ratingDto, accessToken);
        
        if (serviceResult.IsSuccess) return RedirectToAction(nameof(OnGetAsync));

        if (serviceResult.StatusCode == HttpStatusCode.Unauthorized) return Unauthorized();
        return RedirectToPage("/Error");
    }
    
    private async Task<IActionResult> HandleUpdate(RatingDto ratingDto, string accessToken)
    {
        var serviceResult = await workerRatingService.PatchAsync(ratingDto, accessToken);
        
        if (serviceResult.IsSuccess) return RedirectToAction(nameof(OnGetAsync));

        if (serviceResult.StatusCode == HttpStatusCode.Unauthorized) return Unauthorized();
        return RedirectToPage("/Error");
    }
}