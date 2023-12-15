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
    public class NegocioController : Controller
    {

        private readonly IMapper _mapper;
        private readonly INegocioService _negocioService;

        /// <summary>
        /// Constructor que inicializa una nueva instancia del controlador de negocio.
        /// </summary>
        /// <param name="mapper">Instancia del servicio de mapeo AutoMapper.</param>
        /// <param name="negocioService">Instancia del servicio de negocio.</param>
        public NegocioController(IMapper mapper, INegocioService negocioService)
        {
            _mapper = mapper;
            _negocioService = negocioService;
        }

        /// <summary>
        /// Acción que devuelve la vista principal de la gestión de negocio.
        /// </summary>
        /// <returns>Vista principal de la gestión de negocio.</returns>
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Acción HTTP GET que obtiene la información del negocio y devuelve una respuesta HTTP 200 con el resultado.
        /// </summary>
        /// <returns>Respuesta HTTP 200 con un objeto GenericResponse que contiene la información del negocio.</returns>
        [HttpGet]
        public async Task<IActionResult> Obtener()
        {
            GenericResponse<VMNegocio> genericResponse = new GenericResponse<VMNegocio>();
            try
            {
                VMNegocio vmNegiocio = _mapper.Map<VMNegocio>(await _negocioService.Obtener());
                genericResponse.Estado = true;
                genericResponse.Objeto = vmNegiocio;
            }
            catch (Exception ex)
            {
                genericResponse.Estado = false;
                genericResponse.Mensaje = ex.Message;
                throw;
            }
            return StatusCode(StatusCodes.Status200OK, genericResponse);
        }

        /// <summary>
        /// Acción HTTP POST que guarda los cambios en la información del negocio y devuelve una respuesta HTTP 200 con el resultado.
        /// </summary>
        /// <param name="logo">Instancia de IFormFile que representa el logo del negocio.</param>
        /// <param name="modelo">Cadena JSON que representa el modelo de vista del negocio.</param>
        /// <returns>Respuesta HTTP 200 con un objeto GenericResponse que indica el resultado de la operación.</returns>
        [HttpPost]
        public async Task<IActionResult> GuardarCambios([FromForm]IFormFile logo, [FromForm] string modelo)
        {
            GenericResponse<VMNegocio> genericResponse = new GenericResponse<VMNegocio>();
            try
            {
                VMNegocio vmNegiocio = JsonConvert.DeserializeObject<VMNegocio>(modelo);
                string nombreLogo = "";
                Stream LogoStream = null;

                if(logo != null)
                {
                    string nombreCodificado = Guid.NewGuid().ToString("N");
                    string extension = Path.GetExtension(logo.FileName);
                    nombreLogo = string.Concat(nombreCodificado, extension);
                    LogoStream = logo.OpenReadStream();

                }

                Negocio negiocioEditado = await _negocioService.GuardarCambios(_mapper.Map<Negocio>(vmNegiocio), LogoStream, nombreLogo);

                vmNegiocio = _mapper.Map<VMNegocio>(negiocioEditado);

                genericResponse.Estado = true;
                genericResponse.Objeto = vmNegiocio;
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
