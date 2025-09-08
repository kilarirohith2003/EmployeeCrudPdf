using EmployeeCrudPdf.Data;
using EmployeeCrudPdf.Models;
using EmployeeCrudPdf.Services.Logging;
using Microsoft.AspNetCore.Mvc;
using Rotativa.AspNetCore;
using Rotativa.AspNetCore.Options;

namespace EmployeeCrudPdf.Controllers
{
    public class EmployeesController : Controller
    {
        private readonly IEmployeeRepository _repo;
        private readonly IAppLogger _log;

        public EmployeesController(IEmployeeRepository repo, IAppLogger log)
        {
            _repo = repo;
            _log = log;
        }

        public async Task<IActionResult> Index()
        {
            var list = await _repo.GetAllAsync();
            return View(list);
        }

        public async Task<IActionResult> Details(int id)
        {
            var emp = await _repo.GetByIdAsync(id);
            return View(emp);
        }

        public IActionResult Create() => View();

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Employee emp)
        {
            if (!ModelState.IsValid) return View(emp);
            var id = await _repo.CreateAsync(emp);
            return RedirectToAction(nameof(Details), new { id });
        }

        public async Task<IActionResult> Edit(int id)
        {
            var emp = await _repo.GetByIdAsync(id);
            return View(emp);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Employee emp)
        {
            if (id != emp.Id) return BadRequest();
            if (!ModelState.IsValid) return View(emp);

            await _repo.UpdateAsync(emp);
            return RedirectToAction(nameof(Details), new { id = emp.Id });
        }

        public async Task<IActionResult> Delete(int id)
        {
            var emp = await _repo.GetByIdAsync(id);
            return View(emp);
        }

        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _repo.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> DownloadPdf(int id)
        {
            var emp = await _repo.GetByIdAsync(id);

           
            return new ViewAsPdf("DetailsPdf", emp)
            {
                FileName = $"Employee_{emp.Id}.pdf",
                PageSize = Size.A4,
                PageOrientation = Orientation.Portrait,
                PageMargins = new Margins(18, 18, 18, 18),
                CustomSwitches = "--disable-smart-shrinking"
            };
        }
    }
}
