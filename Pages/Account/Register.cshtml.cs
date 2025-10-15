using System.Globalization;
using Ergasia_WebApp.ApiRepositories.Interfaces;
using Ergasia_WebApp.DTOs.User;
using Ergasia_WebApp.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Ergasia_WebApp.Pages.Account;

[IgnoreAntiforgeryToken]
public class Register(IUserApiRepository userApiRepository, ICookieService cookieService)
    : PageModel
{
    public string? Error;
    public void OnGet(string? error)
    {
        Error = error;
    }

    public async Task<IActionResult> OnPostAsync(RegisterDto registerDto, string date, string role)
    {
        if (!DateTime.TryParse(date, out var dateOfBirth)) return RedirectToAction(nameof(OnGet), new {error = "Invalid date of birth."});
        registerDto.DateOfBirth = dateOfBirth;
        
        if (!ModelState.IsValid)
        {
            return RedirectToAction(nameof(OnGet), new {error = "Please follow form instructions."});
        }

        //Validate if email is not already taken
        if (registerDto.Email != null)
        {
            var validEmail = await userApiRepository.ValidateEmailAsync(registerDto.Email);
            switch (validEmail)
            {
                case null:
                    return RedirectToPage("/Error");
                case false:
                    return RedirectToAction(nameof(OnGet), new {error = "This email address is already taken."});
            }
        }
        
        else return RedirectToPage("/Error");
        
        var textInfo = CultureInfo.CurrentCulture.TextInfo;
        registerDto.FirstName = textInfo.ToTitleCase(registerDto.FirstName);
        registerDto.LastName = textInfo.ToTitleCase(registerDto.LastName);
        registerDto.State = textInfo.ToTitleCase(registerDto.State);
        registerDto.City = textInfo.ToTitleCase(registerDto.City);
        registerDto.Address = textInfo.ToTitleCase(registerDto.Address);

        var user = await userApiRepository.RegisterAsync(registerDto, role);
        
        if (user?.AccessToken == null) return Unauthorized();
        
        //Sending refresh token via cookie
        if(user?.RefreshToken == null || user.RefreshTokenExpiration == null) return RedirectToPage("/Error");
        
        cookieService.AddCookie("refreshToken",user.RefreshToken, (DateTime)user.RefreshTokenExpiration);
        if (user.AccessToken != null) cookieService.AddCookie("accessToken", user.AccessToken);
        cookieService.AddCookie("userId", user.Id, (DateTime)user.RefreshTokenExpiration);
        cookieService.AddCookie("userRole", role, (DateTime)user.RefreshTokenExpiration);
        cookieService.AddCookie("userName", $"{user.FirstName} {user.LastName}", (DateTime)user.RefreshTokenExpiration);

        return RedirectToPage("/Account/Update/Picture");
    }
}