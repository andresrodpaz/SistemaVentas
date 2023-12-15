using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using SistemaVenta.BBL.Interfaces;
using SistemaVenta.DAL.Interfaces;
using SistemaVenta.Entity;
using System.Globalization;

namespace SistemaVenta.BBL.Implementacion
{
    /// <summary>
    /// Implementación del servicio de tablero de control (DashBoard) que proporciona estadísticas relacionadas con las ventas, productos y categorías.
    /// </summary>
    public class DashBoardService : IDashBoardService
    {
        private readonly IVentaRepository _ventaRepository;
        private readonly IGenericRepository<DetalleVenta> _detalleRepository;
        private readonly IGenericRepository<Categoria> _categoriaRepository;
        private readonly IGenericRepository<Producto> _productoRepository;
        private DateTime FechaInicio = DateTime.Now;

        /// <summary>
        /// Constructor de la clase DashBoardService.
        /// </summary>
        /// <param name="ventaRepository">Repositorio de ventas para acceder a datos relacionados con las ventas.</param>
        /// <param name="detalleRepository">Repositorio genérico para acceder a detalles de ventas.</param>
        /// <param name="categoriaRepository">Repositorio genérico para acceder a datos relacionados con las categorías.</param>
        public DashBoardService(IVentaRepository ventaRepository, IGenericRepository<DetalleVenta> detalleRepository, IGenericRepository<Categoria> categoriaRepository, IGenericRepository<Producto> productoRepository)
        {
            _ventaRepository = ventaRepository;
            _detalleRepository = detalleRepository;
            _categoriaRepository = categoriaRepository;
            _productoRepository = productoRepository;

            FechaInicio = FechaInicio.AddDays(-7);
        }

        /// <summary>
        /// Obtiene el total de ventas registradas en la última semana.
        /// </summary>
        /// <returns>Una tarea que representa la operación asíncrona y devuelve el total de ventas.</returns>
        public async Task<int> TotalVentasUltimaSemana()
        {
            try
            {
                IQueryable<Venta> query = await _ventaRepository.Consultar(venta => venta.FechaRegistro.Value.Date >= FechaInicio.Date); 
                int total = query.Count();
                return total;
            } catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw;
            }
        }
        /// <summary>
        /// Obtiene el total de ingresos generados por las ventas registradas en la última semana.
        /// </summary>
        /// <returns>Una tarea que representa la operación asíncrona y devuelve el total de ingresos en formato de texto.</returns>
        public async Task<string> TotalIngresosUltimaSemana()
        {
            try
            {
                IQueryable<Venta> query = await _ventaRepository.Consultar(venta => venta.FechaRegistro.Value.Date >= FechaInicio.Date);
                decimal resultado = query.Select(venta => venta.Total).Sum(v => v.Value);
                return Convert.ToString(resultado,new CultureInfo("es-ES"));
            } catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw;
            }
        }
        /// <summary>
        /// Obtiene el total de productos disponibles en la base de datos.
        /// </summary>
        /// <returns>Una tarea que representa la operación asíncrona y devuelve el total de productos.</returns>
        public async Task<int> TotalProductos()
        {
            try
            {
                IQueryable<Producto> query = await _productoRepository.Consultar();
                int total = query.Count();
                return total;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw;
            }
        }
        /// <summary>
        /// Obtiene el total de categorías existentes en la base de datos.
        /// </summary>
        /// <returns>Una tarea que representa la operación asíncrona y devuelve el total de categorías.</returns>
        public async  Task<int> TotalCategorias()
        {
            try
            {
                IQueryable<Categoria> query = await _categoriaRepository.Consultar();
                int total = query.Count();
                return total;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw;
            }
        }
        /// <summary>
        /// Obtiene un diccionario que muestra la cantidad de ventas registradas para cada día de la última semana.
        /// </summary>
        /// <returns>Una tarea que representa la operación asíncrona y devuelve el diccionario de ventas por día.</returns>
        public async Task<Dictionary<string, int>> VentasUltimaSemana()
        {
            try
            {
                //Obtener todas las ventas cuya fecha de registro esté dentro de la última semana utilizando el repositorio de ventas
                IQueryable<Venta> query = await _ventaRepository.Consultar(venta => venta.FechaRegistro.Value.Date >= FechaInicio.Date);
                Dictionary<string, int> result = query
                    .GroupBy(v => v.FechaRegistro.Value.Date) // Agrupar las ventas por la fecha de registro
                    .OrderByDescending(g => g.Key) //Ordenar los grupos en orden descendente según la fecha de registro
                    .Select(dv => new {fecha = dv.Key.ToString("dd/MM/yyyy"), total = dv.Count() }) // Seleccionar una proyección anónima que incluye la fecha (formateada como "dd/MM/yyyy") y la cantidad total de ventas
                    .ToDictionary(keySelector: r => r.fecha, elementSelector: r => r.total); //Convertir el resultado en un diccionario

                return result;

            } catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw;
            }
        }

        /// <summary>
        /// Obtiene un diccionario que muestra los productos más vendidos durante la última semana, limitando los resultados a los cuatro productos más populares.
        /// </summary>
        /// <returns>Una tarea que representa la operación asíncrona y devuelve el diccionario de productos más vendidos.</returns>
        public async Task<Dictionary<string, int>> ProductosTopUltimaSemana()
        {
            try
            {
                // Consulta Base: Obtener todos los detalles de venta utilizando el repositorio genérico
                IQueryable<DetalleVenta> query = await _detalleRepository.Consultar();
                Dictionary<string, int> result = query
                    .Include(v => v.IdVentaNavigation) //Cargar los datos relacionados de la venta asociada a cada detalle de venta
                    .Where(dv => dv.IdVentaNavigation.FechaRegistro.Value.Date >= FechaInicio.Date) // Obtener solo los detalles de venta cuya fecha de registro de venta esté dentro de la última semana
                    .GroupBy(dv => dv.DescripcionProducto) // Agrupar los detalles de venta por la descripción del producto
                    .OrderByDescending(g => g.Count())// Ordenar los grupos en orden descendente según la cantidad de ventas de cada producto
                    .Select(dv => new { producto = dv.Key, total = dv.Count()}) //Seleccionar una proyección anónima que incluye el nombre del producto y la cantidad total de ventas
                    .Take(4)//Limitar los resultados a los cuatro productos más populares
                    .ToDictionary(keySelector: r => r.producto, elementSelector: r => r.total); //Convertir el resultado en un diccionario

                return result;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw;
            }
        }
        
    }
}
