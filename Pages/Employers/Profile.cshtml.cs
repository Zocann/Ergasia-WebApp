using System.Net;
using Ergasia_WebApp.Data;
using Ergasia_WebApp.DTOs.Employer;
using Ergasia_WebApp.DTOs.Rating;
using Ergasia_WebApp.Services.Model.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Ergasia_WebApp.Pages.Employers;

public class Profile(IEmployerService employerService, IEmployerRatingService employerRatingService) : PageModel
{
    private ClientData _clientData = new(new HttpContextAccessor());
    
    
    [BindProperty(SupportsGet = true)]
    public required string EmployerId { get; set; }
    public required EmployerDto Employer { get; set; }
    public required List<EmployerRatingDto>? EmployerRatings { get; set; }
    public float? AverageRating { get; set; }
    
    public async Task<IActionResult> OnGetAsync()
    {
        if (_clientData.AccessToken == null) return Unauthorized();
        
        var serviceResult = await employerService.GetAsync(EmployerId, _clientData.AccessToken);
        if (! serviceResult.IsSuccess)
        {
            if (serviceResult.StatusCode == HttpStatusCode.Unauthorized) return Unauthorized();
            return RedirectToPage("/Error");
        }
        
        Employer = serviceResult.Data;
        
        var ratingsServiceResult = await employerRatingService.GetAllAsync(EmployerId, _clientData.AccessToken);
        if (ratingsServiceResult.IsSuccess) EmployerRatings = ratingsServiceResult.Data.ToList();
        
        var averageRatingServiceResult = await employerRatingService.GetAverageRatingAsync(EmployerId);
        if (averageRatingServiceResult.IsSuccess) AverageRating = averageRatingServiceResult.Data;
        
        return Page();
    }
}