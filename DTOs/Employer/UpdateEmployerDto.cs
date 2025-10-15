using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace Ergasia_WebApp.DTOs.Employer;

public class UpdateEmployerDto
{
    [HiddenInput]
    public required string Id { get; set; }
    
    [Required(ErrorMessage = "Company name is required")]
    [Display(Name = "Company name")]
    public required string CompanyName { get; set; }
}