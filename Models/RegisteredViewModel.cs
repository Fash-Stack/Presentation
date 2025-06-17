
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Presentation.Models;

public class RegisteredViewModel
{
    [Required]
    [EmailAddress]
    public required string Email { get; set; } = default!;
    [Required]
    [PasswordPropertyText]
    public required string Password { get; set; } = default!;
    public string ConfirmPassword { get; set; } = default!;
}
