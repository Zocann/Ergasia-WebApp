using Ergasia_WebApp.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Ergasia_WebApp.Pages.Account;

public class Logout(ICookieService cookieService) : PageModel
{
    public IActionResult OnGet()
    {
        if (Request.Cookies["refreshToken"] != null) cookieService.RevokeCookie("refreshToken");
        if (Request.Cookies["accessToken"] != null) cookieService.RevokeCookie("accessToken");
        if (Request.Cookies["userId"] != null) cookieService.RevokeCookie("userId");
        if (Request.Cookies["userRole"] != null) cookieService.RevokeCookie("userRole");
        if (Request.Cookies["userName"] != null) cookieService.RevokeCookie("userName");
        
        return RedirectToPage("/Index");
    }
}