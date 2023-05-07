using Microsoft.AspNetCore.Mvc;

namespace SistemaVenta_AplicacionWeb.Controllers
{
    public class UsuarioController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
