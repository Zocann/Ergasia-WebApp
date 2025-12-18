using System.Net;
using Ergasia_WebApp.Data;
using Ergasia_WebApp.DTOs.Rating;
using Ergasia_WebApp.DTOs.Worker;
using Ergasia_WebApp.Services.Model.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Ergasia_WebApp.Pages.Workers;

public class Profile(IWorkerService workerService, IWorkerRatingService workerRatingService) : PageModel
{
    private ClientData _clientData = new(new HttpContextAccessor());
    
    
    [BindProperty(SupportsGet = true)]
    public required string WorkerId { get; set; }
    public required WorkerDto Worker { get; set; }
    public required List<WorkerRatingDto>? WorkerRatings { get; set; }
    public float? AverageRating { get; set; }
    
    public async Task<IActionResult> OnGetAsync()
    {
        if (_clientData.AccessToken == null) return Unauthorized();

        var serviceResult = await workerService.GetAsync(WorkerId, _clientData.AccessToken);
        if (! serviceResult.IsSuccess)
        {
            if (serviceResult.StatusCode == HttpStatusCode.Unauthorized) return Unauthorized();
            return RedirectToPage("/Error");
        }
        
        Worker = serviceResult.Data;
        WorkerRatings = await GetAllWorkerRatingsAsync(_clientData.AccessToken);
        AverageRating = await GetAverageRatingAsync();
        
        return Page();
    }

    private async Task<List<WorkerRatingDto>?> GetAllWorkerRatingsAsync(string accessToken)
    {
        var serviceResult = await workerRatingService.GetAllAsync(WorkerId, accessToken);
        return serviceResult.IsSuccess ? serviceResult.Data.ToList() : null;
    }
    
    private async Task<float?> GetAverageRatingAsync()
    {
        var serviceResult = await workerRatingService.GetAverageRatingAsync(WorkerId);
        return serviceResult.Data;
    }
}