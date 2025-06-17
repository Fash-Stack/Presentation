using System.ComponentModel.DataAnnotations;

namespace Presentation.Data.Models;

public class UpdateDepartmentsViewModel
{
    public Guid Id { get; set; }
    
    [Required(ErrorMessage = "Name is required.")]
    public string Name { get; set; } = default!;

    [MaxLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
    public string? Description { get; set; }
}
