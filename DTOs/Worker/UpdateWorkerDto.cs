using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace Ergasia_WebApp.DTOs.Worker;

public class UpdateWorkerDto
{
    [HiddenInput]
    public required string Id { get; set; }
    
    [Display(Name = "Minimal monthly salary")]
    public int? MinimalSalary { get; set; }
}