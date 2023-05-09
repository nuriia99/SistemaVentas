using Microsoft.AspNetCore.Mvc;

namespace SistemaVenta_AplicacionWeb.Controllers
{
    public class PlantillaController : Controller
    {
        public IActionResult EnviarClave(string correo,string clave)
        {
            ViewData["Correo"] = correo;
            ViewData["Clave"] = clave;
            ViewData["Url"] = $"{Request.Scheme}://{this.Request.Host}";//el schema es el http 
            return View();
        }
        public IActionResult RestablecerClave(string clave)
        {
            ViewData["Clave"] = clave;
            return View();
        }
    }
}
