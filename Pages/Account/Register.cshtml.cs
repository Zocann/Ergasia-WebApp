using System.Globalization;
using System.Text.RegularExpressions;
using Ergasia_WebApp.ApiRepositories.Interfaces;
using Ergasia_WebApp.DTOs.User;
using Ergasia_WebApp.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Ergasia_WebApp.Pages.Account;

[IgnoreAntiforgeryToken]
public class Register(IUserApiRepository userApiRepository, ICookieService cookieService, IPasswordValidator<IdentityUser> passwordValidator)
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
        
        if (!ValidatePassword(registerDto.Password)) return RedirectToAction(nameof(OnGet), new {error = "Invalid password. Please enter at least 8 characters with 1 lowercase and 1 uppercase letter, 1 digit and 1 special character."});
        
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
    
    
    private bool ValidatePassword(string password)
    {
        // At least 8 chars, 1 lowercase, 1 uppercase, 1 digit, 1 special char
        var regex = new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$");
        return regex.IsMatch(password);
    }
}