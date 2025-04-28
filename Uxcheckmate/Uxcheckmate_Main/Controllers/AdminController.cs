using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Uxcheckmate_Main.Models;
using Uxcheckmate_Main.ViewModels;
using Uxcheckmate_Main.Areas.Identity.Data;

namespace Uxcheckmate_Main.Controllers;

public class AdminController : Controller
{
    private readonly AuthDbContext _authContext;
    private readonly UxCheckmateDbContext _appContext;

    public AdminController(AuthDbContext authContext, UxCheckmateDbContext appContext)
    {
        _authContext = authContext;
        _appContext = appContext;
    }

    public async Task<IActionResult> Index()
    {
        var users = await _authContext.Users.ToListAsync();
        var reports = await _appContext.Reports.ToListAsync();

        var model = users.Select(user => new AdminUserViewModel
        {
            User = user,
            Reports = reports.Where(r => r.UserID == user.Id).ToList()
        }).ToList();

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> DeleteReport(int id)
    {
        var report = await _appContext.Reports.FindAsync(id);
        if (report != null)
        {
            _appContext.Reports.Remove(report);
            await _appContext.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }
}
