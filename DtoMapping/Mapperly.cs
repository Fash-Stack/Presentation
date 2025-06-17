using Application.Dtos;
using Presentation.Models;
using Presentation.Controllers;
namespace Presentation.DtoMapping;

public static class Mapperly
{
    public static DepartmentViewModel ToViewModel(this DepartmentDto dto)
    {
        return new DepartmentViewModel()
        {
            Id = dto.Id,
            Name = dto.Name,
            Description = dto.Description
        };
    }

    public static DepartmentDto ToDto(this DepartmentViewModel vm)
    {
        return new DepartmentDto()
        {
            Id = vm.Id,
            Name = vm.Name,
            Description = vm.Description,
            Employees = new List<EmployeeDto>()  // Proper initialization
        };
    }

    // DepartmentsViewModel <-> DepartmentsDto
    public static DepartmentsViewModel? ToViewModel(this DepartmentsDto? dto)
    {
        return dto == null ? null : new DepartmentsViewModel
        {
            Departments = dto.Departments?
                .Where(d => d != null)
                .Select(d => d.ToViewModel())
                .ToList() ?? []
        };
    }

    public static DepartmentsDto? ToDto(this DepartmentsViewModel? vm)
    {
        return vm == null ? null : new DepartmentsDto
        {
            Departments = vm.Departments?
                .Where(d => d != null)
                .Select(d => d.ToDto())
                .ToList() ?? []
        };
    }
}
