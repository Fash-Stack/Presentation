using Data.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Presentation.Controllers
{
    public class DashboardController : Controller
    {
        private readonly EmployeeAppDbContext _context;

        public DashboardController(EmployeeAppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<JsonResult> GetDashboardData()
        {
            try
            {
                // Get departments with employee counts
                var departments = await _context.Departments
                    .Select(d => new {
                        name = d.Name,
                        employeeCount = d.Employees.Count()
                    })
                    .OrderBy(d => d.name)
                    .ToListAsync();

                // Get total counts
                var totalEmployees = await _context.Employees.CountAsync();
                var totalDepartments = await _context.Departments.CountAsync();

                // Get monthly employee trend (last 6 months)
                var monthlyData = await GetMonthlyEmployeeData();

                return Json(new {
                    success = true,
                    departments,
                    totalEmployees,
                    totalDepartments,
                    monthlyTrend = monthlyData
                });
            }
            catch (Exception ex)
            {
                return Json(new { 
                    success = false, 
                    message = "Error loading dashboard data: " + ex.Message 
                });
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetEmployeesByDepartment()
        {
            try
            {
                var result = await _context.Departments
                    .Include(d => d.Employees)
                    .Select(d => new {
                        departmentName = d.Name,
                        employeeCount = d.Employees.Count(),
                        employees = d.Employees.Select(e => new {
                            id = e.Id,
                            name = e.FirstName + " " + e.LastName,
                            email = e.Email
                        }).ToList()
                    })
                    .ToListAsync();

                return Json(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return Json(new { 
                    success = false, 
                    message = "Error loading employee data: " + ex.Message 
                });
            }
        }

        private async Task<List<object>> GetMonthlyEmployeeData()
        {
            var monthlyData = new List<object>();
            var currentDate = DateTime.Now;

            for (int i = 5; i >= 0; i--)
            {
                var targetDate = currentDate.AddMonths(-i);
                var endOfMonth = new DateTime(targetDate.Year, targetDate.Month, 
                    DateTime.DaysInMonth(targetDate.Year, targetDate.Month));

                var employeeCount = await _context.Employees
                    .Where(e => e.CreatedAt <= endOfMonth)
                    .CountAsync();

                monthlyData.Add(new {
                    month = targetDate.ToString("MMM"),
                    count = employeeCount
                });
            }

            return monthlyData;
        }

        [HttpGet]
        public async Task<JsonResult> GetDepartmentStatistics()
        {
            try
            {
                var stats = await _context.Departments
                    .Select(d => new {
                        departmentName = d.Name,
                        employeeCount = d.Employees.Count(),
                        avgSalary = d.Employees.Any() ? d.Employees.Average(e => e.Salary) : 0,
                        maxSalary = d.Employees.Any() ? d.Employees.Max(e => e.Salary) : 0,
                        minSalary = d.Employees.Any() ? d.Employees.Min(e => e.Salary) : 0
                    })
                    .ToListAsync();

                return Json(new { success = true, statistics = stats });
            }
            catch (Exception ex)
            {
                return Json(new { 
                    success = false, 
                    message = "Error loading statistics: " + ex.Message 
                });
            }
        }
    }
}
