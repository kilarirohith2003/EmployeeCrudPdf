using Microsoft.AspNetCore.Mvc;
using Rotativa.AspNetCore;

public class EmployeesController : Controller
{
    private readonly IEmployeeRepository _repo;

    public EmployeesController(IEmployeeRepository repo)
    {
        _repo = repo;
    }

    // GET: /Employees
    public async Task<IActionResult> Index()
    {
        var list = await _repo.GetAllAsync();
        return View(list);
    }

    // GET: /Employees/Details/5
    public async Task<IActionResult> Details(int id)
    {
        var emp = await _repo.GetByIdAsync(id);
        if (emp == null) return NotFound();
        return View(emp);
    }

    // GET: /Employees/Create
    public IActionResult Create() => View();

    // POST: /Employees/Create
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Employee emp)
    {
        if (!ModelState.IsValid) return View(emp);
        var id = await _repo.CreateAsync(emp);
        return RedirectToAction(nameof(Details), new { id });
    }

    // GET: /Employees/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        var emp = await _repo.GetByIdAsync(id);
        if (emp == null) return NotFound();
        return View(emp);
    }

    // POST: /Employees/Edit/5
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Employee emp)
    {
        if (id != emp.Id) return BadRequest();
        if (!ModelState.IsValid) return View(emp);

        var ok = await _repo.UpdateAsync(emp);
        if (!ok) return NotFound();

        return RedirectToAction(nameof(Details), new { id = emp.Id });
    }

    // GET: /Employees/Delete/5
    public async Task<IActionResult> Delete(int id)
    {
        var emp = await _repo.GetByIdAsync(id);
        if (emp == null) return NotFound();
        return View(emp);
    }

    // POST: /Employees/Delete/5
    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var ok = await _repo.DeleteAsync(id);
        return RedirectToAction(nameof(Index));
    }

    // GET: /Employees/DownloadPdf/5
    public async Task<IActionResult> DownloadPdf(int id)
    {
        var emp = await _repo.GetByIdAsync(id);
        if (emp == null) return NotFound();

        // Use a dedicated, clean view for PDF (no buttons)
        return new ViewAsPdf("DetailsPdf", emp)
        {
            FileName = $"Employee_{emp.Id}.pdf",
            PageSize = Rotativa.AspNetCore.Options.Size.A4,
            PageOrientation = Rotativa.AspNetCore.Options.Orientation.Portrait,
            CustomSwitches = "--disable-smart-shrinking"
        };
    }
}
