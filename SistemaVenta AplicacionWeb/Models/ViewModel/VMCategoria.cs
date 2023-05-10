using SistemaVenta.Entity;

namespace SistemaVenta_AplicacionWeb.Models.ViewModel
{
    public class VMCategoria
    {
        public int IdCategoria { get; set; }

        public string? Descripcion { get; set; }

        public int EsActivo { get; set; }
    }
}
