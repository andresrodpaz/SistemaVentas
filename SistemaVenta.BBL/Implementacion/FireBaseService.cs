using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SistemaVenta.BBL.Interfaces;
using Firebase.Auth;
using Firebase.Storage;

using SistemaVenta.Entity;
using SistemaVenta.DAL.Interfaces;


namespace SistemaVenta.BBL.Implementacion
{
    /// <summary>
    /// Implementación de servicio para la realizacion de operaciones en FireBase
    /// </summary>
    public class FireBaseService : IFireBaseService
    {
        private readonly IGenericRepository<Configuracion> _configuracionRepository;

        /// <summary>
        /// Constructor de la clase CorreoService.
        /// </summary>
        /// <param name="repositoriy">Instancia del repositorio de configuración para acceder a la información necesaria.</param>
        public FireBaseService(IGenericRepository<Configuracion> repository)
        {
            _configuracionRepository = repository;
        }

        /// <summary>
        /// Sube un archivo al sistema de almacenamiento Firebase.
        /// </summary>
        /// <param name="streamArchivo">El flujo de datos del archivo a cargar.</param>
        /// <param name="carpetaDestino">La carpeta de destino en el sistema de almacenamiento.</param>
        /// <param name="nombreArchivo">El nombre del archivo en el sistema de almacenamiento.</param>
        /// <returns>
        /// La URL del archivo cargado, o una cadena vacía si hay algún error.
        /// </returns>
        /// <remarks>
        /// Este método utiliza Firebase Storage para cargar un archivo proporcionado como un flujo de datos a una carpeta y con un nombre específico en el sistema de almacenamiento.
        /// La autenticación con Firebase se realiza utilizando credenciales obtenidas desde la base de datos.
        /// </remarks>
        public async Task<string> SubirStorage(Stream streamArchivo, string carpetaDestino, string nombreArchivo)
        {
            string UrlImgen = "";
            try
            {
                // Consultar configuraciones desde la base de datos
                IQueryable<Configuracion> query = await _configuracionRepository.Consultar(c => c.Recurso.Equals("FireBase_Storage"));
                Dictionary<string, string> config = query.ToDictionary(keySelector: c => c.Propiedad, elementSelector: c => c.Valor);

                // Inicializar el proveedor de autenticación de Firebase con la clave de la API de Firebase
                var auth = new FirebaseAuthProvider(new FirebaseConfig(config["api_key"]));

                // Autenticarse con las credenciales obtenidas desde la base de datos
                var a = await auth.SignInWithEmailAndPasswordAsync(config["email"], config["clave"]);

                // Configurar opciones de almacenamiento de Firebase
                var cancellation = new CancellationTokenSource();
                var task = new FirebaseStorage(
                    config["ruta"],
                    new FirebaseStorageOptions
                    {
                        AuthTokenAsyncFactory = () => Task.FromResult(a.FirebaseToken),
                        ThrowOnCancel = true
                    }
                    ).Child(carpetaDestino)
                    .Child(nombreArchivo)
                    .PutAsync(streamArchivo, cancellation.Token);
                
                // Esperar a que la tarea de carga se complete y obtener la URL del archivo cargado
                UrlImgen = await task;
            }
            catch (Exception)
            {
                // Manejar cualquier excepción y asignar una cadena vacía si hay un error
                UrlImgen = "";
            }
            return UrlImgen;
        }
        /*-------------------------------- Con Credenciales por si falla la BBDD ----------------------------------------*/
        //Misma funcion sin tener que acceder a la BBDD (en caso de fallo)
        /*
         public async Task<string> SubirStorage(Stream streamArchivo, string carpetaDestino, string nombreArchivo)
        {
            string UrlImgen = "";
            try
            {
                // Inicializar el proveedor de autenticación de Firebase con la clave de la API
                var auth = new FirebaseAuthProvider(new FirebaseConfig("AIzaSyBuxUl3KQ8vqcavDumzlogGMCdn6Ldvn9w"));
        
                // Autenticarse con las credenciales proporcionadas
                var a = await auth.SignInWithEmailAndPasswordAsync("arodriguezpaz00@gmail.com", "rodpaz0208");

                // Configurar opciones de almacenamiento de Firebase
                var cancellation = new CancellationTokenSource();
                var options = new FirebaseStorageOptions
                {
                    AuthTokenAsyncFactory = () => Task.FromResult(a.FirebaseToken),
                    ThrowOnCancel = true
                };

                // Inicializar Firebase Storage y cargar el archivo en la carpeta y con el nombre especificados
                var storage = new FirebaseStorage("sistemaventas-5c336.appspot.com", options);
                var task = storage
                .Child(carpetaDestino)
                .Child(nombreArchivo)
                .PutAsync(streamArchivo, cancellation.Token);

                // Obtener la URL del archivo cargado
                UrlImgen = await task;
            }
           catch (Exception)
            {
                // Manejar cualquier excepción y asignar una cadena vacía si hay un error
                UrlImgen = "";
            }
             return UrlImgen;
        }
         */



