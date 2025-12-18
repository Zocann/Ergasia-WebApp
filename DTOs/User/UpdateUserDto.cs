using System.ComponentModel.DataAnnotations;
using Ergasia_WebApp.DTOs.Employer;
using Ergasia_WebApp.DTOs.Worker;
using Microsoft.AspNetCore.Mvc;

namespace Ergasia_WebApp.DTOs.User;

public class UpdateUserDto
{
    public UpdateUserDto(EmployerDto employer)
    {
        Id = employer.Id;
        FirstName = employer.FirstName;
        LastName = employer.LastName;
        PhoneNumber = employer.PhoneNumber;
        State = employer.State;
        City = employer.City;
        Address = employer.Address;
    }
    
    public UpdateUserDto(WorkerDto worker)
    {
        Id = worker.Id;
        FirstName = worker.FirstName;
        LastName = worker.LastName;
        PhoneNumber = worker.PhoneNumber;
        State = worker.State;
        City = worker.City;
        Address = worker.Address;
    }
    
    [HiddenInput] 
    public string Id { get; set; }
    
    [Required(ErrorMessage = "Please enter your first name")]
    [StringLength(16, ErrorMessage = "Must be between 3 and 16 characters", MinimumLength = 3)]
    [Display(Name = "First Name")]
    public string FirstName { get; set; }
    
    [Required(ErrorMessage = "Please enter your last name")]
    [StringLength(16, ErrorMessage = "Must be between 3 and 16 characters", MinimumLength = 3)]
    [Display(Name = "Last Name")]
    public string LastName { get; set; }
    
    [Phone] [Required]
    public string? PhoneNumber { get; set; }
    
    [Required(ErrorMessage = "Please enter state, where your company is located")]
    [StringLength(15, MinimumLength = 3, ErrorMessage = "State must be between 3 and 15 characters")]
    public string State { get; set; }
    
    [Required(ErrorMessage = "Please enter city, where your company is located")]
    [StringLength(15, MinimumLength = 3, ErrorMessage = "City must be between 3 and 15 characters")]
    public string City { get; set; }
    
    [Required(ErrorMessage = "Please enter state, where your company is located")]
    [StringLength(30, MinimumLength = 3, ErrorMessage = "Address must be between 3 and 30 characters")]
    public string Address { get; set; }
}