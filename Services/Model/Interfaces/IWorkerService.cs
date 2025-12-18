using Ergasia_WebApp.Data;
using Ergasia_WebApp.DTOs.Worker;

namespace Ergasia_WebApp.Services.Model.Interfaces;

public interface IWorkerService
{
    public Task<ServiceResult<WorkerDto>> GetAsync(string workerId, string accessToken);
    public Task<ServiceResult<IEnumerable<WorkerDto>>> GetAllAsync(string accessToken);
    public Task<ServiceResult<WorkerDto>> PatchAsync(WorkerDto workerDto, string accessToken);
}