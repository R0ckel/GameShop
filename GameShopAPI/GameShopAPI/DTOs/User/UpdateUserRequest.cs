using System.ComponentModel.DataAnnotations;

namespace GameShopAPI.DTOs.User;

public class UpdateUserRequest
{
    public Guid Id { get; set; }

    [MaxLength(255)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(255)]
    public string Surname { get; set; } = string.Empty;

    [CustomEmail]
    public string Email { get; set; } = string.Empty;

    public DateOnly? BirthDate { get; set; }

    [Required]
    public string CurrentPassword { get; set; } = string.Empty;

    [CustomValidation(typeof(UpdateUserRequest), nameof(ValidatePassword))]
    public string NewPassword { get; set; } = string.Empty;

    public static ValidationResult? ValidatePassword(string password, ValidationContext context)
    {
        if (string.IsNullOrEmpty(password))
            return ValidationResult.Success;

        if (password.Length < 8)
            return new ValidationResult("Password must be at least 8 characters");

        if (password.Length > 64)
            return new ValidationResult("Password must be at most 64 characters");

        if (!password.Any(char.IsDigit))
            return new ValidationResult("Password must contain at least one digit.");

        if (!password.Any(char.IsUpper))
            return new ValidationResult("Password must contain at least one uppercase letter.");

        if (!password.Any(ch => !char.IsLetterOrDigit(ch)))
            return new ValidationResult("Password must contain at least one non-letter and non-digit character.");

        return ValidationResult.Success;
    }
}

public class CustomEmailAttribute : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        var email = value as string;
        if (string.IsNullOrEmpty(email))
            return true;

        return new EmailAddressAttribute().IsValid(email);
    }
}