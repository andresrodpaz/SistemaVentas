using Microsoft.AspNetCore.Mvc;

using AutoMapper;
using Newtonsoft.Json;
using SistemaVenta.AplicacionWeb.Models.ViewModels;
using SistemaVenta.AplicacionWeb.Utilidades.Response;
using SistemaVenta.BBL.Interfaces;
using SistemaVenta.Entity;
using Microsoft.AspNetCore.Authorization;

namespace SistemaVenta.AplicacionWeb.Controllers
{
    [Authorize]
    public class UsuarioController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IUsuarioService _usuarioService;
        private readonly IRolService _rolService;

        /// <summary>
        /// Constructor que inicializa una nueva instancia del controlador de usuarios.
        /// </summary>
        /// <param name="mapper">Instancia de IMapper utilizada para mapear entre modelos y entidades.</param>
        /// <param name="usuarioService">Servicio de usuarios utilizado para gestionar operaciones relacionadas con usuarios.</param>
        /// <param name="rolService">Servicio de roles utilizado para obtener la lista de roles disponibles.</param>
        public UsuarioController(IMapper mapper, IUsuarioService usuarioService, IRolService rolService)
        {
            _mapper = mapper;
            _usuarioService = usuarioService;
            _rolService = rolService;

        }

        /// <summary>
        /// Acción HTTP GET que devuelve la vista principal del controlador de usuarios.
        /// </summary>
        /// <returns>Vista principal del controlador de usuarios.</returns>
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Acción HTTP GET que obtiene la lista de roles en el sistema.
        /// </summary>
        /// <returns>Respuesta HTTP 200 con la lista de roles.</returns>
        [HttpGet]
        public async Task<IActionResult> ListaRoles()
        {
            var lista = await _rolService.Lista();
            List<VMRol> vmListaRoles = _mapper.Map<List<VMRol>>(lista);
            return StatusCode(StatusCodes.Status200OK, vmListaRoles);
        }

        /// <summary>
        /// Acción HTTP GET que obtiene la lista de usuarios en un formato adecuado para DataTables de jQuery.
        /// </summary>
        /// <returns>Respuesta HTTP 200 con la lista de usuarios en el formato necesario para DataTables.</returns>
        [HttpGet]
        public async Task<IActionResult> ListaUsuarios()
        {
            List<VMUsuario> vmListaUsuarios = _mapper.Map<List<VMUsuario>>(await _usuarioService.Lista());
            return StatusCode(StatusCodes.Status200OK, new { data = vmListaUsuarios });
            //Va asi porque se trabaja con datatable de jquery y tiene que llevar ese formato
        }

        /// <summary>
        /// Acción HTTP POST que crea un nuevo usuario con la opción de cargar una foto de perfil.
        /// </summary>
        /// <param name="foto">Archivo de imagen que representa la foto de perfil del usuario.</param>
        /// <param name="modelo">Datos del usuario en formato JSON.</param>
        /// <returns>Respuesta HTTP 200 con un objeto GenericResponse que contiene el usuario creado y su foto de perfil.</returns>
        [HttpPost]
        public async Task<IActionResult> Crear([FromForm] IFormFile foto, [FromForm] string modelo)
        {
            GenericResponse<VMUsuario> genericResponse = new GenericResponse<VMUsuario>();

            try
            {
                VMUsuario vmUsuario = JsonConvert.DeserializeObject<VMUsuario>(modelo);
                string nombreFoto = "";

                if (foto != null)
                {
                    Console.WriteLine($"Imagen recibida: {foto.FileName}");

                    string nombre_codificado = Guid.NewGuid().ToString("N");
                    string extension = Path.GetExtension(foto.FileName);
                    nombreFoto = String.Concat(nombre_codificado, extension);

                    Console.WriteLine($"Nombre de la imagen generada: {nombreFoto}");

                    using (Stream fotoStream = foto.OpenReadStream())
                    {
                        Console.WriteLine("Procesando imagen...");
                        string urlPlantillaCorreo = $"{this.Request.Scheme}://{this.Request.Host}/Plantilla/EnviarClave?correo=[correo]&clave=[clave]";
                        Usuario usuarioCreado = await _usuarioService.CrearUsuaio(_mapper.Map<Usuario>(vmUsuario), fotoStream, nombreFoto, urlPlantillaCorreo);

                        Console.WriteLine($"Usuario creado con la imagen: {usuarioCreado.IdUsuario}");

                        vmUsuario = _mapper.Map<VMUsuario>(usuarioCreado);
                        genericResponse.Estado = true;
                        genericResponse.Objeto = vmUsuario;
                        Console.WriteLine($"Imagen guardada en: {usuarioCreado.UrlFoto}");
                    }
                }
            }
            catch (Exception ex)
            {
                genericResponse.Estado = false;
                genericResponse.Mensaje = ex.Message;

                Console.WriteLine($"Error al procesar la solicitud: {ex.Message}");
            }

            return StatusCode(StatusCodes.Status200OK, genericResponse);
        }

        /// <summary>
        /// Acción HTTP PUT que edita la información de un usuario existente, incluyendo la posibilidad de cambiar la foto de perfil.
        /// </summary>
        /// <param name="foto">Archivo de imagen que representa la nueva foto de perfil del usuario.</param>
        /// <param name="modelo">Datos del usuario en formato JSON.</param>
        /// <returns>Respuesta HTTP 200 con un objeto GenericResponse que contiene el usuario editado y su nueva foto de perfil.</returns>
        [HttpPut]
        public async Task<IActionResult> Editar([FromForm] IFormFile foto, [FromForm] string modelo)
        {
            GenericResponse<VMUsuario> genericResponse = new GenericResponse<VMUsuario>(); //GenericResponse === RestMessage
            try
            {
                VMUsuario vmUsuario = JsonConvert.DeserializeObject<VMUsuario>(modelo);
                string nombreFoto = "";
                Stream fotoStream = null;

                if (foto != null)  // Verificar si la foto no es nula
                {
                    string nombre_codificado = Guid.NewGuid().ToString("N");
                    string extension = Path.GetExtension(foto.FileName);
                    nombreFoto = String.Concat(nombre_codificado, extension);
                    fotoStream = foto.OpenReadStream();
                }

                Usuario usuarioEditado = await _usuarioService.EditarUsuario(_mapper.Map<Usuario>(vmUsuario), fotoStream, nombreFoto);

                vmUsuario = _mapper.Map<VMUsuario>(usuarioEditado);
                genericResponse.Estado = true;
                genericResponse.Objeto = vmUsuario;
            }
            catch (Exception ex)
            {
                genericResponse.Estado = false;
                genericResponse.Mensaje = ex.Message;
                // No necesitas el throw aquí, ya que ya estás manejando la excepción.
            }

            return StatusCode(StatusCodes.Status200OK, genericResponse);
        }

        /// <summary>
        /// Acción HTTP DELETE que elimina un usuario según el ID proporcionado.
        /// </summary>
        /// <param name="idUsuario">ID del usuario a eliminar.</param>
        /// <returns>Respuesta HTTP 200 con un objeto GenericResponse indicando el resultado de la operación.</returns>
        [HttpDelete]
        public async Task<IActionResult> Eliminar(int idUsuario)
        {
            GenericResponse<string> genericResponse = new GenericResponse<string>();

            try
            {
                genericResponse.Estado = await _usuarioService.DeleteUsuario(idUsuario);
            } catch (Exception ex)
            {
                genericResponse.Estado= false;
                //genericResponse.Objeto = null;
                genericResponse.Mensaje= ex.Message;
            }
            return StatusCode(StatusCodes.Status200OK, genericResponse);
        }
    } 
}

