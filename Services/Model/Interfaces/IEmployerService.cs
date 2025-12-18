using Ergasia_WebApp.Data;
using Ergasia_WebApp.DTOs.Employer;

namespace Ergasia_WebApp.Services.Model.Interfaces;

public interface IEmployerService
{
    public Task<ServiceResult<EmployerDto>> GetAsync(string employerId, string accessToken);
    public Task<ServiceResult<EmployerDto>> PatchAsync(EmployerDto employerDto, string accessToken);
}