using Ergasia_WebApp.DTOs.User;

namespace Ergasia_WebApp.DTOs.Employer;

public class EmployerDto : UserDto
{
    public string? CompanyName { get; set; }
    public string? CompanyState { get; set; }
    public string? CompanyCity { get; set; }
    public string? CompanyAddress { get; set; }
}