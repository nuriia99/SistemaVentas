using Microsoft.AspNetCore.Mvc;


using AutoMapper;
using Newtonsoft;
using SistemaVenta_AplicacionWeb.Models.ViewModel;
using SistemaVenta_AplicacionWeb.Utilidades.Response;
using SistemaVenta.BLL.Interfaces;
using SistemaVenta.Entity;
using SistemaVenta.DAL.Interfaces;
using Newtonsoft.Json;

namespace SistemaVenta_AplicacionWeb.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IUsuarioService _usuarioServicio;
        private readonly IRolService _rolServicio;

        public UsuarioController(IUsuarioService usuarioServicio, IRolService rolServicio, IMapper mapper)
        {
            _mapper = mapper;
            _usuarioServicio = usuarioServicio;
            _rolServicio = rolServicio;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ListaRoles()
        {
            List<VMRol> vmListaRoles = _mapper.Map<List<VMRol>>(await _rolServicio.Lista());
            return StatusCode(StatusCodes.Status200OK, vmListaRoles);

        }
        [HttpGet]
        public async Task<IActionResult> ListaUsuarios()
        {
            List<VMUsuario> vmUsuarioLista = _mapper.Map<List<VMUsuario>>(await _usuarioServicio.Lista());
            return StatusCode(StatusCodes.Status200OK, new {dara = vmUsuarioLista});
        }
        [HttpPost]
        public async Task<IActionResult> CrearUsuario([FromForm] IFormFile foto, [FromForm] string modelo)
        {
            GenericResponse<VMUsuario> genericResponse = new GenericResponse<VMUsuario>();
            try
            {
                VMUsuario vmUsuario = JsonConvert.DeserializeObject<VMUsuario>(modelo);

                string nombreFoto = "";
                Stream fotoStream = null;

                if(foto!=null)
                {
                    string nombreEnCodigo = Guid.NewGuid().ToString("N"); // La n indica que se cree un guid con numeros y letras.
                    string extension = Path.GetExtension(foto.FileName);
                    nombreFoto = string.Concat(nombreEnCodigo, extension);
                    fotoStream = foto.OpenReadStream();
                }

                string urlPlantillaCorreo = $"{this.Request.Scheme}://{this.Request.Host}/Plantilla/EnviarClave?correo=[correo]&clave=[clave]";

                Usuario usuarioCreado = await _usuarioServicio.CrearUsuario(_mapper.Map<Usuario>(vmUsuario), fotoStream, nombreFoto, urlPlantillaCorreo);
                vmUsuario = _mapper.Map<VMUsuario>(usuarioCreado);


                genericResponse.Estado = true;
                genericResponse.Objeto = vmUsuario;

            } catch (Exception ex) 
            {
                genericResponse.Estado = false;
                genericResponse.Mensaje = ex.Message;
                
            }
            return StatusCode(StatusCodes.Status200OK, genericResponse);
        }
        [HttpPut]
        public async Task<IActionResult> EditarUsuario([FromForm] IFormFile foto, [FromForm] string modelo)
        {
            GenericResponse<VMUsuario> genericResponse = new GenericResponse<VMUsuario>();
            try
            {
                VMUsuario vmUsuario = JsonConvert.DeserializeObject<VMUsuario>(modelo);

                string nombreFoto = "";
                Stream fotoStream = null;

                if (foto != null)
                {
                    string nombreEnCodigo = Guid.NewGuid().ToString("N"); // La n indica que se cree un guid con numeros y letras.
                    string extension = Path.GetExtension(foto.FileName);
                    nombreFoto = string.Concat(nombreEnCodigo, extension);
                    fotoStream = foto.OpenReadStream();
                }

                Usuario usuarioEditado = await _usuarioServicio.EditarUsuario(_mapper.Map<Usuario>(vmUsuario), fotoStream, nombreFoto);
                vmUsuario = _mapper.Map<VMUsuario>(usuarioEditado);

                genericResponse.Estado = true;
                genericResponse.Objeto = vmUsuario;

            }
            catch (Exception ex)
            {
                genericResponse.Estado = false;
                genericResponse.Mensaje = ex.Message;
            }
            return StatusCode(StatusCodes.Status200OK, genericResponse);
        }
        [HttpDelete]
        public async Task<IActionResult> EliminarUsuario(int idUsuario)
        {
            GenericResponse<string> genericResponse = new GenericResponse<string>();
            try
            {
                genericResponse.Estado = await _usuarioServicio.EliminarUsuario(idUsuario);
            }
            catch (Exception ex)
            {
                genericResponse.Estado = false;
                genericResponse.Mensaje = ex.Message;
            }
            return StatusCode(StatusCodes.Status200OK, genericResponse);
        }

    }
}
