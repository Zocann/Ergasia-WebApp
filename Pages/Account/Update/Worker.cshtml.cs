using Ergasia_WebApp.ApiRepositories.Interfaces;
using Ergasia_WebApp.Data;
using Ergasia_WebApp.DTOs.Worker;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Ergasia_WebApp.Pages.Account.Update;

public class Worker(IWorkerApiRepository workerApiRepository) : PageModel
{
    private ClientData _clientData = new(new HttpContextAccessor());
    
    [BindProperty(SupportsGet = true)] 
    public required string WorkerId { get; set; }
    public required WorkerDto WorkerDto { get; set; }
    public string? Error;

    
    public async Task<IActionResult> OnGetAsync(string? error)
    {
        if (!_clientData.GetAccessToken()) return Unauthorized();

        var worker = await workerApiRepository.GetAsync(WorkerId, _clientData.AccessToken);
        if (worker == null)
        {
            if (Response.StatusCode == 401) return Unauthorized();
            return RedirectToPage("/Error");
        }
        WorkerDto = worker;
        Error = error;
        return Page();
    }
    
    public async Task<IActionResult> OnPostAsync(WorkerDto workerDto, string date)
    {
        if (!DateTime.TryParse(date, out var dateOfBirth)) return RedirectToAction(nameof(OnGetAsync), new {error = "Invalid date of birth."});
        if (!ModelState.IsValid)
            return RedirectToAction(nameof(OnGetAsync), new {error = "Please follow form instructions."});
        if (!_clientData.GetAccessToken()) return Unauthorized();

        workerDto.DateOfBirth = dateOfBirth;
        var worker = await workerApiRepository.PatchAsync(workerDto, _clientData.AccessToken);
        if (worker == null)
        {
            if (Response.StatusCode == 401) return Unauthorized();
            return RedirectToPage("/Error");
        }
        
        return RedirectToAction(nameof(OnGetAsync));
    }
}