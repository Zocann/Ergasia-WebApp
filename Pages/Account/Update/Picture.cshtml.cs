using Ergasia_WebApp.ApiRepositories.Interfaces;
using Ergasia_WebApp.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Ergasia_WebApp.Pages.Account.Update;

public class Picture(IUserApiRepository userApiRepository) : PageModel
{
    private ClientData _clientData = new(new HttpContextAccessor());
    public string? Error { get; set; }
    
    public void OnGet(string? error)
    {
        Error = error;
    }
    
    public async Task<IActionResult> OnPostAsync([FromForm]IFormFile file)
    {
        if (file.Length == 0) return RedirectToAction(nameof(OnGet), new {error = "Invalid file"});
        if (file.ContentType != "image/jpeg" && file.ContentType != "image/png") return RedirectToAction(nameof(OnGet), new {error = "Invalid file type. Available file types are jpg, jpeg, png"});
        
        var extension = Path.GetExtension(file.FileName);
        if (string.IsNullOrEmpty(extension)) return RedirectToAction(nameof(OnGet), new {error = "Invalid file extension"});
        
        if (_clientData.AccessToken == null) return Unauthorized();
        if (_clientData.Id == null) return RedirectToPage("/Error");
        
        var user = await userApiRepository.UploadAsync(file, _clientData.Id, _clientData.AccessToken);

        if (user == null)
        {
            if (Response.StatusCode == 401) return Unauthorized();
            return RedirectToPage("/Error");
        }

        return RedirectToPage("/Account/Index");
    }
}