using Microsoft.AspNetCore.Mvc;

using AutoMapper;
using Newtonsoft.Json;

using SistemaVenta.AplicacionWeb.Models.ViewModels;
using SistemaVenta.AplicacionWeb.Utilidades.Response;
using SistemaVenta.BBL.Interfaces;
using SistemaVenta.Entity;
using System.Drawing;
using Microsoft.AspNetCore.Authorization;

namespace SistemaVenta.AplicacionWeb.Controllers
{
    [Authorize]
    public class ProductoController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IProductoService _productoService;

        /// <summary>
        /// Constructor que inicializa una nueva instancia del controlador de productos.
        /// </summary>
        public ProductoController(IMapper mapper, IProductoService productoService)
        {
            _mapper = mapper;
            _productoService = productoService;
        }

        /// <summary>
        /// Acción que devuelve la vista principal de productos.
        /// </summary>
        /// <returns>Vista principal de productos.</returns>
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Acción HTTP GET que obtiene la lista de productos y devuelve una respuesta HTTP 200 con los datos.
        /// </summary>
        /// <returns>Respuesta HTTP 200 con un objeto anónimo que contiene la lista de productos.</returns>
        [HttpGet]
        public async Task<IActionResult> Lista() {
            List<VMProducto> vmProductoLista = _mapper.Map<List<VMProducto>>(await _productoService.Lista());
            return StatusCode(StatusCodes.Status200OK, new {data = vmProductoLista});
        }


        /// <summary>
        /// Acción HTTP POST que crea un nuevo producto y devuelve una respuesta HTTP 200 con el resultado.
        /// </summary>
        /// <param name="imagen">Archivo de imagen del producto.</param>
        /// <param name="modelo">Datos del producto en formato JSON.</param>
        /// <returns>Respuesta HTTP 200 con un objeto GenericResponse que indica el resultado de la operación.</returns>

        [HttpPost]
        public async Task<IActionResult> Crear([FromForm]IFormFile imagen, [FromForm]string modelo)
        {
            GenericResponse<VMProducto> genericResponse = new GenericResponse<VMProducto>();
            try
            {
                VMProducto producto = JsonConvert.DeserializeObject<VMProducto>(modelo);
                string nombreImagen = "";
                Stream imagenStream = null;

                if(imagen != null) { 
                    string nombreCodificado = Guid.NewGuid().ToString("N");
                    string extension = Path.GetExtension(imagen.FileName);
                    nombreImagen = string.Concat(nombreCodificado, extension);
                    imagenStream = imagen.OpenReadStream();
                }
                Producto productoCreado = await _productoService.Crear(_mapper.Map<Producto>(producto),imagenStream, nombreImagen);
                producto = _mapper.Map<VMProducto>(productoCreado);

                genericResponse.Estado = true;
                genericResponse.Objeto = producto;

            } catch (Exception ex)
            {
                genericResponse.Estado=false;
                genericResponse.Mensaje = ex.Message;

                Console.WriteLine(ex.ToString());
            }
            return StatusCode(StatusCodes.Status200OK, genericResponse);
        }

        /// <summary>
        /// Acción HTTP PUT que edita un producto existente y devuelve una respuesta HTTP 200 con el resultado.
        /// </summary>
        /// <param name="imagen">Archivo de imagen del producto.</param>
        /// <param name="modelo">Datos del producto en formato JSON.</param>
        /// <returns>Respuesta HTTP 200 con un objeto GenericResponse que indica el resultado de la operación.</returns>
        [HttpPut]
        public async Task<IActionResult> Editar([FromForm] IFormFile imagen, [FromForm] string modelo)
        {
            GenericResponse<VMProducto> genericResponse = new GenericResponse<VMProducto>();
            try
            {
                VMProducto producto = JsonConvert.DeserializeObject<VMProducto>(modelo);
                string nombreImagen = "";
                Stream imagenStream = null;

                if (imagen != null)
                {
                    string nombreCodificado = Guid.NewGuid().ToString("N");
                    string extension = Path.GetExtension(imagen.FileName);
                    nombreImagen = string.Concat(nombreCodificado, extension);
                    Console.WriteLine(nombreImagen);    
                    imagenStream = imagen.OpenReadStream();
                }
                Producto productoEditado = await _productoService.Editar(_mapper.Map<Producto>(producto), imagenStream, nombreImagen);
                producto = _mapper.Map<VMProducto>(productoEditado);

                genericResponse.Estado = true;
                genericResponse.Objeto = producto;

            }
            catch (Exception ex)
            {
                genericResponse.Estado = false;
                genericResponse.Mensaje = ex.Message;

                Console.WriteLine(ex.ToString());
            }
            return StatusCode(StatusCodes.Status200OK, genericResponse);
        }

        /// <summary>
        /// Acción HTTP DELETE que elimina un producto por su ID y devuelve una respuesta HTTP 200 con el resultado.
        /// </summary>
        /// <param name="idProducto">ID del producto a eliminar.</param>
        /// <returns>Respuesta HTTP 200 con un objeto GenericResponse que indica el resultado de la operación.</returns>
        [HttpDelete]
        public async Task<IActionResult> Eliminar(int idProducto)
        {
            GenericResponse<string> genericResponse = new GenericResponse<string>();
            try
            {
                genericResponse.Estado = await _productoService.Eliminar(idProducto);
            }
            catch (Exception ex)
            {
                genericResponse.Estado = false;
                genericResponse.Mensaje = ex.Message;

                Console.WriteLine(ex.ToString());
            }
            return StatusCode(StatusCodes.Status200OK, genericResponse);

        }
    }
}
