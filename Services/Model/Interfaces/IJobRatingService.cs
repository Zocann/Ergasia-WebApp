using Ergasia_WebApp.Data;
using Ergasia_WebApp.DTOs.Job;
using Ergasia_WebApp.DTOs.Rating;

namespace Ergasia_WebApp.Services.Model.Interfaces;

public interface IJobRatingService
{
    public Task<ServiceResult<float>> GetAverageRatingAsync(string jobId);
    public Task<ServiceResult<WorkerJobDto>> PatchAsync(JobRatingDto ratingDto, string employerId, string accessToken);
    public Task<ServiceResult<bool>> DeleteAsync(string employerId, string jobId, string workerId, string accessToken);
}