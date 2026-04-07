using Microsoft.AspNetCore.Mvc;

namespace AUTOX.NETCORE.Controllers
{
    public class AutoxController : Controller
    {
        public async Task<IActionResult> Index()
        {
            return View();
        }
    }
}