        /// <summary>
        /// Elimina un archivo del sistema de almacenamiento Firebase.
        /// </summary>
        /// <param name="carpetaDestino">La carpeta de destino en el sistema de almacenamiento.</param>
        /// <param name="nombreArchivo">El nombre del archivo en el sistema de almacenamiento.</param>
        /// <returns>
        /// `true` si la eliminación se realiza con éxito, de lo contrario, `false`.
        /// </returns>
        /// <remarks>
        /// Este método utiliza Firebase Storage para eliminar un archivo ubicado en una carpeta específica del sistema de almacenamiento.
        /// La autenticación con Firebase se realiza utilizando las credenciales proporcionadas en la configuración.
        /// </remarks>
        public async Task<bool> EliminarStorage(string carpetaDestino, string nombreArchivo)
        {
            try
            {
                // Consultar configuraciones desde la base de datos
                IQueryable<Configuracion> query = await _configuracionRepository.Consultar(c => c.Recurso.Equals("FireBase_Storage"));
                Dictionary<string, string> config = query.ToDictionary(keySelector: c => c.Propiedad, elementSelector: c => c.Valor);

                // Autenticarse con las credenciales proporcionadas
                var auth = new FirebaseAuthProvider(new FirebaseConfig(config["api_key"]));
                var a = await auth.SignInWithEmailAndPasswordAsync(config["email"], config["clave"]);

                // Configurar opciones de almacenamiento de Firebase
                var cancellation = new CancellationTokenSource();
                var options = new FirebaseStorageOptions
                {
                    AuthTokenAsyncFactory = () => Task.FromResult(a.FirebaseToken),
                    ThrowOnCancel = true
                };

                // Inicializar Firebase Storage y eliminar el archivo en la carpeta y con el nombre especificados
                var storage = new FirebaseStorage(config["ruta"], options);
                var task = storage
                    .Child(carpetaDestino)
                    .Child(nombreArchivo)
                    .DeleteAsync();

                // Esperar a que la tarea de eliminación se complete
                await task;

                return true; // La eliminación fue exitosa
            }
            catch (Exception)
            {
                return false; // Ocurrió un error durante la eliminación
            }

        }

        /*-------------------------------- Con Credenciales por si falla la BBDD ----------------------------------------*/
        /*
        public async Task<bool> EliminarStorage(string carpetaDestino, string nombreArchivo)
        {
            try
            {
                

                // Inicializar el proveedor de autenticación de Firebase con la clave de la API
                var auth = new FirebaseAuthProvider(new FirebaseConfig("AIzaSyBuxUl3KQ8vqcavDumzlogGMCdn6Ldvn9w"));
        
                // Autenticarse con las credenciales proporcionadas
                var a = await auth.SignInWithEmailAndPasswordAsync("arodriguezpaz00@gmail.com", "rodpaz0208");

                // Configurar opciones de almacenamiento de Firebase
                var cancellation = new CancellationTokenSource();
                
                // Inicializar Firebase Storage y cargar el archivo en la carpeta y con el nombre especificados
                var task = new FirebaseStorage(
                    "sistemaventas-5c336.appspot.com",
                    new FirebaseStorageOptions
                    {
                        AuthTokenAsyncFactory = () => Task.FromResult(a.FirebaseToken),
                        ThrowOnCancel = true
                    }
                    ).Child(carpetaDestino)
                    .Child(nombreArchivo)
                    .DeleteAsync();

                await task;
                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }*/
    }
}
