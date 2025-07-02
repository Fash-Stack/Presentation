using Data.Context;
using Data.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Presentation.Extensions;
using Presentation.Models;
using Application.Dtos;
using Microsoft.AspNetCore.Authorization;
using Application.Services.Department;
using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
using Application.Services.Address;

namespace Presentation.Controllers;

[Authorize]
public class EmployeeController : BaseController
{
    public readonly IDepartmentService _departmentService;
    private readonly IEmployeeService _employeeService;
    private readonly EmployeeAppDbContext _context;
    private readonly ILogger<EmployeeController> _logger;
    private readonly Cloudinary _cloudinary;
    private readonly IAddressService _addressService;

    public EmployeeController(IEmployeeService employeeService,
     IDepartmentService departmentService, EmployeeAppDbContext employeeAppDbContext,
      ILogger<EmployeeController> logger, Cloudinary cloudinary, IAddressService addressService)
    {
        _employeeService = employeeService;
        _departmentService = departmentService;
        _context = employeeAppDbContext;
        _logger = logger;
        _cloudinary = cloudinary;
        _addressService = addressService;
    }

    public async Task<IActionResult> Index()
    {
        var employees = await _employeeService.GetAllEmployeesAsync();
        return View(employees.ToViewModel());
    }

    public async Task<IActionResult> Details(Guid id)
    {
        if (id == Guid.Empty)
        {
            return NotFound();
        }

        var employee = await _employeeService.GetEmployeeByIdAsync(id);
        if (employee == null)
        {
            return NotFound();
        }

        var address = await _addressService.GetAddressByEmployeeIdAsync(id);

        var viewModel = new EmployeeViewModel
        {
            Id = employee.Id,
            ImageUrl = employee.ImageUrl,
            FirstName = employee.FirstName,
            LastName = employee.LastName,
            Email = employee.Email,
            PhoneNumber = employee.PhoneNumber,
            Department = employee.Department!,
            Position = employee.Position!,
            Salary = (decimal)employee.Salary!,
            IsActive = employee.IsActive,
            Address = address != null ? new AddressViewModel
            {
                Id = address.Id,
                EmployeeId = address.EmployeeId,
                Street = address.Street,
                City = address.City,
                State = address.State,
                Country = address.Country
            } : new AddressViewModel()
        };

        return View(viewModel);
    }

