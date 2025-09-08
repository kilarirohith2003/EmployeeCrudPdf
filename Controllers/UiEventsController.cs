using EmployeeCrudPdf.Services.Logging;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeCrudPdf.Controllers
{
    [Route("ui-events")]
    public class UiEventsController : Controller
    {
        private readonly IAppLogger _log;
        public UiEventsController(IAppLogger log) => _log = log;

        public class NavFailDto
        {
            public string Component { get; set; } = "";  // e.g. "Employees/ViewButton"
            public string Href { get; set; } = "";       // link target
            public string? Location { get; set; }        // page where click occurred
            public string? Extra { get; set; }           // id/classes/elapsed JSON
        }

        [HttpPost("navigation-failed")]
        [IgnoreAntiforgeryToken]
        public IActionResult NavigationFailed([FromBody] NavFailDto dto)
        {
            _log.Error("UI.Navigation",
                $"Navigation did NOT occur for {dto.Component} -> {dto.Href} (from {dto.Location}) extra={dto.Extra}");
            return NoContent();
        }
    }
}
