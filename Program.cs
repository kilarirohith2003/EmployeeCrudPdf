using EmployeeCrudPdf.Data;
using EmployeeCrudPdf.Middleware;
using EmployeeCrudPdf.Services.Logging;
using Rotativa.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddControllersWithViews()
    .AddRazorRuntimeCompilation();

builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();

// File logger (single instance)
builder.Services.AddSingleton<IAppLogger, FileLogger>();

var app = builder.Build();

app.UseStatusCodePages(async context =>
{
    var http = context.HttpContext;
    var logger = http.RequestServices.GetRequiredService<EmployeeCrudPdf.Services.Logging.IAppLogger>();
    var code = http.Response.StatusCode;
    if (code >= 400)
    {
        var path = http.Request.Path.Value;
        var referer = http.Request.Headers["Referer"].ToString();
        var ua = http.Request.Headers["User-Agent"].ToString();
        logger.Warn("StatusCodePages", $"Status {code} for {path} | Referrer={referer} | UA={ua}");
    }
    await Task.CompletedTask;
});



// Global exception handler middleware (maps exceptions to HTTP codes + logs)
app.UseMiddleware<ExceptionHandlingMiddleware>();

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthorization();

// Rotativa wkhtmltopdf binaries live in wwwroot/Rotativa
RotativaConfiguration.Setup(app.Environment.WebRootPath, "Rotativa");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Employees}/{action=Index}/{id?}");

app.Run();
