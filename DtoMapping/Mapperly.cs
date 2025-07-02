using Application.Dtos;
using Presentation.Models;
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
    public static CreateAddressDto ToDto(this CreateAddressViewModel address)
    {
        return new CreateAddressDto
        {
            EmployeeId = address.EmployeeId,
            Street = address.Street,
            City = address.City,
            State = address.State,
            Country = address.Country
        };
    }
    public static UpdateAddressViewModel ToUpdateAddressViewModel(this AddressDto dto)
    {
        return new UpdateAddressViewModel
        {
            Id = dto.Id,
            EmployeeId = dto.EmployeeId,
            Street = dto.Street,
            City = dto.City,
            State = dto.State,
            Country = dto.Country
        };
    }
    public static AddressDto ToDto(this UpdateAddressViewModel model)
    {
        return new AddressDto
        {
            Id = model.Id,
            EmployeeId = model.EmployeeId,
            Street = model.Street,
            City = model.City,
            State = model.State,
            Country = model.Country
        };
    }
    public static UpdateAddressDto ToUpdateDto(this UpdateAddressViewModel model)
    {
        return new UpdateAddressDto
        {
            Id = model.Id,
            EmployeeId = model.EmployeeId,
            Street = model.Street,
            City = model.City,
            State = model.State,
            Country = model.Country
        };
    }
}
