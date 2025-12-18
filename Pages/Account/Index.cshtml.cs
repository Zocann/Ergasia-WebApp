using System.Net;
using Ergasia_WebApp.Data;
using Ergasia_WebApp.DTOs.Employer;
using Ergasia_WebApp.DTOs.Rating;
using Ergasia_WebApp.DTOs.Worker;
using Ergasia_WebApp.Services.Model.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Ergasia_WebApp.Pages.Account;

public class Index(
    IEmployerService employerService,
    IWorkerService workerService,
    IEmployerRatingService employerRatingService,
    IWorkerRatingService workerRatingService) : PageModel
{
    private ClientData _clientData = new(new HttpContextAccessor());
    public WorkerDto? Worker { get; set; }
    public EmployerDto? Employer { get; set; }
    public List<WorkerRatingDto>? WorkerRatings { get; set; }
    public List<EmployerRatingDto>? EmployerRatings { get; set; }

    public async Task<IActionResult> OnGet()
    {
        if (_clientData.AccessToken == null) return Unauthorized();
        if (_clientData.Id == null || _clientData.Role == null) return RedirectToPage("/Error");

        return _clientData.Role switch
        {
            "Employer" => await HanldeEmployer(_clientData.Id, _clientData.AccessToken),
            "Worker" => await HandleWorker(_clientData.Id, _clientData.AccessToken),
            _ => RedirectToPage("/Error")
        };
    }

    private async Task<IActionResult> HanldeEmployer(string userId, string accessToken)
    {
        var employerServiceResult = await employerService.GetAsync(userId, accessToken);
        if (! employerServiceResult.IsSuccess)
        {
            if (employerServiceResult.StatusCode == HttpStatusCode.Unauthorized) return Unauthorized();
            return RedirectToPage("/Error");
        }

        Employer = employerServiceResult.Data;

        var employerRatingServiceResult =
            await employerRatingService.GetAllAsync(Employer.Id, accessToken);
        if (employerRatingServiceResult.IsSuccess) EmployerRatings = employerRatingServiceResult.Data.ToList();
        return Page();
    }
    
    private async Task<IActionResult> HandleWorker(string userId, string accessToken)
    {
        var workerServiceResult = await workerService.GetAsync(userId, accessToken);
        if (!workerServiceResult.IsSuccess)
        {
            if (workerServiceResult.StatusCode == HttpStatusCode.Unauthorized) return Unauthorized();
            return RedirectToPage("/Error");
        }

        Worker = workerServiceResult.Data;

        var workerRatingServiceResult =
            await workerRatingService.GetAllAsync(Worker.Id, accessToken);

        if (workerRatingServiceResult.IsSuccess) WorkerRatings = workerRatingServiceResult.Data.ToList();
        return Page();
    }
}