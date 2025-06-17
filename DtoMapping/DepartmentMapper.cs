using Riok.Mapperly.Abstractions;
using Application.Dtos;
using Presentation.Models;

namespace Presentation.DtoMapping;

[Mapper]
public partial class DepartmentMapper
{
    public partial DepartmentViewModel DepartmentDtoToViewModel(DepartmentDto dto);
    public partial List<DepartmentViewModel> DepartmentDtosToViewModels(List<DepartmentDto> dtos);
}
