using Ergasia_WebApp.ApiRepositories.Interfaces;
using Ergasia_WebApp.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Ergasia_WebApp.Pages.Account.Update;

public class Picture(IUserApiRepository userApiRepository) : PageModel
{
    private ClientData _clientData = new(new HttpContextAccessor());
    public List<string> Errors { get; set; } = [];
    
    public void OnGet()
    {
        
    }
    
    public async Task<IActionResult> OnPostAsync([FromForm]IFormFile file)
    {
        if (file.Length == 0)
        {
            Errors.Add("Please provide a file.");
            return Page();
        }

        if (file.ContentType != "image/jpeg" && file.ContentType != "image/png")
        {
            Errors.Add("Invalid file type.");
            return Page();
        }
        
        var extension = Path.GetExtension(file.FileName);

        if (string.IsNullOrEmpty(extension))
        {
            Errors.Add("Invalid file extension.");
            return Page();
        }



        if (!_clientData.GetAccessToken()) return Unauthorized();
        if (!_clientData.GetId()) return RedirectToPage("/Error");
        
        var user = await userApiRepository.UploadAsync(file, _clientData.Id!, _clientData.AccessToken!);

        if (user == null)
        {
            if (Response.StatusCode == 401) return Unauthorized();
            return RedirectToPage("/Error");
        }

        return RedirectToPage("/Account/Index");
    }
}