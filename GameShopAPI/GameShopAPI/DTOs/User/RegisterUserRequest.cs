using System.ComponentModel.DataAnnotations;

namespace GameShopAPI.DTOs.User;

public class RegisterUserRequest
{
    [Required, MaxLength(40)]
    public string Name { get; set; } = string.Empty;

    [Required, MaxLength(40)]
    public string Surname { get; set; } = string.Empty;

    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public DateOnly BirthDate { get; set; }

    [Required, MinLength(8), MaxLength(64)]
    [CustomValidation(typeof(RegisterUserRequest), nameof(ValidatePassword))]
    public string Password { get; set; } = string.Empty;

    public static ValidationResult? ValidatePassword(string password, ValidationContext context)
    {
        if (string.IsNullOrEmpty(password))
            return new ValidationResult("Password is required.");

        if (!password.Any(char.IsDigit))
            return new ValidationResult("Password must contain at least one digit.");

        if (!password.Any(char.IsUpper))
            return new ValidationResult("Password must contain at least one uppercase letter.");

        if (!password.Any(ch => !char.IsLetterOrDigit(ch)))
            return new ValidationResult("Password must contain at least one non-letter and non-digit character.");

        return ValidationResult.Success;
    }
}
