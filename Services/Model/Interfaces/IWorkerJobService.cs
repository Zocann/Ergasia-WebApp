using Ergasia_WebApp.Data;
using Ergasia_WebApp.DTOs.Job;

namespace Ergasia_WebApp.Services.Model.Interfaces;

public interface IWorkerJobService
{
    public Task<ServiceResult<WorkerJobDto>> GetAsync(string jobId, string workerId, string accessToken);
    public Task<ServiceResult<IEnumerable<WorkerJobDto>>> GetByJobIdAsync(string jobId, string accessToken);
    public Task<ServiceResult<IEnumerable<WorkerJobDto>>> GetByWorkerIdAsync(string workerId, string accessToken);
    public Task<ServiceResult<WorkerJobDto>> PostAsync(string jobId, string employerId, string workerId, string accessToken);
    public Task<ServiceResult<int>> GetAvailableWorkSpotsAsync(string jobId, string accessToken);
}