
using System.ComponentModel.DataAnnotations;

namespace Presentation.Data.Models;

public class LogInViewModel
{
    [Required]
    [EmailAddress]
    public required string Email { get; set; } = default!;
    public string Password { get; set; } = default!;
    public bool RememberMe { get; set; }
}
