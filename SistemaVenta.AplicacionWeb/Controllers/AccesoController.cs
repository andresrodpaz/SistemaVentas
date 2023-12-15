using Microsoft.AspNetCore.Mvc;

using SistemaVenta.AplicacionWeb.Models.ViewModels;
using SistemaVenta.BBL.Interfaces;
using SistemaVenta.Entity;

using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;


namespace SistemaVenta.AplicacionWeb.Controllers
{
    public class AccesoController : Controller
    {
        private readonly IUsuarioService _usuarioService;

        // Constructor que recibe una instancia de IUsuarioService a través de la inyección de dependencias
        public AccesoController(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }
        // Método que muestra la vista de inicio de sesión
        public IActionResult Login()
        {

            // Verifica si el usuario ya está autenticado, en cuyo caso redirige al Dashboard
            ClaimsPrincipal claimsUser = HttpContext.User;

            if (claimsUser.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Dashboard");
            }
            // Si no está autenticado, muestra la vista de inicio de sesión
            return View();
        }

        // Método que muestra la vista para restablecer la clave
        public IActionResult RestablecerClave()
        {
            return View();
        }

        /// <summary>
        /// Maneja la autenticación de usuarios después de enviar el formulario de inicio de sesión.
        /// </summary>
        /// <param name="modelo">Modelo que contiene las credenciales del usuario.</param>
        /// <returns>Una acción que redirige a la página principal o muestra un mensaje de error.</returns>
        [HttpPost]
        public async Task<IActionResult> Login(VMUsuarioLogin modelo)
        {
            // Intenta obtener un usuario por sus credenciales (correo y clave) utilizando el servicio de usuarios
            Usuario usuarioEncontrado = await _usuarioService.ObtenerPorCredenciales(modelo.Correo, modelo.Clave);

            // Verifica si no se encuentra un usuario con las credenciales proporcionadas
            if (usuarioEncontrado == null)
            {
                // Configura un mensaje de error y devuelve la vista de inicio de sesión que se muestra en la vista
                ViewData["Mensaje"] = "No se ha encontrado un usuario con esas credenciales";
                return View();
            }

            // Si se encuentra el usuario, limpia cualquier mensaje de error anterior
            ViewData["Mensaje"] = null;

            // Crea una lista de claims para el usuario autenticado, trabajando con las cookies
            List<Claim> claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, usuarioEncontrado.Nombre),
                new Claim(ClaimTypes.NameIdentifier, usuarioEncontrado.IdUsuario.ToString()),
                new Claim(ClaimTypes.Role, usuarioEncontrado.IdRol.ToString()),
                new Claim("UrlFoto", usuarioEncontrado.UrlFoto)
            };

            // Crea una identidad de claims con el esquema de autenticación de cookies
            ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            // Configura las propiedades de autenticación, como la posibilidad de actualización y la persistencia de la sesión
            AuthenticationProperties properties = new AuthenticationProperties()
            {
                AllowRefresh = true,
                //Dependiendo de si el usuario quiere o no que se mantega la sesion, segun el checkbox del Login
                IsPersistent = modelo.MantenerSesion,
            };


            // Realiza la autenticación del usuario mediante la creación de una cookie de autenticación
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme, 
                new ClaimsPrincipal(claimsIdentity), 
                properties
                );
            //Redirecciona a Metodo, Controller donde esta e´l metodo
            return RedirectToAction("Index", "Home");
        }

        /// <summary>
        /// Maneja la solicitud de restablecimiento de clave del usuario.
        /// </summary>
        /// <param name="modelo">Modelo que contiene la información necesaria para restablecer la clave.</param>
        /// <returns>Una vista con mensajes de éxito o error después de intentar restablecer la clave.</returns>
        [HttpPost]
        public async Task<IActionResult> RestablecerClave(VMUsuarioLogin modelo)
        {
            try
            {
                // Construye la URL para la plantilla de correo electrónico que incluye un marcador de posición para la clave
                string urlPlantillaCorreo = $"{this.Request.Scheme}://{this.Request.Host}/Plantilla/RestablecerClave?clave=[clave]";

                // Intenta restablecer la clave del usuario utilizando el servicio de usuarios
                bool result = await _usuarioService.RestablecerClave(modelo.Correo, urlPlantillaCorreo);

                // Según el resultado, muestra mensajes apropiados en la vista
                if (result)
                {
                    ViewData["Mensaje"] = "¡Perfecto! Su contraseña ha sido restablecida. Por favor, revise su correo electrónico para más detalles.";
                    ViewData["MensajeError"] = null;
                }
                else
                {
                    ViewData["MensajeError"] = "¡Ups! Parece que ha habido un problema. Por favor, inténtelo de nuevo más tarde.";
                    ViewData["Mensaje"] = null;
                }
                // Retorna la vista de restablecimiento de clave
                return View();

            }
            catch (Exception ex)
            {
                ViewData["MensajeError"] = ex.Message;
                ViewData["Mensaje"] = null;
                Console.WriteLine(ex.ToString());   
                throw;
            }

        }
    }

    
}
