using Microsoft.AspNetCore.Mvc;

namespace SistemaVenta_AplicacionWeb.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
