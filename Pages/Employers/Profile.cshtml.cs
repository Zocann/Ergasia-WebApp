using Ergasia_WebApp.ApiRepositories.Interfaces;
using Ergasia_WebApp.Data;
using Ergasia_WebApp.DTOs.Employer;
using Ergasia_WebApp.DTOs.Rating;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Ergasia_WebApp.Pages.Employers;

public class Profile(IEmployerApiRepository employerApiRepository, IRatingApiRepository ratingApiRepository) : PageModel
{
    private ClientData _clientData = new(new HttpContextAccessor());
    
    
    [BindProperty(SupportsGet = true)]
    public required string EmployerId { get; set; }
    public required EmployerDto Employer { get; set; }
    public required List<EmployerRatingDto>? EmployerRatings { get; set; }
    public decimal? AverageRating { get; set; }
    
    public async Task<IActionResult> OnGetAsync()
    {
        if (_clientData.AccessToken == null) return Unauthorized();
        
        var employer = await employerApiRepository.GetAsync(EmployerId, _clientData.AccessToken);
        if (employer == null)
        {
            if (Response.StatusCode == 401) return Unauthorized();
            return RedirectToPage("/Error");
        }
        
        Employer = employer;
        EmployerRatings = await ratingApiRepository.GetEmployerRatingsAsync(EmployerId, _clientData.AccessToken);
        AverageRating = await ratingApiRepository.GetEmployerAverageRating(EmployerId);
        
        return Page();
    }
}