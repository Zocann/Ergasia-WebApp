using Ergasia_WebApp.ApiRepositories.Interfaces;
using Ergasia_WebApp.Data;
using Ergasia_WebApp.DTOs.Rating;
using Ergasia_WebApp.DTOs.Worker;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Ergasia_WebApp.Pages.Workers;

public class Profile(IWorkerApiRepository workerApiRepository, IRatingApiRepository ratingApiRepository) : PageModel
{
    private ClientData _clientData = new(new HttpContextAccessor());
    
    
    [BindProperty(SupportsGet = true)]
    public required string WorkerId { get; set; }
    public required WorkerDto Worker { get; set; }
    public required List<WorkerRatingDto?> WorkerRatings { get; set; }
    public decimal? AverageRating { get; set; }
    
    public async Task<IActionResult> OnGetAsync()
    {
        if (!_clientData.GetAccessToken()) return Unauthorized();

        var worker = await workerApiRepository.GetAsync(WorkerId, _clientData.AccessToken!);
        if (worker == null)
        {
            if (Response.StatusCode == 401) return Unauthorized();
            return RedirectToPage("/Error");
        }
        Worker = worker;
        
        var ratings = await ratingApiRepository.GetWorkerRatingsAsync(WorkerId, _clientData.AccessToken);

        if (ratings != null)
        {
            WorkerRatings = ratings.ToList();
        }

        AverageRating = await ratingApiRepository.GetWorkerAverageRating(WorkerId);
        
        return Page();
    }
}