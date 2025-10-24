using System.ComponentModel.DataAnnotations;
using Ergasia_WebApp.ApiRepositories.Interfaces;
using Ergasia_WebApp.Data;
using Ergasia_WebApp.DTOs.Employer;
using Ergasia_WebApp.DTOs.Rating;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Ergasia_WebApp.Pages.Employers;

public class AddReview(IEmployerApiRepository employerApiRepository, IRatingApiRepository ratingApiRepository) : PageModel
{
    private ClientData _clientData = new(new HttpContextAccessor());

    [BindProperty(SupportsGet = true)] 
    public required string EmployerId { get; set; }
    public EmployerRatingDto? Rating { get; set; }
    public required EmployerDto Employer { get; set; }
    public string? Error;


    public async Task<IActionResult> OnGetAsync(string? error)
    {
        if (!_clientData.GetAccessToken()) return Unauthorized();
        if (!_clientData.GetId()) return RedirectToPage("/Error");

        var employer = await employerApiRepository.GetAsync(EmployerId, _clientData.AccessToken);
        if (employer == null)
        {
            if (Response.StatusCode == 401) return Unauthorized();
            return RedirectToPage("/Error");
        }

        Employer = employer;
        Rating = await ratingApiRepository.GetEmployerRatingAsync(EmployerId, _clientData.Id, _clientData.AccessToken);
        Error = error;
        
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(string employerId, [Required][Range(1, 5)] string rating, 
        string verRating, string action, string? delete)
    {
        if (!ModelState.IsValid)
        {
            return RedirectToAction(nameof(OnGetAsync), new {error = "Please provide a valid rating"});
        }
        if (!_clientData.GetAccessToken()) return Unauthorized();
        if (!_clientData.GetId()) return RedirectToPage("/Error");

        if (delete == "true")
        {
            if (!await ratingApiRepository.DeleteEmployerRatingAsync(EmployerId, _clientData.Id, _clientData.AccessToken))
            {
                if (Response.StatusCode == 401) return Unauthorized();
                return RedirectToPage("/Error");
            }
            return RedirectToAction(nameof(OnGetAsync)); 
        }
        
        EmployerRatingDto? employerRating;
        
        switch (action)
        {
            case "post":
                employerRating = await ratingApiRepository.PostEmployerRatingAsync(employerId, _clientData.Id, rating, verRating, _clientData.AccessToken);

                if (employerRating == null)
                {
                    if (Response.StatusCode == 401) return Unauthorized();
                    return RedirectToPage("/Error");
                }
                break;

            case "update":
                employerRating = await ratingApiRepository.PatchEmployerRatingAsync(employerId, _clientData.Id, rating, verRating, _clientData.AccessToken);

                if (employerRating == null)
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