using Ergasia_WebApp.DTOs.Employer;

namespace Ergasia_WebApp.ApiRepositories.Interfaces;

public interface IEmployerApiRepository
{
    public Task<EmployerDto?> GetAsync(string employerId, string accessToken);
    public Task<EmployerDto?> PatchAsync(EmployerDto employerDto, string accessToken);
}