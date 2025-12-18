using System.Net;
using Ergasia_WebApp.Data;
using Ergasia_WebApp.DTOs.Worker;
using Ergasia_WebApp.Services.Model.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Ergasia_WebApp.Pages.Workers;

public class Index(IWorkerService workerService, IEmployerRatingService employerRatingService) : PageModel
{
    private ClientData _clientData = new(new HttpContextAccessor());

    public required List<WorkerDto> Workers { get; set; }
    public required Dictionary<string, float> AverageRating { get; set; } = new();

    public async Task<IActionResult> OnGet()
    {
        if (_clientData.AccessToken == null) return Unauthorized();

        var serviceResult = await workerService.GetAllAsync(_clientData.AccessToken);
        
        if (!serviceResult.IsSuccess)
        {
            if (serviceResult.StatusCode == HttpStatusCode.Unauthorized) return Unauthorized();
            return RedirectToPage("/Error");
        }
        
        Workers = serviceResult.Data.ToList();

        foreach (var worker in Workers)
        {
            await AddAverageRating(worker);
        }
        
        return Page();
    }

    private async Task AddAverageRating(WorkerDto worker)
    {
        var serviceResult = await employerRatingService.GetAverageRatingAsync(worker.Id);
        if (serviceResult.IsSuccess) AverageRating.Add(worker.Id, serviceResult.Data);
    }
}