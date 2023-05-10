using SistemaVenta.Entity;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVenta.BLL.Interfaces
{
    public interface IUsuarioService
    {
        Task<List<Usuario>> Lista();
        Task <Usuario> CrearUsuario(Usuario entidad, Stream foto = null, string nombreFoto = "", string urlPlantillaCorreo = "");
        Task<Usuario> EditarUsuario(Usuario entidad, Stream foto = null, string nombreFoto = "");
        Task<bool> EliminarUsuario(int idUsuario);
        Task<Usuario> ObtenerPorCredenciales(string correo, string clave);
        Task<Usuario> ObtenerPorId(int idUsuario);
        Task<bool> GuardarPerfil(Usuario entidad);
        Task<bool> CambiarClave(int idUsuario, string claveActual, string claveNueva);
        Task<bool> RestablecerClave(string correo, string urlPlantillaCorreo);
    }
}
