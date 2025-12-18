using System.Net;
using Ergasia_WebApp.Data;
using Ergasia_WebApp.Services.Model.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Ergasia_WebApp.Pages.Account.Update;

public class Picture(IUserService userService) : PageModel
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
        if (IsInvalidFileType(file)) return RedirectToAction(nameof(OnGet), new {error = "Invalid file type. Available file types are jpg, jpeg, png"});
        
        var extension = Path.GetExtension(file.FileName);
        if (string.IsNullOrEmpty(extension)) return RedirectToAction(nameof(OnGet), new {error = "Invalid file extension"});
        
        if (_clientData.AccessToken == null) return Unauthorized();
        if (_clientData.Id == null) return RedirectToPage("/Error");
        
        var serviceResult = await userService.UploadPictureAsync(file, _clientData.Id, _clientData.AccessToken);

        if (serviceResult.IsSuccess) return RedirectToPage("/Account/Index");
        
        if (serviceResult.StatusCode == HttpStatusCode.Unauthorized) return Unauthorized();
        return RedirectToPage("/Error");

    }

    private static bool IsInvalidFileType(IFormFile file)
    {
        return file.ContentType != "image/jpeg" && file.ContentType != "image/png";
    }
}