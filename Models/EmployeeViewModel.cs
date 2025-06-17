using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Presentation.Models;

public class EmployeeViewModel
{
    public Guid Id { get; set; } = default!;

    [Required(ErrorMessage = "First name is required")]
    [Display(Name = "First Name")]
    [StringLength(50, ErrorMessage = "First name cannot exceed 50 characters")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Last name is required")]
    [Display(Name = "Last Name")]
    [StringLength(50, ErrorMessage = "Last name cannot exceed 50 characters")]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    [Display(Name = "Email Address")]
    public string Email { get; set; } = string.Empty;

    [Display(Name = "PhoneNumber")]
    [Phone(ErrorMessage = "Invalid PhoneNumber number format")]
    public string? PhoneNumber { get; set; }

    [Required(ErrorMessage = "Position is required")]
    [Display(Name = "Position")]
    [StringLength(100, ErrorMessage = "Position cannot exceed 100 characters")]
    public string Position { get; set; } = string.Empty;

    [Required(ErrorMessage = "Hire date is required")]
    [Display(Name = "Hire Date")]
    [DataType(DataType.Date)]
    public DateTime HireDate { get; set; }

    [Required(ErrorMessage = "Salary is required")]
    [Display(Name = "Salary")]
    [Range(0, double.MaxValue, ErrorMessage = "Salary must be a positive number")]
    [DataType(DataType.Currency)]
    public decimal Salary { get; set; }

    [Display(Name = "Department Name")]
    [Required(ErrorMessage = "Department is required")]
    public Guid? DepartmentId { get; set; }

    public string Department { get; set; } = string.Empty;

    [Display(Name = "Active")]
    public bool IsActive { get; set; } = true;

    public List<SelectListItem> DepartmentSelectList { get; set; } = new List<SelectListItem>();
}

public class EmployeesViewModel
{
    public List<EmployeeViewModel> Employees { get; set; } = new List<EmployeeViewModel>();
}