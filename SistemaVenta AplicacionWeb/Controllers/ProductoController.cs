using Microsoft.AspNetCore.Mvc;

namespace SistemaVenta_AplicacionWeb.Controllers
{
    public class ProductoController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
