using Microsoft.AspNetCore.Mvc;

using AutoMapper;
using SistemaVenta.AplicacionWeb.Models.ViewModels;
using SistemaVenta.BBL.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace SistemaVenta.AplicacionWeb.Controllers
{
    [Authorize]
    public class ReporteController : Controller
    {

        private readonly IMapper _mapper;
        private readonly IVentaService _ventaService;

        /// <summary>
        /// Constructor que inicializa una nueva instancia del controlador de reportes de ventas.
        /// </summary>
        public ReporteController(IMapper mapper, IVentaService ventaService)
        {
            _mapper = mapper;
            _ventaService = ventaService;
        }

        /// <summary>
        /// Acción que devuelve la vista principal de los reportes.
        /// </summary>
        /// <returns>Vista principal de los reportes.</returns>
        public IActionResult Index()
        {
            return View();
        }

        
        /// <summary>
        /// Acción HTTP GET que obtiene un reporte de ventas según las fechas proporcionadas.
        /// </summary>
        /// <param name="fechaInicio">Fecha de inicio del periodo del reporte.</param>
        /// <param name="fechaFin">Fecha de fin del periodo del reporte.</param>
        /// <returns>Respuesta HTTP 200 con un JSON que contiene los datos del reporte de ventas.</returns>
        [HttpGet]
        public async Task<IActionResult> ReporteVenta(string fechaInicio, string fechaFin)
        {
            List<VMReporteVenta> vmLista = _mapper.Map<List<VMReporteVenta>>(await _ventaService.Reporte(fechaInicio, fechaFin));
            return StatusCode(StatusCodes.Status200OK, new { data = vmLista });

        }
    }
}
