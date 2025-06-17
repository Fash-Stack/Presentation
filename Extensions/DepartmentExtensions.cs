using Presentation.Models;
using Application.Dtos;

namespace Presentation.Extensions;

public static class DepartmentExtensions
{
    public static DepartmentViewModel ToViewModel(this DepartmentDto dto)
    {
        if (dto == null) return null!;

        return new DepartmentViewModel
        {
            Id = dto.Id,
            Name = dto.Name,
            Description = dto.Description,
            IsActive = dto.IsActive,
            CreatedAt = dto.CreatedAt,
            UpdatedAt = dto.UpdatedAt,
            EmployeeCount = dto.EmployeeCount
        };
    }

    public static DepartmentDto ToDto(this DepartmentViewModel vm)
    {
        if (vm == null) return null!;

        return new DepartmentDto
        {
            Id = vm.Id,
            Name = vm.Name,
            Description = vm.Description,
            IsActive = vm.IsActive,
            CreatedAt = vm.CreatedAt,
            UpdatedAt = vm.UpdatedAt,
            EmployeeCount = vm.EmployeeCount
        };
    }

    public static List<DepartmentViewModel> ToViewModel(this List<DepartmentDto> dtos)
    {
        if (dtos == null) 
            return new List<DepartmentViewModel>();
            
        return dtos
            .Where(d => d != null)
            .Select(d => d.ToViewModel())
            .ToList();
    }

    public static DepartmentsViewModel ToViewModel(this DepartmentsDto dto)
    {
        if (dto == null) return null!;

        return new DepartmentsViewModel
        {
            Departments = dto.Departments?
                .Where(d => d != null)
                .Select(d => d.ToViewModel())
                .ToList() ?? new List<DepartmentViewModel>()
        };
    }

    public static DepartmentsDto ToDto(this DepartmentsViewModel vm)
    {
        if (vm == null) return null!;
        
        return new DepartmentsDto
        {
            Departments = vm.Departments?
                .Where(d => d != null)
                .Select(d => d.ToDto())
                .ToList() ?? new List<DepartmentDto>()
        };
    }
}