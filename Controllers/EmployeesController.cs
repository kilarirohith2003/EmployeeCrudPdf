using Microsoft.AspNetCore.Mvc;
using Rotativa.AspNetCore;

public class EmployeesController : Controller
{
    private readonly IEmployeeRepository _repo;

    public EmployeesController(IEmployeeRepository repo)
    {
        _repo = repo;
    }

   
    public async Task<IActionResult> Index()
    {
        var list = await _repo.GetAllAsync();
        return View(list);
    }

    
    public async Task<IActionResult> Details(int id)
    {
        var emp = await _repo.GetByIdAsync(id);
        if (emp == null) return NotFound();
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
        if (emp == null) return NotFound();
        return View(emp);
    }

    
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Employee emp)
    {
        if (id != emp.Id) return BadRequest();
        if (!ModelState.IsValid) return View(emp);

        var ok = await _repo.UpdateAsync(emp);
        if (!ok) return NotFound();

        return RedirectToAction(nameof(Details), new { id = emp.Id });
    }

    
    public async Task<IActionResult> Delete(int id)
    {
        var emp = await _repo.GetByIdAsync(id);
        if (emp == null) return NotFound();
        return View(emp);
    }

    
    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var ok = await _repo.DeleteAsync(id);
        return RedirectToAction(nameof(Index));
    }

    
    public async Task<IActionResult> DownloadPdf(int id)
    {
        var emp = await _repo.GetByIdAsync(id);
        if (emp == null) return NotFound();

        
        return new ViewAsPdf("DetailsPdf", emp)
        {
            FileName = $"Employee_{emp.Id}.pdf",
            PageSize = Rotativa.AspNetCore.Options.Size.A4,
            PageOrientation = Rotativa.AspNetCore.Options.Orientation.Portrait,
            CustomSwitches = "--disable-smart-shrinking"
        };
    }
}
