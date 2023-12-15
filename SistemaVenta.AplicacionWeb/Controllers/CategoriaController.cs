using Microsoft.AspNetCore.Mvc;

using AutoMapper;
using SistemaVenta.AplicacionWeb.Models.ViewModels;
using SistemaVenta.AplicacionWeb.Utilidades.Response;
using SistemaVenta.BBL.Interfaces;
using SistemaVenta.Entity;
using Microsoft.AspNetCore.Authorization;

namespace SistemaVenta.AplicacionWeb.Controllers
{
    [Authorize]
    public class CategoriaController : Controller
    {
        private readonly IMapper _mapper;
        private readonly ICategoriaService _categoriaService;

        /// <summary>
        /// Constructor que inicializa una nueva instancia del controlador de categorías.
        /// </summary>
        /// <param name="mapper">Instancia de IMapper para realizar mapeo entre modelos de vista y entidades.</param>
        /// <param name="categoriaService">Instancia de ICategoriaService para realizar operaciones relacionadas con categorías.</param>
        public CategoriaController(IMapper mapper, ICategoriaService categoriaService)
        {
            _categoriaService = categoriaService;
            _mapper = mapper;
        }

        /// <summary>
        /// Acción que devuelve la vista principal de categorías.
        /// </summary>
        /// <returns>Vista principal de categorías.</returns>
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Acción HTTP GET que obtiene la lista de categorías y la devuelve como respuesta HTTP 200.
        /// </summary>
        /// <returns>Respuesta HTTP 200 con la lista de categorías en formato DataTable.</returns>
        [HttpGet]
        public async Task<IActionResult> Lista()
        {
            List<VMCategoria> vmCategoriaLista = _mapper.Map<List<VMCategoria>>(await _categoriaService.Lista());
            return StatusCode(StatusCodes.Status200OK, new {data = vmCategoriaLista}); //Por DataTable
        }

        /// <summary>
        /// Acción HTTP POST que recibe un modelo de vista de categoría, intenta crearla y devuelve una respuesta HTTP 200 con el resultado.
        /// </summary>
        /// <param name="modelo">Modelo de vista de categoría para la creación.</param>
        /// <returns>Respuesta HTTP 200 con el resultado de la operación de creación.</returns>
        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] VMCategoria modelo)
        {
            GenericResponse<VMCategoria> genericResponse = new GenericResponse<VMCategoria>();

            try
            {
                Categoria categoriaCreada =await _categoriaService.Crear(_mapper.Map<Categoria>(modelo));
                modelo = _mapper.Map<VMCategoria>(categoriaCreada);

                genericResponse.Estado = true;
                genericResponse.Objeto = modelo;
            }
            catch (Exception ex)
            {
                genericResponse.Estado= false;
                genericResponse.Mensaje = ex.Message;
                Console.WriteLine(ex.ToString()); 
                throw;
            }
            return StatusCode(StatusCodes.Status200OK, genericResponse);
        }

        /// <summary>
        /// Acción HTTP PUT que recibe un modelo de vista de categoría, intenta editarla y devuelve una respuesta HTTP 200 con el resultado.
        /// </summary>
        /// <param name="modelo">Modelo de vista de categoría para la edición.</param>
        /// <returns>Respuesta HTTP 200 con el resultado de la operación de edición.</returns>
        [HttpPut]
        public async Task<IActionResult> Editar([FromBody] VMCategoria modelo)
        {
            GenericResponse<VMCategoria> genericResponse = new GenericResponse<VMCategoria>();
            try
            {
                Categoria categoriaEditar = await _categoriaService.Editar(_mapper.Map<Categoria>(modelo));
                modelo = _mapper.Map<VMCategoria>(categoriaEditar);

                genericResponse.Estado = true;
                genericResponse.Objeto = modelo;
            }
            catch (Exception ex) {
                genericResponse.Estado = false;
                genericResponse.Mensaje = ex.Message;
                Console.WriteLine(ex.ToString());
                throw;
            }
            return StatusCode(StatusCodes.Status200OK, genericResponse);
        }

        /// <summary>
        /// Acción HTTP DELETE que recibe el identificador de una categoría, intenta eliminarla y devuelve una respuesta HTTP 200 con el resultado.
        /// </summary>
        /// <param name="idCategoria">Identificador de la categoría a eliminar.</param>
        /// <returns>Respuesta HTTP 200 con el resultado de la operación de eliminación.</returns>
        [HttpDelete]
        public async Task<IActionResult> Eliminar(int idCategoria)
        {
            GenericResponse<string> genericResponse = new GenericResponse<string>();
            try
            {
                genericResponse.Estado = await _categoriaService.Elminar(idCategoria);
            }  catch (Exception ex) {
                genericResponse.Estado = false;
                genericResponse.Mensaje = ex.Message;
                Console.WriteLine(ex.ToString());
                throw;
            }
            return StatusCode(StatusCodes.Status200OK, genericResponse);
        }
        
    }
}
