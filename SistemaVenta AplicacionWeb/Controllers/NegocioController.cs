using Microsoft.AspNetCore.Mvc;

namespace SistemaVenta_AplicacionWeb.Controllers
{
    public class NegocioController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
