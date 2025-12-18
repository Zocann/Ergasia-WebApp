using System.Globalization;
using System.Text.RegularExpressions;
using Ergasia_WebApp.DTOs.User;
using Ergasia_WebApp.Services.Interfaces;
using Ergasia_WebApp.Services.Model.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Ergasia_WebApp.Pages.Account;

[IgnoreAntiforgeryToken]
public class Register(IUserService userService, ICookieService cookieService)
    : PageModel
{
    public string? Error;

    public void OnGet(string? error)
    {
        Error = error;
    }

    public async Task<IActionResult> OnPostAsync(RegisterDto registerDto, string date, string role)
    {
        if (!DateTime.TryParse(date, out var dateOfBirth))
            return RedirectToAction(nameof(OnGet), new { error = "Invalid date of birth." });
        if (!ModelState.IsValid)
            return RedirectToAction(nameof(OnGet), new { error = "Please follow form instructions." });

        registerDto.DateOfBirth = dateOfBirth;

        //Validate if email is not already taken
        var emailServiceResult = await userService.ValidateEmailAsync(registerDto.Email);
        if (! emailServiceResult.IsSuccess) return RedirectToPage("/Error");
        if (emailServiceResult.Data == false) return RedirectToAction(nameof(OnGet), new { error = "This email address is already taken." });

        if (!IsValidPassword(registerDto.Password))
            return RedirectToAction(nameof(OnGet),
                new {
                    error =
                        "Invalid password. Please enter at least 8 characters with 1 lowercase and 1 uppercase letter, 1 digit and 1 special character." });

        registerDto = ConvertRegisterDtoToTitleCase(registerDto);
        
        var userServiceResult = await userService.RegisterAsync(registerDto, role);
        if (!userServiceResult.IsSuccess) return RedirectToPage("/Error");
        
        var user = userServiceResult.Data;
        if (user.AccessToken == null) return Unauthorized();
        if (user.RefreshToken == null || user.RefreshTokenExpiration == null) return RedirectToPage("/Error");
        
        AddCookiesToBrowser(user, role);
        
        return RedirectToPage("/Account/Update/Picture");
    }


    private static bool IsValidPassword(string password)
    {
        // At least 8 chars, 1 lowercase, 1 uppercase, 1 digit, 1 special char
        var regex = new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^a-zA-Z\d]).{8,}$");
        return regex.IsMatch(password);
    }

    private static RegisterDto ConvertRegisterDtoToTitleCase(RegisterDto registerDto)
    {
        var textInfo = CultureInfo.CurrentCulture.TextInfo;
        registerDto.FirstName = textInfo.ToTitleCase(registerDto.FirstName);
        registerDto.LastName = textInfo.ToTitleCase(registerDto.LastName);
        registerDto.State = textInfo.ToTitleCase(registerDto.State);
        registerDto.City = textInfo.ToTitleCase(registerDto.City);
        registerDto.Address = textInfo.ToTitleCase(registerDto.Address);
        return registerDto;
    }

    private void AddCookiesToBrowser(UserDto userDto, string role)
    {
        cookieService.AddCookie("refreshToken", userDto.RefreshToken!, (DateTime)userDto.RefreshTokenExpiration!);
        if (userDto.AccessToken != null) cookieService.AddCookie("accessToken", userDto.AccessToken);
        cookieService.AddCookie("userId", userDto.Id, (DateTime)userDto.RefreshTokenExpiration);
        cookieService.AddCookie("userRole", role, (DateTime)userDto.RefreshTokenExpiration);
        cookieService.AddCookie("userName", $"{userDto.FirstName} {userDto.LastName}",
            (DateTime)userDto.RefreshTokenExpiration);
    }
}