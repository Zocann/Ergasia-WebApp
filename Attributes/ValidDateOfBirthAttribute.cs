using System.ComponentModel.DataAnnotations;

namespace Ergasia_WebApp.Attributes;

//Attribute for date of birth validation
public class ValidDateOfBirthAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        var dateOfBirth = Convert.ToDateTime(value);
        if (dateOfBirth >= DateTime.UtcNow) return new ValidationResult("Invalid Date Of Birth.");
        return dateOfBirth <= DateTime.UtcNow.AddYears(-18) 
            ? ValidationResult.Success 
            : new ValidationResult("Worker needs to be at least 18 years old");
    }
}