using Ergasia_WebApp.Data;
using Ergasia_WebApp.DTOs.User;

namespace Ergasia_WebApp.Services.Model.Interfaces;

public interface IUserService
{
    public Task<ServiceResult<UserDto>> GetAsync(string userId, string accessToken);
    public Task<ServiceResult<bool>> ValidateEmailAsync(string email);
    public Task<ServiceResult<string>> GetRoleAsync(string userId, string accessToken);
    public Task<ServiceResult<UserDto>> LoginAsync(LoginDto loginDto);
    public Task<ServiceResult<UserDto>> RegisterAsync(RegisterDto registerDto, string role);
    public Task<ServiceResult<bool>> UploadPictureAsync(IFormFile file, string userId, string accessToken);
    
    public Task<ServiceResult<UserDto>> RefreshTokenAsync(string refreshToken);
}