using Ergasia_WebApp.Data;
using Ergasia_WebApp.DTOs.Job;

namespace Ergasia_WebApp.Services.Model.Interfaces;

public interface IJobService
{
    public Task<ServiceResult<JobDto>> GetAsync(string jobId, string accessToken);
    public Task<ServiceResult<IEnumerable<JobDto>>> GetFromEmployerAsync(string employerId, string accessToken);
    public Task<ServiceResult<IEnumerable<JobDto>>> GetAllUpcomingAsync();
    public Task<ServiceResult<JobDto>> AddAsync(JobDto jobDto, string accessToken);
    public Task<ServiceResult<JobDto>> PatchAsync(JobDto jobDto, string accessToken);
}