using Ergasia_WebApp.Data;
using Ergasia_WebApp.DTOs.Rating;

namespace Ergasia_WebApp.Services.Model.Interfaces;

public interface IEmployerRatingService
{
    public Task<ServiceResult<EmployerRatingDto>> GetAsync(string employerId, string workerId, string accessToken);
    public Task<ServiceResult<IEnumerable<EmployerRatingDto>>> GetAllAsync(string employerId, string accessToken);
    public Task<ServiceResult<float>> GetAverageRatingAsync(string employerId);
    public Task<ServiceResult<EmployerRatingDto>> PostAsync(RatingDto ratingDto, string accessToken);
    public Task<ServiceResult<EmployerRatingDto>> PatchAsync(RatingDto ratingDto, string accessToken);
    public Task<ServiceResult<bool>> DeleteAsync(string employerId, string workerId, string accessToken);
}