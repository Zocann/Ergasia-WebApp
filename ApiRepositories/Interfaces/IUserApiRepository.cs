using Ergasia_WebApp.DTOs.User;

namespace Ergasia_WebApp.ApiRepositories.Interfaces;

public interface IUserApiRepository
{
    public Task<UserDto?> GetAsync(string userId, string accessToken);
    public Task<bool?> ValidateEmailAsync(string email);
    public Task<string?> GetRoleAsync(string userId, string accessToken);
    public Task<UserDto?> LoginAsync(LoginDto loginDto);
    public Task<UserDto?> RegisterAsync(RegisterDto registerDto, string role);
    public Task<UserDto?> UploadPictureAsync(IFormFile file, string userId, string accessToken);
    
    public Task<UserDto?> RefreshTokensAsync(string refreshToken);
}