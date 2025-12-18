using System.ComponentModel.DataAnnotations;
using Ergasia_WebApp.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace Ergasia_WebApp.DTOs.User;

public class RegisterDto
{
    [Required(ErrorMessage = "Please enter your first name")]
    [StringLength(16, ErrorMessage = "Must be between 3 and 16 characters", MinimumLength = 3)]
    [Display(Name = "First Name")]
    public required string FirstName { get; set; }
    
    [Required(ErrorMessage = "Please enter your last name")]
    [StringLength(16, ErrorMessage = "Must be between 3 and 16 characters", MinimumLength = 3)]
    [Display(Name = "Last Name")]
    public required string LastName { get; set; }

    [Required (ErrorMessage = "Please enter password")]
    [StringLength(16, MinimumLength = 4, ErrorMessage = "Must be between 4 and 16 characters")]
    [DataType(DataType.Password)]
    public required string Password { get; set; }
    
    [EmailAddress]
    public required string Email { get; set; }
    
    [Phone] [Required]
    [Display(Name = "Phone number")]
    public required string PhoneNumber { get; set; }
    
    [Required(ErrorMessage = "Please enter state")]
    [StringLength(15, MinimumLength = 3, ErrorMessage = "State must be between 3 and 15 characters")]
    public required string State { get; set; }

    [Required(ErrorMessage = "Please enter city")]
    [StringLength(15, MinimumLength = 3, ErrorMessage = "City must be between 3 and 15 characters")]
    public required string City { get; set; }

    [Required(ErrorMessage = "Please enter address")]
    [StringLength(30, MinimumLength = 3, ErrorMessage = "Address must be between 3 and 30 characters")]
    public required string Address { get; set; }

    [Required] public DateTime DateOfBirth { get; set; } = DateTime.UtcNow;
}