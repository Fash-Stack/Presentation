using System;
using System.ComponentModel.DataAnnotations;

namespace Presentation.Models;

public class ResetPasswordViewModel
{
    [Required]
    public string Email { get; set; } = default!;

    [Required]
    public string Password { get; set; } = default!;

    [Required]
    [Compare("Password", ErrorMessage = "Passwords do not match")]
    public string ConfirmPassword { get; set; } = default!;

    [Required]
    public string Token { get; set; } = default!;
}
