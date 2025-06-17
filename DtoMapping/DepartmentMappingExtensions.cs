using Application.Dtos;
using Presentation.Models;

namespace Presentation.DtoMapping
{
    public static class DepartmentMappingExtensions
    {
        public static List<DepartmentViewModel> ToViewModel(this List<DepartmentDto> dtos)
        {
            return dtos.Select(dto => new DepartmentViewModel
            {
                Id = dto.Id,
                Name = dto.Name,
                Description = dto.Description,
                IsActive = dto.IsActive,
                CreatedAt = dto.CreatedAt,
                UpdatedAt = dto.UpdatedAt,
                EmployeeCount = dto.EmployeeCount
            }).ToList();
        }
    }
}
