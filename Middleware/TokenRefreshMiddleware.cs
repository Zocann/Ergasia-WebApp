using System.Net;
using Ergasia_WebApp.ApiRepositories.Interfaces;
using Ergasia_WebApp.Services.Interfaces;

namespace Ergasia_WebApp.Middleware;

public class TokenRefreshMiddleware(RequestDelegate next, IUserApiRepository userRepository)
{
    public async Task InvokeAsync(HttpContext context, ICookieService cookieService)
    {
        await next(context);
        var path = context.Request.Path;

        // If API returned 401 Unauthorized, try to get new access token
        if (context.Response.StatusCode == (int)HttpStatusCode.Unauthorized)
        {
            var refreshToken = context.Request.Cookies["refreshToken"];
            if (!string.IsNullOrEmpty(refreshToken))
            {
                var user = await userRepository.RefreshTokensAsync(refreshToken);
                if (user == null) return;

                if (user.RefreshToken != null && user.RefreshTokenExpiration != null && user.AccessToken != null)
                {
                    //Get user role
                    var role = await userRepository.GetRoleAsync(user.Id, user.AccessToken);

                    if (string.IsNullOrEmpty(role))
                    {
                        context.Response.Redirect("/Account/Logout");
                        return;
                    }

                    // Update cookies
                    cookieService.AddCookie("userRole", role, (DateTime)user.RefreshTokenExpiration);
                    cookieService.AddCookie("refreshToken", user.RefreshToken, (DateTime)user.RefreshTokenExpiration);
                    cookieService.AddCookie("accessToken", user.AccessToken);
                    cookieService.AddCookie("userId", user.Id, (DateTime)user.RefreshTokenExpiration);
                    cookieService.AddCookie("userName", $"{user.FirstName} {user.LastName}", (DateTime)user.RefreshTokenExpiration);


                    context.Response.Redirect(path);
                }
                else context.Response.Redirect("/Account/Logout");
            }
        }
    }
}

public static class TokenRefreshMiddlewareExtensions
{
    public static IApplicationBuilder UseTokenRefresh(
        this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<TokenRefreshMiddleware>();
    }
}