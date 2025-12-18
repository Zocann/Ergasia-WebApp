using System.ComponentModel.DataAnnotations;
using System.Net;
using Ergasia_WebApp.Data;
using Ergasia_WebApp.DTOs.Employer;
using Ergasia_WebApp.DTOs.Rating;
using Ergasia_WebApp.Services.Model.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Ergasia_WebApp.Pages.Employers;

public class AddReview(IEmployerService employerService, IEmployerRatingService employerRatingService) : PageModel
{
    private ClientData _clientData = new(new HttpContextAccessor());

    [BindProperty(SupportsGet = true)] 
    public required string EmployerId { get; set; }
    public EmployerRatingDto? Rating { get; set; }
    public required EmployerDto Employer { get; set; }
    public string? Error;


    public async Task<IActionResult> OnGetAsync(string? error)
    {
        if (_clientData.AccessToken == null) return Unauthorized();
        if (_clientData.Id == null) return RedirectToPage("/Error");

        var serviceResult = await employerService.GetAsync(EmployerId, _clientData.AccessToken);
        if (! serviceResult.IsSuccess)
        {
            if (serviceResult.StatusCode == HttpStatusCode.Unauthorized) return Unauthorized();
            return RedirectToPage("/Error");
        }

        Employer = serviceResult.Data;
        
        var ratingServiceResult = await employerRatingService.GetAsync(EmployerId, _clientData.Id, _clientData.AccessToken);
        if (ratingServiceResult.IsSuccess) Rating = ratingServiceResult.Data;
        Error = error;
        
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(string employerId, [Required][Range(1, 5)] int rating, 
        string verRating, string action, string? delete)
    {
        if (!ModelState.IsValid) 
            return RedirectToAction(nameof(OnGetAsync), new {error = "Please provide a valid rating"});
        if (_clientData.AccessToken == null) return Unauthorized();
        if (_clientData.Id == null) return RedirectToPage("/Error");

        if (delete == "true")
        {
            return await HandleDeleteAsync(EmployerId, _clientData.Id, _clientData.AccessToken);
        }
        
        var ratingDto = new RatingDto(employerId, _clientData.Id, rating, verRating);

        return action switch
        {
            "post" => await HandlePostAsync(ratingDto, _clientData.AccessToken),
            "update" => await HandleUpdateAsync(ratingDto, _clientData.AccessToken),
            _ => RedirectToPage("/Error")
        };
    }

    private async Task<IActionResult> HandleDeleteAsync(string employerId, string clientId, string accessToken)
    {
        var serviceResult = await employerRatingService.DeleteAsync(employerId, clientId, accessToken);
        if (serviceResult.IsSuccess) return RedirectToAction(nameof(OnGetAsync));
        
        if (serviceResult.StatusCode == HttpStatusCode.Unauthorized) return Unauthorized();
        return RedirectToPage("/Error");
    }

    private async Task<IActionResult> HandlePostAsync(RatingDto ratingDto, string accessToken)
    {
        var serviceResult = await employerRatingService.PostAsync(ratingDto, accessToken);
        if (serviceResult.IsSuccess) return RedirectToAction(nameof(OnGetAsync));
        
        if (serviceResult.StatusCode == HttpStatusCode.Unauthorized) return Unauthorized();
        return RedirectToPage("/Error");
    }
    
    private async Task<IActionResult> HandleUpdateAsync(RatingDto ratingDto, string accessToken)
    {
        var serviceResult = await employerRatingService.PatchAsync(ratingDto, accessToken);
        if (serviceResult.IsSuccess) return RedirectToAction(nameof(OnGetAsync));
        
        if (serviceResult.StatusCode == HttpStatusCode.Unauthorized) return Unauthorized();
        return RedirectToPage("/Error");
    }
}