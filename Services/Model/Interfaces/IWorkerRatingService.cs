using Ergasia_WebApp.Data;
using Ergasia_WebApp.DTOs.Rating;

namespace Ergasia_WebApp.Services.Model.Interfaces;

public interface IWorkerRatingService
{
    public Task<ServiceResult<WorkerRatingDto>> GetAsync(string workerId, string employerId, string accessToken);
    public Task<ServiceResult<IEnumerable<WorkerRatingDto>>> GetAllAsync(string workerId, string accessToken);
    public Task<ServiceResult<float>> GetAverageRatingAsync(string workerId);
    public Task<ServiceResult<WorkerRatingDto>> PostAsync(RatingDto ratingDto, string accessToken);
    public Task<ServiceResult<WorkerRatingDto>> PatchAsync(RatingDto ratingDto, string accessToken);
    public Task<ServiceResult<bool>> DeleteAsync(string workerId, string employerId, string accessToken);
}