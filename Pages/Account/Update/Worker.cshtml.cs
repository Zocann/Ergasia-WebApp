using System.Net;
using Ergasia_WebApp.Data;
using Ergasia_WebApp.DTOs.Worker;
using Ergasia_WebApp.Services.Interfaces;
using Ergasia_WebApp.Services.Model.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Ergasia_WebApp.Pages.Account.Update;

public class Worker(IWorkerService workerService, ICookieService cookieService) : PageModel
{
    private ClientData _clientData = new(new HttpContextAccessor());
    [BindProperty(SupportsGet = true)] public required string WorkerId { get; set; }
    public required WorkerDto WorkerDto { get; set; }
    public string? Error;

    
    public async Task<IActionResult> OnGetAsync(string? error)
    {
        if (_clientData.AccessToken == null) return Unauthorized();

        var serviceResult = await workerService.GetAsync(WorkerId, _clientData.AccessToken);
        if (! serviceResult.IsSuccess)
        {
            if (serviceResult.StatusCode == HttpStatusCode.Unauthorized) return Unauthorized();
            return RedirectToPage("/Error");
        }

        WorkerDto = serviceResult.Data;
        Error = error;
        return Page();
    }
    
    public async Task<IActionResult> OnPostAsync(WorkerDto workerDto, string date)
    {
        if (!DateTime.TryParse(date, out var dateOfBirth)) return RedirectToAction(nameof(OnGetAsync), new {error = "Invalid date of birth."});
        if (!ModelState.IsValid)
            return RedirectToAction(nameof(OnGetAsync), new {error = "Please follow form instructions."});
        if (_clientData.AccessToken == null) return Unauthorized();

        workerDto.DateOfBirth = dateOfBirth;
        var serviceResult = await workerService.PatchAsync(workerDto, _clientData.AccessToken);

        if (serviceResult.IsSuccess)
        {
            cookieService.AddCookie("userName", $"{workerDto.FirstName} {workerDto.LastName}");
            return RedirectToAction(nameof(OnGetAsync));
        }
        
        if (serviceResult.StatusCode == HttpStatusCode.Unauthorized) return Unauthorized();
        return RedirectToPage("/Error");
    }
}