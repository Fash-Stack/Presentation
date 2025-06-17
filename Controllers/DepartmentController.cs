using Application.Dtos;
using Application.Services.Department;
using Microsoft.AspNetCore.Mvc;
using Presentation.Models;
using Presentation.Extensions;
using Presentation.Data.Models;
using Microsoft.AspNetCore.Authorization;


namespace Presentation.Controllers;

[Authorize]
public class DepartmentController : BaseController
{
    private readonly IDepartmentService _departmentService;
    private readonly ILogger<DepartmentController> _logger;

    public DepartmentController(IDepartmentService departmentService, ILogger<DepartmentController> logger)
    {
        _departmentService = departmentService;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        var departments = await _departmentService.GetAllDepartmentsAsync();

        var viewModel = departments.ToViewModel();

        return View(viewModel);
    }

    public async Task<IActionResult> Details(Guid id)
    {
        if (id == Guid.Empty)
        {
            return NotFound();
        }

        try
        {
            var department = await _departmentService.GetDepartmentByIdAsync(id);
            if (department == null)
            {
                return NotFound();
            }

            return View(department.ToViewModel());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while loading department details for ID: {DepartmentId}", id);
            TempData["ErrorMessage"] = "An error occurred while loading department details.";
            return RedirectToAction(nameof(Index));
        }
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateDepartmentViewModel model)
    {
        if (!ModelState.IsValid)
        {
            SetFlashMessage("Please fill in all required fields correctly.", "error");
            return View(model);
        }

        var viewModel = new CreateDepartmentDto
        {
            Id = Guid.NewGuid(),
            Name = model.Name,
            Description = model.Description
        };

        var result = await _departmentService.CreateDepartmentAsync(viewModel);

        if (result == null)
        {
            SetFlashMessage("An error occurred while creating the department. Please try again.", "error");
            return View(model);
        }

        SetFlashMessage("Department created successfully.", "success");

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(Guid id)
    {
        if (id == Guid.Empty)
        {
            return NotFound();
        }

        try
        {
            var department = await _departmentService.GetDepartmentByIdAsync(id);
            if (department == null)
            {
                return NotFound();
            }

            var updateModel = new UpdateDepartmentsViewModel
            {
                Id = department.Id,
                Name = department.Name,
                Description = department.Description,
            };

            return View(updateModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while loading department details for ID: {DepartmentId}", id);
            TempData["ErrorMessage"] = "An error occurred while loading department details.";
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, DepartmentDto departmentDto)
    {
        if (id != departmentDto.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                await _departmentService.UpdateDepartmentAsync(departmentDto);
                TempData["SuccessMessage"] = "Department updated successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError("", ex.Message);
            }
        }

        return View(departmentDto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var department = await _departmentService.GetDepartmentByIdAsync(id);
            if (department == null)
            {
                return NotFound();
            }

            if (department.EmployeeCount > 0)
            {
                TempData["Error"] = $"Cannot delete department '{department.Name}' because it has {department.EmployeeCount} employee(s) assigned to it.";
                return RedirectToAction(nameof(Details), new { id });
            }

            await _departmentService.DeleteDepartmentAsync(id);
            TempData["Success"] = $"Department '{department.Name}' was deleted successfully.";
        }
        catch (Exception ex)
        {
            TempData[$"Error {ex}"] = "An error occurred while deleting the department.";
        }

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> ViewEmployees(string departmentId)
    {
        if (!Guid.TryParse(departmentId, out Guid deptGuid))
        {
            return NotFound();
        }

        var department = await _departmentService.GetDepartmentByIdAsync(deptGuid);

        if (department == null)
        {
            return NotFound();
        }

        var employees = await _departmentService.GetEmployeesByDepartmentIdAsync(deptGuid);

        ViewBag.Department = department.Name;
        ViewBag.DepartmentId = departmentId;

        return View(employees);
    }
}
