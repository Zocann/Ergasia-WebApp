using Ergasia_WebApp.ApiRepositories.Interfaces;
using Ergasia_WebApp.Data;
using Ergasia_WebApp.DTOs.User;
using Ergasia_WebApp.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Ergasia_WebApp.Pages.Account;

public class Login(IUserApiRepository userApiRepository,ICookieService cookieService)
    : PageModel
{
    public void OnGet()
    {
        
    }
    
    public async Task<IActionResult> OnPostAsync(LoginDto loginDto)
    {
        
        if (!ModelState.IsValid) return RedirectToPage("/Index", new { error = "Invalid email or password" });
        
        var user = await userApiRepository.LoginAsync(loginDto);
        if (user == null) return RedirectToPage("/Index", new { error = "Invalid email or password" });
        
        //Saving cookies in browser
        if(user.RefreshToken == null || user.RefreshTokenExpiration == null) return RedirectToPage("/Error");

        if (user.AccessToken == null) return Unauthorized();
        
        var role = await userApiRepository.GetRoleAsync(user.Id, user.AccessToken);
        if (role == null) return RedirectToPage("/Error");
        
        cookieService.AddCookie("refreshToken",user.RefreshToken, (DateTime)user.RefreshTokenExpiration);
        if (user.AccessToken != null) cookieService.AddCookie("accessToken", user.AccessToken);
        cookieService.AddCookie("userId", user.Id, (DateTime)user.RefreshTokenExpiration);
        cookieService.AddCookie("userRole", role, (DateTime)user.RefreshTokenExpiration);
        cookieService.AddCookie("userName", $"{user.FirstName} {user.LastName}", (DateTime)user.RefreshTokenExpiration);

        return RedirectToPage("/Index");
    }
}