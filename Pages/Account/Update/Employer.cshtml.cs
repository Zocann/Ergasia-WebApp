using System.Net;
using Ergasia_WebApp.Data;
using Ergasia_WebApp.DTOs.Employer;
using Ergasia_WebApp.Services.Interfaces;
using Ergasia_WebApp.Services.Model.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Ergasia_WebApp.Pages.Account.Update;

public class Employer(IEmployerService employerService, ICookieService cookieService) : PageModel
{
    private ClientData _clientData = new(new HttpContextAccessor());
    [BindProperty(SupportsGet = true)] public required string EmployerId { get; set; }
    public required EmployerDto EmployerDto { get; set; }
    public string? Error;

    
    public async Task<IActionResult> OnGetAsync(string? error)
    {
        if (_clientData.AccessToken == null) return Unauthorized();

        var serviceResult = await employerService.GetAsync(EmployerId, _clientData.AccessToken);

        if (!serviceResult.IsSuccess)
        {
            if (serviceResult.StatusCode == HttpStatusCode.Unauthorized) return Unauthorized();
            return RedirectToPage("/Error");
        }

        EmployerDto = serviceResult.Data;
        Error = error;
        return Page();
    }
    
    public async Task<IActionResult> OnPostAsync(EmployerDto employerDto, string date)
    {
        if (! DateTime.TryParse(date, out var dateOfBirth)) return RedirectToAction(nameof(OnGetAsync), new {error = "Invalid date of birth."});
        if (! ModelState.IsValid)
            return RedirectToAction(nameof(OnGetAsync), new {error = "Please follow form instructions."});
        if (_clientData.AccessToken == null) return Unauthorized();

        employerDto.DateOfBirth = dateOfBirth;
        var serviceResult = await employerService.PatchAsync(employerDto, _clientData.AccessToken);

        if (serviceResult.IsSuccess)
        {
            cookieService.AddCookie("userName", $"{employerDto.FirstName} {employerDto.LastName}");
            return RedirectToAction(nameof(OnGetAsync));
        }
        
        if (serviceResult.StatusCode == HttpStatusCode.Unauthorized) return Unauthorized();
        return RedirectToPage("/Error");

    }
}