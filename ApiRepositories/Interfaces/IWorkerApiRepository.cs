using Ergasia_WebApp.DTOs.Worker;

namespace Ergasia_WebApp.ApiRepositories.Interfaces;

public interface IWorkerApiRepository
{
    public Task<WorkerDto?> GetAsync(string workerId, string accessToken);
    public Task<IEnumerable<WorkerDto?>?> GetAllAsync(string accessToken);
    public Task<WorkerDto?> PatchAsync(WorkerDto workerDto, string accessToken);
}