using Presentation.Models;
using Application.Dtos; 

namespace Presentation.Extensions; 

public static class EmployeeExtensions
{
    public static EmployeeViewModel ToViewModel(this EmployeeDto dto)
    {
        if (dto == null) return null!;

        return new EmployeeViewModel
        { 
            Id = dto.Id,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            PhoneNumber = dto.PhoneNumber,
            Position = dto.Position!,
            HireDate = dto.HireDate ?? DateTime.MinValue,        
            Salary = dto.Salary ?? 0m,                          
            DepartmentId = dto.DepartmentId ?? Guid.Empty,      
            Department = dto.Department,
            IsActive = dto.IsActive
        };
    }

    public static EmployeeDto ToDto(this EmployeeViewModel vm)
    {
        if (vm == null) return null!;

        return new EmployeeDto
        {
            Id = vm.Id,
            FirstName = vm.FirstName,
            LastName = vm.LastName,
            Email = vm.Email,
            PhoneNumber = vm.PhoneNumber,
            Position = vm.Position,
            HireDate = vm.HireDate,
            Salary = vm.Salary,
            DepartmentId = vm.DepartmentId,
            Department = vm.Department,
            IsActive = vm.IsActive
        };
    }

    public static EmployeesViewModel ToViewModel(this EmployeesDto dto)
    {
        if (dto == null) return null!;

        return new EmployeesViewModel
        {
            Employees = dto.Employees?
                .Where(e => e != null)
                .Select(e => e.ToViewModel())
                .ToList() ?? new List<EmployeeViewModel>()
        };
    }

    public static EmployeesDto ToDto(this EmployeesViewModel vm)
    {
        if (vm == null) return null!;

        return new EmployeesDto
        {
            Employees = vm.Employees?
                .Where(e => e != null)
                .Select(e => e.ToDto())
                .ToList() ?? new List<EmployeeDto>()
        };
    }
}