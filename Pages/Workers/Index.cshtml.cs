using Ergasia_WebApp.ApiRepositories.Interfaces;
using Ergasia_WebApp.Data;
using Ergasia_WebApp.DTOs.Worker;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Ergasia_WebApp.Pages.Workers;

public class Index(IWorkerApiRepository workerApiRepository, IRatingApiRepository ratingApiRepository) : PageModel
{
    private ClientData _clientData = new(new HttpContextAccessor());

    public required IEnumerable<WorkerDto?> Workers { get; set; } = [];
    public required Dictionary<string, decimal> AverageRating { get; set; } = new();

    public async Task<IActionResult> OnGet()
    {
        if (!_clientData.GetAccessToken()) return Unauthorized();

        var workers = await workerApiRepository.GetAllAsync(_clientData.AccessToken!);
        if (workers == null)
        {
            if (Response.StatusCode == 401) return Unauthorized();
            return RedirectToPage("/Error");
        }
        
        Workers = workers;

        foreach (var worker in Workers)
        {
            if (worker == null) continue;
            var rating = await ratingApiRepository.GetWorkerAverageRating(worker.Id);
            if (rating != null) AverageRating.Add(worker.Id, (decimal)rating);
        }
        
        return Page();
    }
}