using System.ComponentModel.DataAnnotations;

namespace Ergasia_WebApp.DTOs.User;

public class LoginDto
{
    [Required]
    [Display(Name = "Email address")]
    [EmailAddress]
    public required string Email { get; set; }
    
    [Required]
    [DataType(DataType.Password)]
    public required string Password { get; set; }
}