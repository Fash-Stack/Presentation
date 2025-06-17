using System.ComponentModel.DataAnnotations;
namespace Presentation.Models;

public class DepartmentViewModel
{
    public Guid Id { get; set; } = default!;
    
    [Required(ErrorMessage = "Department name is required")]
    [Display(Name = "Department Name")]
    [StringLength(100, ErrorMessage = "Department name cannot exceed 100 characters")]
    public string Name { get; set; } = string.Empty;

    [Display(Name = "Description")]
    [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
    public string? Description { get; set; }

    public bool IsActive { get; set; } = true;

    [Display(Name = "Created Date")]
    public DateTime CreatedAt { get; set; }

    [Display(Name = "Last Updated")]
    public DateTime UpdatedAt { get; set; }

    [Display(Name = "Number of Employees")]
    public int EmployeeCount { get; set; } = 0;
}

public class DepartmentsViewModel
{
    public List<DepartmentViewModel> Departments { get; set; } = default!;
}
