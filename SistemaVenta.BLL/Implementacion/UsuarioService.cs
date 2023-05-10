using SistemaVenta.BLL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using System.Net;
using SistemaVenta.BLL.Interfaces;
using SistemaVenta.DAL.Interfaces;
using SistemaVenta.Entity;

namespace SistemaVenta.BLL.Implementacion
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IGenericRepository<Usuario> _repositorio;
        private readonly IFirebaseService _firebaseService;
        private readonly IUtilidadesService _utilidadesService;
        private readonly ICorreoService _correoService;
        public UsuarioService(IGenericRepository<Usuario> repository, IFirebaseService firebaseService, IUtilidadesService utilidadesService, ICorreoService correoService)
        {
            _repositorio = repository;
            _firebaseService = firebaseService;
            _utilidadesService = utilidadesService;
            _correoService = correoService;
        }
        public async Task<List<Usuario>> Lista()
        {
            IQueryable<Usuario> query = await _repositorio.Consultar();
            return query.Include(r => r.IdRolNavigation).ToList();
        }


        public async Task<Usuario> CrearUsuario(Usuario entidad, Stream foto = null, string nombreFoto = "", string urlPlantillaCorreo = "")
        {
            Usuario usuario_existe = await _repositorio.Obtener(u => u.Correo == entidad.Correo);
            if(usuario_existe != null)
            {
                throw new TaskCanceledException("El correo ya existe");
            }

            try
            {
                string clave_generada = _utilidadesService.GenerarClave();
                entidad.Clave = _utilidadesService.ConvertirSha256(clave_generada);
                entidad.NombreFoto = nombreFoto;
                
                if(foto != null)
                {
                    string urlFoto = await _firebaseService.SubirStorage(foto, "carpeta_usuario", nombreFoto);
                    entidad.UrlFoto = urlFoto;
                }

                Usuario usuarioCreado = await _repositorio.Crear(entidad);
                if(usuarioCreado.IdUsuario == 0)
                {
                    throw new TaskCanceledException("No se pudo crear el usuario");
                }

                if(urlPlantillaCorreo != "")
                {
                    urlPlantillaCorreo = urlPlantillaCorreo.Replace("[correo]", usuarioCreado.Correo).Replace("[clave]", clave_generada);
                    string htmlCorreo = "";

                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(urlPlantillaCorreo);
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                    if(response.StatusCode == HttpStatusCode.OK)
                    {
                        using (Stream dataStream = response.GetResponseStream())
                        {
                            StreamReader reader = null;

                            if(response.CharacterSet == null)
                            {
                                reader = new StreamReader(dataStream);
                            } else
                            {
                                reader = new StreamReader(dataStream, Encoding.GetEncoding(response.CharacterSet));
                            }

                            htmlCorreo = reader.ReadToEnd();
                            response.Close();
                            reader.Close();
                        }
                    }

                    if(htmlCorreo != "")
                    {
                        await _correoService.EnviarCorreo(usuarioCreado.Correo, "Cuenta creada", htmlCorreo);
                    }
                }

                IQueryable<Usuario> query = await _repositorio.Consultar(u => u.IdUsuario == usuarioCreado.IdUsuario);
                usuarioCreado = query.Include(r => r.IdRolNavigation).First();
                return usuarioCreado;

            } catch(Exception)
            {
                throw;
            }

        }

        public async Task<Usuario> EditarUsuario(Usuario entidad, Stream foto = null, string nombreFoto = "")
        {
            Usuario usuario_existe = await _repositorio.Obtener(u => u.Correo == entidad.Correo && u.IdUsuario != entidad.IdUsuario);
            if (usuario_existe != null)
            {
                throw new TaskCanceledException("El correo ya existe");
            }

            try
            {
                IQueryable<Usuario> queryUsuario = await _repositorio.Consultar(u => u.IdUsuario == entidad.IdUsuario);

                Usuario usuarioEditar = queryUsuario.First();

                usuarioEditar.Nombre = entidad.Nombre;
                usuarioEditar.Correo = entidad.Correo;
                usuarioEditar.Telefono = entidad.Telefono;
                usuarioEditar.IdRol = entidad.IdRol;

                if (usuarioEditar.NombreFoto == "")
                {
                    usuarioEditar.NombreFoto = nombreFoto;
                }

                if (foto != null)
                {
                    string urlFoto = await _firebaseService.SubirStorage(foto, "carpeta_usuario", usuarioEditar.NombreFoto);
                    usuarioEditar.UrlFoto = urlFoto;
                }

                bool respuesta = await _repositorio.Editar(usuarioEditar);

                if (!respuesta)
                {
                    throw new TaskCanceledException("No se pudo modificar el usuario");
                }

                Usuario usuarioEditado = queryUsuario.Include(r => r.IdRolNavigation).First();
                return usuarioEditado;
            }
            catch (Exception) {
                {
                    throw;
                }
            }
        }

        public async Task<bool> EliminarUsuario(int idUsuario)
        {
                try
                {
                    Usuario usuarioEncontrado = await _repositorio.Obtener(u => u.IdUsuario == idUsuario);

                    if(usuarioEncontrado == null)
                    {
                        throw new TaskCanceledException("El usuario no existe");
                    }

                    string nombreFoto = usuarioEncontrado.NombreFoto;
                    bool respuesta = await _repositorio.Eliminar(usuarioEncontrado);

                    if (respuesta) await _firebaseService.EliminarStorage("carpeta_usuario", nombreFoto);

                    return true;

                } catch (Exception) {
                    throw;
                }
        }



        public async Task<Usuario> ObtenerPorCredenciales(string correo, string clave)
        {
            string claveEncriptada = _utilidadesService.ConvertirSha256(clave);
            Usuario usuarioEncontrado = await _repositorio.Obtener(u => correo.Equals(u.Correo) && u.Clave.Equals(claveEncriptada));

            return usuarioEncontrado;
        }

        public async Task<Usuario> ObtenerPorId(int idUsuario)
        {
            IQueryable<Usuario> query = await _repositorio.Consultar(u => u.IdUsuario == idUsuario);
            Usuario resultado = query.Include(r => r.IdRolNavigation).FirstOrDefault();
            return resultado;
        }
        public async Task<bool> GuardarPerfil(Usuario entidad)
        {
            try{
                Usuario usuarioEncontrado = await _repositorio.Obtener(u => u.IdUsuario == entidad.IdUsuario);
                if(usuarioEncontrado == null) throw new TaskCanceledException("El usuario no existe");

                usuarioEncontrado.Correo = entidad.Correo;
                usuarioEncontrado.Telefono = entidad.Telefono;

                bool respuesta = await _repositorio.Editar(usuarioEncontrado);

                return respuesta;
            } catch {
                throw;
            }
        }
        public async Task<bool> CambiarClave(int idUsuario, string claveActual, string claveNueva)
        {
            try
            {
                Usuario usuarioEncontrado = await _repositorio.Obtener(u => u.IdUsuario == idUsuario);
                if (usuarioEncontrado == null) throw new TaskCanceledException("El usuario no existe");

                if (usuarioEncontrado.Clave != _utilidadesService.ConvertirSha256(claveActual)) 
                    throw new TaskCanceledException("La contraseña no es correcta.");

                usuarioEncontrado.Clave = _utilidadesService.ConvertirSha256(claveNueva);

                bool respuesta = await _repositorio.Editar(usuarioEncontrado);
                return respuesta;
            }
            catch
            {
                throw;
            }
        }

        public async Task<bool> RestablecerClave(string correo, string urlPlantillaCorreo)
        {
            try
            {
                Usuario usuarioEncontrado = await _repositorio.Obtener(u => u.Correo == correo);
                if (usuarioEncontrado == null) throw new TaskCanceledException("No encontramos ningun usuario asociado a ese correo.");

                string claveGenerada = _utilidadesService.GenerarClave();
                usuarioEncontrado.Clave = _utilidadesService.ConvertirSha256(claveGenerada);

                urlPlantillaCorreo = urlPlantillaCorreo.Replace("[clave]", claveGenerada);
                string htmlCorreo = "";

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(urlPlantillaCorreo);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    using (Stream dataStream = response.GetResponseStream())
                    {
                        StreamReader reader = null;

                        if (response.CharacterSet == null)
                        {
                            reader = new StreamReader(dataStream);
                        }
                        else
                        {
                            reader = new StreamReader(dataStream, Encoding.GetEncoding(response.CharacterSet));
                        }

                        htmlCorreo = reader.ReadToEnd();
                        response.Close();
                        reader.Close();
                    }
                }

                bool correoEnviado = false;

                if (htmlCorreo != "")
                {
                    correoEnviado = await _correoService.EnviarCorreo(correo, "Contraseña restablecida.", htmlCorreo);
                    if(!correoEnviado) throw new TaskCanceledException("Tenemos problemas. Por favor, intentelo mas tarde.");
                }

                bool respuesta = await _repositorio.Editar(usuarioEncontrado);
                return respuesta;

            }
            catch
            {
                throw;
            }
        }
    }
}