    public async Task<IActionResult> Create()
    {
        var viewModel = new EmployeeViewModel
        {
            HireDate = DateTime.Today
        };

        await PopulateDepartmentDropdown(viewModel);
        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(EmployeeViewModel viewModel)
    {
        if (!ModelState.IsValid)
        {
            foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
            {
                Console.WriteLine($"Model error: {error.ErrorMessage}");
            }

            await PopulateDepartmentDropdown(viewModel);
            return View(viewModel);
        }

        if (!await _context.Departments.AnyAsync(d => d.Id == viewModel.DepartmentId))
        {
            ModelState.AddModelError("DepartmentId", "Please select a valid department.");
            await PopulateDepartmentDropdown(viewModel);
            return View(viewModel);
        }

        var employee = new Employee
        {
            Id = Guid.NewGuid(),
            FirstName = viewModel.FirstName,
            LastName = viewModel.LastName,
            Email = viewModel.Email,
            PhoneNumber = viewModel.PhoneNumber,
            Position = viewModel.Position,
            HireDate = viewModel.HireDate,
            Salary = viewModel.Salary,
            DepartmentId = viewModel.DepartmentId!.Value,
            IsActive = viewModel.IsActive,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Employees.Add(employee);
        await _context.SaveChangesAsync();

        var employeeCount = await _context.Employees
            .CountAsync(e => e.DepartmentId == employee.DepartmentId);

        var activeEmployeeCount = await _context.Employees
            .CountAsync(e => e.DepartmentId == employee.DepartmentId && e.IsActive);

        Console.WriteLine($"Total employees in employee: {employeeCount}");

        TempData["SuccessMessage"] = "Employee created successfully!";
        return RedirectToAction("Index");
    }

    private async Task PopulateDepartmentDropdown(EmployeeViewModel viewModel)
    {
        var departments = await _context.Departments.ToListAsync();

        viewModel.DepartmentSelectList = departments.Select(d => new SelectListItem
        {
            Value = d.Id.ToString(),
            Text = d.Name
        }).ToList();
    }

    public async Task<IActionResult> Edit(Guid id)
    {
        try
        {
            var employee = await _employeeService.GetEmployeeByIdAsync(id);
            if (employee == null)
            {
                return NotFound();
            }

            var departments = await _departmentService.GetAllDepartmentsAsync();

            var viewModel = new EmployeeViewModel
            {
                Id = employee.Id,
                Email = employee.Email,
                PhoneNumber = employee.PhoneNumber,
                Position = employee.Position ?? "",
                Salary = employee.Salary ?? 0,
                Department = departments.Departments.FirstOrDefault(d => d.Id == employee.DepartmentId)?.Name ?? "",
                IsActive = employee.IsActive,
                ImageUrl = employee.ImageUrl,
            };

            ViewBag.Department = departments.Departments.Select(d => d.Name).ToList();

            return View(viewModel);
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Error loading employee: {ex.Message}";
            return RedirectToAction("Index");
        }
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, EmployeeViewModel model)
    {
        if (id != model.Id)
        {
            return BadRequest();
        }

        try
        {
            var departments = await _departmentService.GetDepartmentsForDropdownAsync();

            var selectedDepartment = departments
                .FirstOrDefault(d => string.Equals(d.Name, model.Department, StringComparison.OrdinalIgnoreCase));

            if (selectedDepartment == null)
            {
                TempData["Error"] = "Selected department not found.";
                ViewBag.Department = departments.Select(d => d.Name).ToList();
                return View(model);
            }

            string imageUrl = model.ImagePath ?? "";

            if (model.Image != null && model.Image.Length > 0)
            {
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(model.Image.FileName, model.Image.OpenReadStream())
                };

                var uploadResult = await _cloudinary.UploadAsync(uploadParams);

                if (uploadResult.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    imageUrl = uploadResult.SecureUrl.AbsoluteUri;
                }
                else
                {
                    TempData["Error"] = "Image upload failed.";
                    ViewBag.Department = departments.Select(d => d.Name).ToList();
                    return View(model);
                }
            }

            var employeeDto = new EmployeeDto
            {
                Id = model.Id,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                Position = model.Position,
                Salary = model.Salary,
                DepartmentId = selectedDepartment.Id,
                IsActive = model.IsActive,
                ImageUrl = imageUrl,
            };

            var updatedEmployee = await _employeeService.UpdateEmployeeAsync(employeeDto);

            if (updatedEmployee == null)
            {
                return NotFound();
            }

            TempData["Success"] = "Employee updated successfully!";
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Error updating employee: {ex.Message}";
            var departments = await _departmentService.GetDepartmentsForDropdownAsync();
            ViewBag.Department = departments.Select(d => d.Name).ToList();
            return View(model);
        }
    }

    public async Task<IActionResult> Delete(Guid id)
    {
        var employee = await _context.Employees
            .FirstOrDefaultAsync(e => e.Id == id);

        if (employee == null)
        {
            return NotFound();
        }

        var viewModel = new EmployeeViewModel
        {
            Id = employee.Id,
            FirstName = employee.FirstName,
            LastName = employee.LastName,
            Email = employee.Email,
            Position = employee.Position!,
            IsActive = employee.IsActive,
            DepartmentId = employee.DepartmentId,
            Department = (await _context.Departments
                            .Where(d => d.Id == employee.DepartmentId)
                            .Select(d => d.Name)
                            .FirstOrDefaultAsync()) ?? "Unknown",
        };

        return View(viewModel);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(Guid id)
    {
        var employee = await _context.Employees.FindAsync(id);

        if (employee == null)
        {
            return NotFound();
        }

        _context.Employees.Remove(employee);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Employee deleted successfully!";
        return RedirectToAction("Index");
    }

    private async Task<List<SelectListItem>> GetDepartmentSelectListAsync()
    {
        var employees = await _context.Departments
            .Where(d => d.IsActive)
            .OrderBy(d => d.Name)
            .ToListAsync();

        return employees.Select(d => new SelectListItem
        {
            Value = d.Name,
            Text = d.Name
        }).ToList();
    }

    public async Task<IActionResult> EditImage(Guid id)
    {
        var employee = await _employeeService.GetEmployeeByIdAsync(id);
        if (employee == null)
            return NotFound();

        var viewModel = new EmployeeViewModel
        {
            Id = employee.Id,
            ImageUrl = employee.ImageUrl
        };

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditImage(Guid id, EmployeeViewModel model)
    {
        if (model.Image == null || model.Image.Length == 0)
        {
            TempData["Error"] = "Please select an image.";
            return View(model);
        }

        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(model.Image.FileName, model.Image.OpenReadStream())
        };

        var uploadResult = await _cloudinary.UploadAsync(uploadParams);

        if (uploadResult.StatusCode == System.Net.HttpStatusCode.OK)
        {
            await _employeeService.UpdateEmployeeImageAsync(id, uploadResult.SecureUrl.AbsoluteUri);
            TempData["Success"] = "Image updated successfully.";
            return RedirectToAction("Details", new { id });
        }

        TempData["Error"] = "Image upload failed.";
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteImage(Guid id)
    {
        var employee = await _employeeService.GetEmployeeByIdAsync(id);
        if (employee == null)
            return NotFound();

        await _employeeService.UpdateEmployeeImageAsync(id, null);  
        TempData["Success"] = "Image deleted successfully.";
        return RedirectToAction("Details", new { id });
    }

}