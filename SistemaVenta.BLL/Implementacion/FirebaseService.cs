using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SistemaVenta.BLL.Interfaces;
using Firebase.Auth;
using Firebase.Storage;
using SistemaVenta.Entity;
using SistemaVenta.DAL.Interfaces;
using Firebase.Auth.Providers;
using System.Net.Http.Headers;
using Firebase.Auth.Repository;

namespace SistemaVenta.BLL.Implementacion
{
    public class FirebaseService : IFirebaseService
    {
        private readonly IGenericRepository<Configuracion> _repositorio;

        public FirebaseService(IGenericRepository<Configuracion> repositorio)
        {
            _repositorio = repositorio;
        }

        public async Task<string> SubirStorage(Stream StreamArchivo, string CarpetaDestino, string NombreDestino)
        {
            string UrlImagen = "";

            try
            {
                IQueryable<Configuracion> query = await _repositorio.Consultar(c => c.Recurso.Equals("Firebase_Storage"));

                Dictionary<string, string> Config = query.ToDictionary(keySelector: c => c.Propiedad, elementSelector: c => c.Valor);

                var config = new FirebaseAuthConfig
                {
                    ApiKey = Config["api_key"]
                };

                var auth = new FirebaseAuthClient(config);
                var a = await auth.SignInWithEmailAndPasswordAsync(Config["email"], Config["clave"]);

                var cancellation = new CancellationTokenSource();

                var task = new FirebaseStorage(
                    Config["ruta"],
                    new FirebaseStorageOptions
                    {
                        ThrowOnCancel = true
                    })
                    .Child(Config[CarpetaDestino])
                    .Child(Config[NombreDestino])
                    .PutAsync(StreamArchivo, cancellation.Token);

                UrlImagen = await task;
                

            } catch (Exception ex)
            {
                return "";
            }
            return UrlImagen;
        }
        public async Task<bool> EliminarStorage(string CarpetaDestino, string NombreDestino)
        {
            try
            {
                IQueryable<Configuracion> query = await _repositorio.Consultar(c => c.Recurso.Equals("Firebase_Storage"));

                Dictionary<string, string> Config = query.ToDictionary(keySelector: c => c.Propiedad, elementSelector: c => c.Valor);

                var config = new FirebaseAuthConfig
                {
                    ApiKey = Config["api_key"]
                };

                var auth = new FirebaseAuthClient(config);
                var a = await auth.SignInWithEmailAndPasswordAsync(Config["email"], Config["clave"]);

                var cancellation = new CancellationTokenSource();

                var task = new FirebaseStorage(
                    Config["ruta"],
                    new FirebaseStorageOptions
                    {
                        ThrowOnCancel = true
                    })
                    .Child(Config[CarpetaDestino])
                    .Child(Config[NombreDestino])
                    .DeleteAsync();

                await task;
                return true;

            }
            catch (Exception ex)
            {
                return false;
            }
        }

    }
}
