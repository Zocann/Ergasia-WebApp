using Ergasia_WebApp.ApiRepositories.Interfaces;
using Ergasia_WebApp.Data;
using Ergasia_WebApp.DTOs.Employer;
using Ergasia_WebApp.DTOs.Rating;
using Ergasia_WebApp.DTOs.Worker;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Ergasia_WebApp.Pages.Account;

public class Index(IEmployerApiRepository employerApiRepository, IWorkerApiRepository workerApiRepository, IRatingApiRepository ratingApiRepository) : PageModel
{
    private ClientData _clientData = new(new HttpContextAccessor());
    
    public WorkerDto? Worker { get; set; }
    public EmployerDto? Employer { get; set; }
    public List<WorkerRatingDto>? WorkerRatings { get; set; }
    public List<EmployerRatingDto>? EmployerRatings { get; set; }
    
    public async Task<IActionResult> OnGet()
    {
        if (_clientData.AccessToken == null) return Unauthorized();
        if (_clientData.Id == null) return RedirectToPage("/Error");
        if (_clientData.Role == null) return RedirectToPage("/Error");
        
        switch (_clientData.Role)
        {
            case "Employer":
                var employer = await employerApiRepository.GetAsync(_clientData.Id, _clientData.AccessToken);
                if (employer == null)
                {
                    if (Response.StatusCode == 401) return Unauthorized();
                    return RedirectToPage("/Error");
                }
                Employer = employer;
                
                EmployerRatings = await ratingApiRepository.GetEmployerRatingsAsync(employer.Id, _clientData.AccessToken);
                break;
            
            case "Worker":
                var worker = await workerApiRepository.GetAsync(_clientData.Id, _clientData.AccessToken);
                if (worker == null)
                {
                    if (Response.StatusCode == 401) return Unauthorized();
                    return RedirectToPage("/Error");
                }
                Worker = worker;
                
                WorkerRatings = await ratingApiRepository.GetWorkerRatingsAsync(worker.Id, _clientData.AccessToken);
                break;
        }
        
        return Page();
    }
}