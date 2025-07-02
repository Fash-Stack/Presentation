namespace Presentation.Models;

public class CreateAddressViewModel
{
    public Guid EmployeeId { get; set; }
    public string? Street { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? Country { get; set; }
}

public class UpdateAddressViewModel
{
    public Guid Id { get; set; }
    public Guid EmployeeId { get; set; }
    public string Street { get; set; } = default!;
    public string City { get; set; } = default!;
    public string State { get; set; } = default!;
    public string Country { get; set; } = default!;
}

public class AddressViewModel
{
    public Guid Id { get; set; }
    public Guid EmployeeId { get; set; }
    public string? Street { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? Country { get; set; }
}
