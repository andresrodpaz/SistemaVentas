using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using SistemaVenta.AplicacionWeb.Models.ViewModels;
using SistemaVenta.AplicacionWeb.Utilidades.Response;
using SistemaVenta.BBL.Interfaces;

namespace SistemaVenta.AplicacionWeb.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly IDashBoardService _dashboardService;

        /// <summary>
        /// Constructor que inicializa una nueva instancia del controlador de Dashboard.
        /// </summary>
        /// <param name="boardService">Servicio para realizar operaciones relacionadas con el dashboard.</param>
        public DashboardController(IDashBoardService boardService)
        {
            _dashboardService = boardService;
        }

        // <summary>
        /// Acción que devuelve la vista principal del dashboard.
        /// </summary>
        /// <returns>Vista principal del dashboard.</returns>
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Acción HTTP GET que obtiene un resumen del dashboard y devuelve una respuesta HTTP 200 con el resultado.
        /// </summary>
        /// <returns>Respuesta HTTP 200 con un objeto GenericResponse que contiene el resumen del dashboard.</returns>
        [HttpGet]
        public async Task<IActionResult> ObtenerResumen()
        {
            GenericResponse<VMDashBoard> gResponse = new GenericResponse<VMDashBoard>();
            try
            {
                // Inicializa un modelo de vista para el dashboard
                VMDashBoard vmDashBoard = new VMDashBoard();

                // Obtiene información del dashboard utilizando el servicio correspondiente
                vmDashBoard.TotalVentas = await _dashboardService.TotalVentasUltimaSemana();
                vmDashBoard.TotalIngresos = await _dashboardService.TotalIngresosUltimaSemana();
                vmDashBoard.TotalProductos = await _dashboardService.TotalProductos();
                vmDashBoard.TotalCategorias = await _dashboardService.TotalCategorias();

                // Inicializa listas para las ventas y productos de la última semana
                List<VMVentasSemana> listaVentaSemana = new List<VMVentasSemana>();
                List<VMProductosSemana> listaProductosSemana = new List<VMProductosSemana>();

                // Obtiene y mapea las ventas de la última semana
                foreach (KeyValuePair<string, int> item in await _dashboardService.VentasUltimaSemana())
                {
                    listaVentaSemana.Add(new VMVentasSemana()
                    {
                        Fecha = item.Key,
                        Total = item.Value
                    });
                };
                // Obtiene y mapea los productos más vendidos de la última semana
                foreach (KeyValuePair<string, int> item in await _dashboardService.ProductosTopUltimaSemana())
                {
                    listaProductosSemana.Add(new VMProductosSemana()
                    {
                        Producto= item.Key,
                        Cantidad = item.Value
                    });
                }
                // Asigna las listas al modelo de vista
                vmDashBoard.VentasUltimaSemana = listaVentaSemana;
                vmDashBoard.ProductosTopUltimaSemana = listaProductosSemana;

                // Configura el objeto GenericResponse con el resultado exitoso y el modelo de vista
                gResponse.Estado = true;
                gResponse.Objeto = vmDashBoard;


            } catch (Exception ex)
            {
                // En caso de error, registra el error y configura el objeto GenericResponse con el resultado fallido y el mensaje de error
                Console.WriteLine(ex.ToString());
                gResponse.Estado= false;
                gResponse.Mensaje = ex.Message;
            }
            // Retorna una respuesta HTTP 200 con el objeto GenericResponse que contiene el resumen del dashboard
            return StatusCode(StatusCodes.Status200OK, gResponse);
        }
    }
}
