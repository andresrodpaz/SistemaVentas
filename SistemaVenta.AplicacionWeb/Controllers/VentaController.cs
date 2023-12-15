using Microsoft.AspNetCore.Mvc;

using AutoMapper;
using SistemaVenta.AplicacionWeb.Models.ViewModels;
using SistemaVenta.AplicacionWeb.Utilidades.Response;
using SistemaVenta.BBL.Interfaces;
using SistemaVenta.Entity;

using DinkToPdf;
using DinkToPdf.Contracts;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace SistemaVenta.AplicacionWeb.Controllers
{
    [Authorize]
    public class VentaController : Controller
    {

        private readonly ITipoDocumentoVentaService _tipoDocumentoVentaService;
        private readonly IVentaService _ventaService;
        private readonly IMapper _mapper;
        private readonly IConverter _converter;

        /// <summary>
        /// Constructor que inicializa una nueva instancia del controlador de ventas.
        /// </summary>
        /// <param name="tipoDocumentoVentaService">Servicio de tipos de documento de venta.</param>
        /// <param name="ventaService">Servicio de ventas.</param>
        /// <param name="mapper">Instancia de IMapper utilizada para mapear entre modelos y entidades.</param>
        /// <param name="converter">Instancia de IConverter utilizada para convertir HTML a PDF.</param>
        public VentaController(ITipoDocumentoVentaService tipoDocumentoVentaService, IVentaService ventaService, IMapper mapper, IConverter converter)
        {
            _tipoDocumentoVentaService = tipoDocumentoVentaService;
            _ventaService = ventaService;   
            _mapper = mapper;
            _converter = converter;
        }


        /// <summary>
        /// Acción HTTP GET que devuelve la vista para realizar una nueva venta.
        /// </summary>
        /// <returns>Vista para realizar una nueva venta.</returns>
        public IActionResult NuevaVenta()
        {
            return View();
        }
        /// <summary>
        /// Acción HTTP GET que devuelve la vista para visualizar el historial de ventas.
        /// </summary>
        /// <returns>Vista para visualizar el historial de ventas.</returns>
        public IActionResult HistorialVenta()
        {
            return View();
        }

        /// <summary>
        /// Acción HTTP GET que obtiene la lista de tipos de documento de venta.
        /// </summary>
        /// <returns>Respuesta HTTP 200 con la lista de tipos de documento de venta.</returns>
        [HttpGet]
        public async Task<IActionResult> ListaTipoDocumentoVenta()
        {
            List<VMTipoDocumentoVenta> vmTipoDocumento = _mapper.Map<List<VMTipoDocumentoVenta>>(await _tipoDocumentoVentaService.Lista());
            return StatusCode(StatusCodes.Status200OK, vmTipoDocumento);
        }

        /// <summary>
        /// Acción HTTP GET que obtiene la lista de productos según una cadena de búsqueda.
        /// </summary>
        /// <param name="busqueda">Cadena de búsqueda para filtrar los productos.</param>
        /// <returns>Respuesta HTTP 200 con la lista de productos filtrada.</returns>
        [HttpGet]
        public async Task<IActionResult> ObtenerProductos(string busqueda)
        {
            List<VMProducto> vmListaProductos = _mapper.Map<List<VMProducto>>(await _ventaService.ObtenerProductos(busqueda));
            return StatusCode(StatusCodes.Status200OK, vmListaProductos);
        }

        /// <summary>
        /// Acción HTTP POST que registra una nueva venta.
        /// </summary>
        /// <param name="modelo">Datos de la venta en formato JSON.</param>
        /// <returns>Respuesta HTTP 200 con un objeto GenericResponse que contiene la venta registrada.</returns>
        [HttpPost]
        public async Task<IActionResult> RegistrarVenta([FromBody] VMVenta modelo)
        {
            GenericResponse<VMVenta> genericResponse = new GenericResponse<VMVenta>();
            try
            {
                ClaimsPrincipal claimUser = HttpContext.User;
                string idUsuario = claimUser.Claims
                    .Where(c => c.Type == ClaimTypes.NameIdentifier) //ClaimTypes.NameIdentifier viene de accesoController, allí a NameIdentifier se le asigna el id del usario
                    .Select(c => c.Value)
                    .FirstOrDefault();


                modelo.IdUsuario = int.Parse(idUsuario);

                Venta ventaCreada = await _ventaService.Registrar(_mapper.Map<Venta>(modelo));
                modelo = _mapper.Map<VMVenta>(ventaCreada);

                genericResponse.Estado = true;
                genericResponse.Objeto = modelo;
            } catch(Exception ex) {
                genericResponse.Estado = false;
                genericResponse.Mensaje = ex.Message;
                Console.WriteLine(ex.ToString());
            }
            return StatusCode(StatusCodes.Status200OK, genericResponse);
        }

        /// <summary>
        /// Acción HTTP GET que obtiene el historial de ventas según el número de venta y un rango de fechas.
        /// </summary>
        /// <param name="numeroVenta">Número de la venta a filtrar.</param>
        /// <param name="fechaInicio">Fecha de inicio del rango de fechas.</param>
        /// <param name="fechaFin">Fecha de fin del rango de fechas.</param>
        /// <returns>Respuesta HTTP 200 con el historial de ventas filtrado.</returns>
        [HttpGet]
        public async Task<IActionResult> Historial(string numeroVenta, string fechaInicio, string fechaFin)
        {
            List<VMVenta> vmHistorialVentas = _mapper.Map<List<VMVenta>>(await _ventaService.Historial(numeroVenta, fechaInicio, fechaFin));
            return StatusCode(StatusCodes.Status200OK, vmHistorialVentas);
        }

        /// <summary>
        /// Acción HTTP GET que genera y muestra un archivo PDF de una venta específica.
        /// </summary>
        /// <param name="numeroVenta">Número de la venta para generar el PDF.</param>
        /// <returns>Archivo PDF de la venta.</returns>
        public IActionResult MostrarPDFVenta(string numeroVenta)
        {
            string urlPlantillaVista = $"{this.Request.Scheme}://{this.Request.Host}/Plantilla/PDFVenta?numeroVenta={numeroVenta}";

            var pdf = new HtmlToPdfDocument()
            {
                GlobalSettings = new GlobalSettings()
                {
                    PaperSize = PaperKind.A4,
                    Orientation = Orientation.Portrait,

                },
                Objects =
                {
                    new ObjectSettings()
                    {
                        Page = urlPlantillaVista
                    }
                }
            };
            var archivoPDF = _converter.Convert(pdf);
            return File(archivoPDF, "application/pdf");
        }

    }
}
