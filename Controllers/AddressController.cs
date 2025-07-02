using Application.Services.Address;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Presentation.DtoMapping;
using Presentation.Models;

namespace Presentation.Controllers
{
    [Route("Address")]
    public class AddressController : Controller
    {
        private readonly IAddressService _addressService;

        public AddressController(IAddressService addressService)
        {
            _addressService = addressService;
        }

        [HttpGet("Create")]
        public IActionResult Create(Guid employeeId)
        {
            var model = new CreateAddressViewModel
            {
                EmployeeId = employeeId
            };

            PopulateStatesDropdown();

            return View(model);
        }

        [HttpPost("Create")]
        public async Task<IActionResult> Create(CreateAddressViewModel model)
        {
            if (!ModelState.IsValid)
            {
                PopulateStatesDropdown();
                return View(model);
            }

            var dto = model.ToDto();
            await _addressService.AddAddressAsync(dto);

            return RedirectToAction("Details", "Employee", new { id = model.EmployeeId });
        }

        [HttpGet("Update")]
        public async Task<IActionResult> Update(Guid employeeId)
        {
            var dto = await _addressService.GetAddressByEmployeeIdAsync(employeeId);
            if (dto == null)
            {
                return RedirectToAction("Details", "Employee", new { id = employeeId });
            }

            var model = dto.ToUpdateAddressViewModel();
            PopulateStatesDropdown();
            return View(model);
        }

        [HttpPost("Update")]
        public async Task<IActionResult> Update(UpdateAddressViewModel model)
        {
            if (!ModelState.IsValid)
            {
                PopulateStatesDropdown();
                return View(model);
            }

            var dto = model.ToUpdateDto();
            await _addressService.UpdateAddressAsync(dto);
            return RedirectToAction("Details", "Employee", new { id = model.EmployeeId });
        }

        [HttpPost("Delete")]
        public async Task<IActionResult> Delete(Guid employeeId)
        {
            await _addressService.DeleteAddressAsync(employeeId);
            return RedirectToAction("Details", "Employee", new { id = employeeId });
        }

        private void PopulateStatesDropdown()
        {
            ViewBag.States = _addressService.GetAllStates()
                .Select(state => new SelectListItem
                {
                    Text = state,
                    Value = state
                })
                .ToList();
        }
    }
}