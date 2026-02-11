using Microsoft.AspNetCore.Mvc;

namespace FaceLook.Controllers
{
    public class MessagesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> SendMessage()
        {

        }
    }
}
