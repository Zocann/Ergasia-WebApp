using Ergasia_WebApp.ApiRepositories.Interfaces;
using Ergasia_WebApp.Data;
using Ergasia_WebApp.DTOs.Employer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Ergasia_WebApp.Pages.Account.Update;

public class Employer(IEmployerApiRepository employerApiRepository) : PageModel
{
    private ClientData _clientData = new(new HttpContextAccessor());
    
    [BindProperty(SupportsGet = true)] 
    public required string EmployerId { get; set; }
    public required EmployerDto EmployerDto { get; set; }
    public string? Error;

    
    public async Task<IActionResult> OnGetAsync(string? error)
    {
        if (!_clientData.GetAccessToken()) return Unauthorized();

        var employer = await employerApiRepository.GetAsync(EmployerId, _clientData.AccessToken!);
        if (employer == null)
        {
            if (Response.StatusCode == 401) return Unauthorized();
            return RedirectToPage("/Error");
        }
        EmployerDto = employer;
        Error = error;
        return Page();
    }
    
    public async Task<IActionResult> OnPostAsync(EmployerDto employerDto, string date)
    {
        if (!DateTime.TryParse(date, out var dateOfBirth)) return RedirectToAction(nameof(OnGetAsync), new {error = "Invalid date of birth."});
        employerDto.DateOfBirth = dateOfBirth;
        
        if (!ModelState.IsValid)
        {
            return RedirectToAction(nameof(OnGetAsync), new {error = "Please follow form instructions."});
        }
        
        if (!_clientData.GetAccessToken()) return Unauthorized();
        
        employerDto.DateOfBirth = DateTime.Parse(date);

        var employer = await employerApiRepository.PatchAsync(employerDto, _clientData.AccessToken!);
        if (employer == null)
        {
            if (Response.StatusCode == 401) return Unauthorized();
            return RedirectToPage("/Error");
        }
        
        return RedirectToAction(nameof(OnGetAsync));
    }
}