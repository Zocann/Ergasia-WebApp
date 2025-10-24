using Ergasia_WebApp.DTOs.Job;
using Ergasia_WebApp.DTOs.Rating;

namespace Ergasia_WebApp.ApiRepositories.Interfaces;

public interface IRatingApiRepository
{
    //Employer
    public Task<EmployerRatingDto?> GetEmployerRatingAsync(string employerId, string workerId, string accessToken);
    public Task<List<EmployerRatingDto>?> GetEmployerRatingsAsync(string employerId, string accessToken);
    public Task<decimal?> GetEmployerAverageRating(string employerId);
    public Task<EmployerRatingDto?> PostEmployerRatingAsync(string employerId, string workerId, string numRating,
        string verbalRating, string accessToken);
    public Task<EmployerRatingDto?> PatchEmployerRatingAsync(string employerId, string workerId, string numRating,
        string verbalRating, string accessToken);
    public Task<bool> DeleteEmployerRatingAsync(string employerId, string workerId, string accessToken);
    
    //Worker
    public Task<WorkerRatingDto?> GetWorkerRatingAsync(string workerId, string employerId, string accessToken);
    public Task<List<WorkerRatingDto>?> GetWorkerRatingsAsync(string workerId, string accessToken);
    public Task<decimal?> GetWorkerAverageRating(string workerId);
    public Task<WorkerRatingDto?> PostWorkerRatingAsync(string workerId, string employerId, string numRating,
        string verbalRating, string accessToken);
    public Task<WorkerRatingDto?> PatchWorkerRatingAsync(string workerId, string employerId, string numRating,
        string verbalRating, string accessToken);
    public Task<bool> DeleteWorkerRatingAsync(string workerId, string employerId, string accessToken);
    
    //Job
    public Task<decimal?> GetJobAverageRating(string jobId);
    public Task<WorkerJobDto?> PatchJobRatingAsync(string employerId, string jobId, string workerId,
        string numRating,
        string verbalRating, string accessToken);
    public Task<bool> DeleteJobRatingAsync(string employerId, string jobId, string workerId, string accessToken);
}