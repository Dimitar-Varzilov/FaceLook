using FaceLook.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace FaceLook.Web.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            var model = new SendMessageRequest()
            {
                ChatId = Guid.Empty,
                Content = string.Empty,
                SenderId = string.Empty
            };
            return View(model);
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
}
