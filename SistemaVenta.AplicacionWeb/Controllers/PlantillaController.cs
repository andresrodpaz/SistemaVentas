using Microsoft.AspNetCore.Mvc;

using AutoMapper;
using SistemaVenta.AplicacionWeb.Models.ViewModels;
using SistemaVenta.BBL.Interfaces;

namespace SistemaVenta.AplicacionWeb.Controllers
{
    public class PlantillaController : Controller
    {
        private readonly IMapper _mapper;
        private readonly INegocioService _negocioService;
        private readonly IVentaService _ventaService;

        /// <summary>
        /// Constructor que inicializa una nueva instancia del controlador Plantilla.
        /// </summary>
        /// <param name="mapper">Instancia del servicio de mapeo (AutoMapper).</param>
        /// <param name="negocioService">Instancia del servicio de negocio.</param>
        /// <param name="ventaService">Instancia del servicio de venta.</param>
        public PlantillaController(IMapper mapper, INegocioService negocioService, IVentaService ventaService)
        {
            _mapper = mapper;
            _negocioService = negocioService;
            _ventaService = ventaService;
        }

        /// <summary>
        /// Acción que devuelve la vista para enviar la clave por correo.
        /// </summary>
        /// <param name="correo">Correo electrónico del usuario.</param>
        /// <param name="clave">Clave a enviar.</param>
        /// <returns>Vista para enviar la clave por correo.</returns>
        public IActionResult EnviarClave(string correo, string clave)
        {
            ViewData["Correo"] = correo;
            ViewData["Clave"] = clave;
            ViewData["Url"] = $"{this.Request.Scheme}://{this.Request.Host}";
            return View();
        }

        /// <summary>
        /// Acción que devuelve la vista para restablecer la clave.
        /// </summary>
        /// <param name="clave">Clave a restablecer.</param>
        /// <returns>Vista para restablecer la clave.</returns>
        public IActionResult RestablecerClave(string clave)
        {
            ViewData["Clave"] = clave;
            return View();
        }

        /// <summary>
        /// Acción asincrónica que devuelve la vista para generar un PDF de los detalles de una venta.
        /// </summary>
        /// <param name="numeroVenta">Número de la venta para obtener detalles.</param>
        /// <returns>Vista para generar un PDF de los detalles de la venta.</returns>
        public async Task<IActionResult> PDFVenta(string numeroVenta)
        {
            VMVenta vmVenta = _mapper.Map<VMVenta>(await _ventaService.Detalle(numeroVenta));
            VMNegocio vmNegocio = _mapper.Map<VMNegocio>(await _negocioService.Obtener());

            VMPDFVenta modelo = new VMPDFVenta();

            modelo.venta = vmVenta;
            modelo.negocio = vmNegocio;

            return View(modelo);
        }
    }
}
