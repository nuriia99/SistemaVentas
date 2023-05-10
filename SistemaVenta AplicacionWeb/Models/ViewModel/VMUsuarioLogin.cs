using SistemaVenta.Entity;

namespace SistemaVenta_AplicacionWeb.Models.ViewModel
{
    public class VMUsuarioLogin
    {

        public string? Correo { get; set; }

        public string? Clave { get; set; }

        public bool MantenerSesion { get; set; }
    }
}
