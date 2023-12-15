using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using SistemaVenta.AplicacionWeb.Models;
using System.Diagnostics;
using System.Net;
using System.Security.Claims;

using AutoMapper;
using SistemaVenta.AplicacionWeb.Models.ViewModels;
using SistemaVenta.AplicacionWeb.Utilidades.Response;
using SistemaVenta.BBL.Interfaces;
using SistemaVenta.Entity;




namespace SistemaVenta.AplicacionWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly IMapper _mapper;
        private readonly IUsuarioService _usuarioService;

        /// <summary>
        /// Constructor que inicializa una nueva instancia del controlador de inicio y perfil del usuario.
        /// </summary>
        public HomeController(IMapper mapper, IUsuarioService usuarioService, ILogger<HomeController> logger)
        {
            _mapper = mapper;
            _usuarioService = usuarioService;
            _logger = logger;
        }

        /// <summary>
        /// Acción que devuelve la vista principal del sistema.
        /// </summary>
        /// <returns>Vista principal del sistema.</returns>
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Acción que devuelve la vista de privacidad.
        /// </summary>
        /// <returns>Vista de privacidad.</returns>
        public IActionResult Privacy()
        {
            return View();
        }

        /// <summary>
        /// Acción que devuelve la vista del perfil del usuario.
        /// </summary>
        /// <returns>Vista del perfil del usuario.</returns>
        public IActionResult Perfil()
        {
            return View();
        }

        /// <summary>
        /// Acción que devuelve la vista de error junto con un identificador de solicitud único.
        /// </summary>
        /// <returns>Vista de error.</returns>
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        /// <summary>
        /// Método asincrónico para cerrar la sesión del usuario y redirigirlo a la página de inicio de sesión.
        /// </summary>
        /// <returns>Redirección a la página de inicio de sesión.</returns>
        public async Task<IActionResult>Salir()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login","Acceso");
        }

        /// <summary>
        /// Acción HTTP GET que obtiene la información del usuario actual y devuelve una respuesta HTTP 200 con el resultado.
        /// </summary>
        /// <returns>Respuesta HTTP 200 con un objeto GenericResponse que contiene la información del usuario.</returns>
        [HttpGet]
        public async Task<IActionResult> ObtenerUsuario()
        {
            GenericResponse<VMUsuario> genericResponse = new GenericResponse<VMUsuario>();

            try
            {
                // Obtiene el ID del usuario actual desde las claims del contexto HTTP
                ClaimsPrincipal claimUser = HttpContext.User;
                string idUsuario = claimUser.Claims
                    .Where(c => c.Type == ClaimTypes.NameIdentifier) //ClaimTypes.NameIdentifier viene de accesoController, allí a NameIdentifier se le asigna el id del usario
                    .Select(c => c.Value)
                    .FirstOrDefault();

                // Obtiene y mapea la información del usuario utilizando el servicio correspondiente
                VMUsuario userEncontrado = _mapper.Map<VMUsuario>(await _usuarioService.ObtenerPorId(int.Parse(idUsuario)));

                // Configura el objeto GenericResponse con el resultado exitoso y la información del usuario
                genericResponse.Estado = true;
                genericResponse.Objeto = userEncontrado;

            }
            catch (Exception ex)
            {
                // En caso de error, registra el error y configura el objeto GenericResponse con el resultado fallido y el mensaje de error
                genericResponse.Estado=false;
                genericResponse.Mensaje = ex.Message;
                throw;
            }

            // Retorna una respuesta HTTP 200 con el objeto GenericResponse que contiene la información del usuario
            return StatusCode(StatusCodes.Status200OK, genericResponse);
        }

        /// <summary>
        /// Acción HTTP POST que guarda los cambios en el perfil del usuario y devuelve una respuesta HTTP 200 con el resultado.
        /// </summary>
        /// <param name="modelo">Modelo que contiene los datos del perfil del usuario.</param>
        /// <returns>Respuesta HTTP 200 con un objeto GenericResponse que indica el resultado de la operación.</returns>
        [HttpPost]
        public async Task<IActionResult> GuardarPerfil([FromBody]VMUsuario modelo)
        {
            GenericResponse<VMUsuario> genericResponse = new GenericResponse<VMUsuario>();

            try
            {
                // Obtiene el ID del usuario actual desde las claims del contexto HTTP
                ClaimsPrincipal claimUser = HttpContext.User;
                string idUsuario = claimUser.Claims
                    .Where(c => c.Type == ClaimTypes.NameIdentifier) //ClaimTypes.NameIdentifier viene de accesoController, allí a NameIdentifier se le asigna el id del usario
                    .Select(c => c.Value)
                    .FirstOrDefault();

                // Mapea el modelo de vista a la entidad de usuario y asigna el ID del usuario actual
                Usuario entidad = _mapper.Map<Usuario>(modelo);
                entidad.IdUsuario = int.Parse(idUsuario);

                // Guarda los cambios en el perfil del usuario utilizando el servicio correspondiente
                bool resultado = await _usuarioService.GuardarPerfil(entidad);

                // Configura el objeto GenericResponse con el resultado exitoso
                genericResponse.Estado = true;
            
            }
            catch (Exception ex)
            {
                // En caso de error, registra el error y configura el objeto GenericResponse con el resultado fallido y el mensaje de error
                genericResponse.Estado = false;
                genericResponse.Mensaje = ex.Message;
                throw;
            }
            // Retorna una respuesta HTTP 200 con el objeto GenericResponse que indica el resultado de la operación
            return StatusCode(StatusCodes.Status200OK, genericResponse);
        }

        /// <summary>
        /// Acción HTTP POST que cambia la clave del usuario y devuelve una respuesta HTTP 200 con el resultado.
        /// </summary>
        /// <param name="modelo">Modelo que contiene los datos del perfil del usuario.</param>
        /// <returns>Respuesta HTTP 200 con un objeto GenericResponse que indica el resultado de la operación.</returns>
        [HttpPost]
        public async Task<IActionResult> CambiarClave([FromBody] VMCambiarClave modelo)
        {
            GenericResponse<bool> genericResponse = new GenericResponse<bool>();

            try
            {
                ClaimsPrincipal claimUser = HttpContext.User;
                string idUsuario = claimUser.Claims
                    .Where(c => c.Type == ClaimTypes.NameIdentifier) //ClaimTypes.NameIdentifier viene de accesoController, allí a NameIdentifier se le asigna el id del usario
                    .Select(c => c.Value)
                    .FirstOrDefault();

                
                bool resultado = await _usuarioService.CambiarClave(int.Parse(idUsuario), modelo.ClaveActual, modelo.ClaveNueva);


                genericResponse.Estado = true;


            }
            catch (Exception ex)
            {
                genericResponse.Estado = false;
                genericResponse.Mensaje = ex.Message;
                throw;
            }
            return StatusCode(StatusCodes.Status200OK, genericResponse);
        }


    }
}