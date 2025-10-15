using Ergasia_WebApp.DTOs.User;

namespace Ergasia_WebApp.DTOs.Worker;

public class WorkerDto : UserDto
{
    public int? MinimalSalary { get; set; }
    public string? Description { get; set; }
}