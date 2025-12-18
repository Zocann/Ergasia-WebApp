using Ergasia_WebApp.Data;
using Ergasia_WebApp.DTOs.Job;

namespace Ergasia_WebApp.Services.Model.Interfaces;

public interface IJobRequestService
{
    public Task<ServiceResult<JobRequestDto>> GetAsync(string jobId, string workerId, string accessToken);
    public Task<ServiceResult<IEnumerable<JobRequestDto>>> GetByJobIdAsync(string jobId, string employerId, string accessToken);
    public Task<ServiceResult<IEnumerable<JobRequestDto>>> GetByWorkerIdAsync(string workerId, string accessToken);
    public Task<ServiceResult<JobRequestDto>> PostAsync(string jobId, string workerId, string? message, string accessToken);
    public Task<ServiceResult<bool>> DeleteAsync(string jobId, string employerId, string workerId, string accessToken);
}