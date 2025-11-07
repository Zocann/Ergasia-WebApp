using System.Net;
using System.Text.Json;
using Ergasia_WebApp.ApiRepositories.Interfaces;
using Ergasia_WebApp.DTOs.User;
using Ergasia_WebApp.Services;
using Ergasia_WebApp.Services.Interfaces;

namespace Ergasia_WebApp.Middleware;

public class TokenRefreshMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context, ICookieService cookieService, IHttpClientFactory clientFactory)
    {
        await next(context);
        var path = context.Request.Path;

        // If API returned 401 Unauthorized, try to get new access token
        if (context.Response.StatusCode == (int)HttpStatusCode.Unauthorized)
        {
            var refreshToken = context.Request.Cookies["refreshToken"];
            if (!string.IsNullOrEmpty(refreshToken))
            {
                var client = clientFactory.CreateClient("API");
                client.DefaultRequestHeaders.Add("Cookie", $"refreshToken={refreshToken}");
        
                var refreshResponse = await client.GetAsync("Users/refresh-token");
                if (refreshResponse.IsSuccessStatusCode)
                {
                    var responseString = await refreshResponse.Content.ReadAsStringAsync();
                    var user = JsonSerializerService.Deserialize<UserDto>(responseString);

                    if (user != null && user.RefreshToken != null && user.RefreshTokenExpiration != null)
                    {
                        //Get user role
                        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {user.AccessToken}");

                        var response = await client.GetAsync($"Users/role/{user.Id}");
                        if (!response.IsSuccessStatusCode)
                        {
                            context.Response.Redirect("/Error");
                        }

                        responseString = await response.Content.ReadAsStringAsync();

                        if (string.IsNullOrEmpty(responseString)) context.Response.Redirect("/Error");

                        // Update cookies
                        if (cookieService == null) throw new InvalidOperationException("No cookie service found");

                        cookieService.AddCookie("userRole", responseString, (DateTime)user.RefreshTokenExpiration);

                        cookieService.AddCookie("refreshToken", user.RefreshToken,
                            (DateTime)user.RefreshTokenExpiration);
                        if (user.AccessToken != null) cookieService.AddCookie("accessToken", user.AccessToken);
                        cookieService.AddCookie("userId", user.Id, (DateTime)user.RefreshTokenExpiration);
                        cookieService.AddCookie("userName", $"{user.FirstName} {user.LastName}",
                            (DateTime)user.RefreshTokenExpiration);


                        context.Response.Redirect(path);
                    }
                }
                else context.Response.Redirect("/Account/Logout");
            }
            else context.Response.Redirect("/Account/Logout");
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