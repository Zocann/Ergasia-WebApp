using Ergasia_WebApp.Data;
using Ergasia_WebApp.DTOs.User;
using Ergasia_WebApp.Services.Interfaces;
using Ergasia_WebApp.Services.Model.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Ergasia_WebApp.Pages.Account;

public class Login(IUserService userService,ICookieService cookieService)
    : PageModel
{
    public void OnGet()
    {
        
    }
    
    public async Task<IActionResult> OnPostAsync(LoginDto loginDto)
    {
        
        if (!ModelState.IsValid) return RedirectToPage("/Index", new { error = "Invalid email or password" });
        
        var userServiceResult = await userService.LoginAsync(loginDto);
        if (! userServiceResult.IsSuccess) return RedirectToPage("/Index", new { error = "Invalid email or password" });
        
        var user = userServiceResult.Data;
        //Saving cookies in browser
        if(user.RefreshToken == null || user.RefreshTokenExpiration == null) return RedirectToPage("/Error");
        if (user.AccessToken == null) return Unauthorized();
        
        var roleServiceResult = await userService.GetRoleAsync(user.Id, user.AccessToken);
        if (! roleServiceResult.IsSuccess) return RedirectToPage("/Error");
        var role = roleServiceResult.Data;
        
        AddCookiesToBrowser(user, role);
        return RedirectToPage("/Index");
    }

    private void AddCookiesToBrowser(UserDto userDto, string role)
    {
        cookieService.AddCookie("refreshToken",userDto.RefreshToken!, (DateTime)userDto.RefreshTokenExpiration!);
        if (userDto.AccessToken != null) cookieService.AddCookie("accessToken", userDto.AccessToken);
        cookieService.AddCookie("userId", userDto.Id, (DateTime)userDto.RefreshTokenExpiration);
        cookieService.AddCookie("userRole", role, (DateTime)userDto.RefreshTokenExpiration);
        cookieService.AddCookie("userName", $"{userDto.FirstName} {userDto.LastName}", (DateTime)userDto.RefreshTokenExpiration);
    }
}