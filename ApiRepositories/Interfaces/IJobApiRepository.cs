using Ergasia_WebApp.DTOs.Job;

namespace Ergasia_WebApp.ApiRepositories.Interfaces;

public interface IJobApiRepository
{
    //Job
    public Task<JobDto?> GetAsync(string jobId, string accessToken);
    public Task<List<JobDto>?> GetFromEmployerAsync(string employerId, string accessToken);
    public Task<List<JobDto>?> GetAllUpcomingAsync();
    public Task<JobDto?> AddAsync(JobDto jobDto, string accessToken);
    public Task<JobDto?> PatchAsync(JobDto jobDto, string accessToken);
    
    //WorkerJob
    public Task<WorkerJobDto?> GetWorkerJobAsync(string jobId, string workerId, string accessToken);
    public Task<List<WorkerJobDto>?> GetWorkerJobsByJobIdAsync(string jobId, string accessToken);
    public Task<List<WorkerJobDto>?> GetWorkerJobsByWorkerIdAsync(string workerId, string accessToken);
    public Task<WorkerJobDto?> PostWorkerJobAsync(string jobId, string employerId, string workerId, string accessToken);
    public Task<int?> GetAvailableWorkSpotsAsync(string jobId, string accessToken);
    
    //Job request
    public Task<JobRequestDto?> GetJobRequestAsync(string jobId, string workerId, string accessToken);
    public Task<List<JobRequestDto>?> GetJobRequestsByJobIdAsync(string jobId, string employerId, string accessToken);
    public Task<List<JobRequestDto>?> GetJobRequestsByWorkerIdAsync(string workerId, string accessToken);
    public Task<JobRequestDto?> PostJobRequestAsync(string jobId, string workerId, string? message, string accessToken);
    public Task<bool> DeleteJobRequest(string jobId, string employerId, string workerId, string accessToken);
}