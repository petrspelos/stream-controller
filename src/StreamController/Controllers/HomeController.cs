using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SharpHook;
using SharpHook.Data;
using StreamController.Core;
using StreamOverlay.Models;

namespace StreamOverlay.Controllers;

public class HomeController(
    IObsClient obsService
    ) : Controller
{
    public IActionResult Index()
    {
        return View(new IndexViewModel
        {
            IsObsConnected = obsService.IsConnected
        });
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
