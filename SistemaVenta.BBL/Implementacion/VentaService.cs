using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using SistemaVenta.BBL.Interfaces;
using SistemaVenta.DAL.Interfaces;
using SistemaVenta.Entity;

namespace SistemaVenta.BBL.Implementacion
{
    /// <summary>
    /// Clase que gestiona las operaciones relacionadas con las ventas de productos en el sistema.
    /// </summary>
    public class VentaService : IVentaService
    {
        private readonly IGenericRepository<Producto> _repositoryProducto;
        private readonly IVentaRepository _repositoryVenta;

        /// <summary>
        /// Constructor de la clase VentaService.
        /// </summary>
        /// <param name="repository">Repositorio genérico para productos.</param>
        /// <param name="ventaRepository">Repositorio específico para ventas.</param>
        public VentaService(IGenericRepository<Producto> repository, IVentaRepository ventaRepository)
        {
            _repositoryProducto = repository;
            _repositoryVenta = ventaRepository;
        }

        /// <summary>
        /// Obtiene una lista de productos que coinciden con una cadena de búsqueda.
        /// </summary>
        /// <param name="busqueda">Cadena de búsqueda para filtrar los productos.</param>
        /// <returns>Lista de productos que coinciden con la búsqueda.</returns>
        public async Task<List<Producto>> ObtenerProductos(string busqueda)
        {
            IQueryable<Producto> query = await _repositoryProducto.Consultar(
                prod => prod.EsActivo == true &&
                        prod.Stock > 0 &&
                        string.Concat(prod.CodigoBarra, prod.Marca, prod.Descripcion).Contains(busqueda)
                );
            return query.Include(c => c.IdCategoriaNavigation).ToList();
        }

        /// <summary>
        /// Registra una nueva venta en el sistema.
        /// </summary>
        /// <param name="entidad">Entidad de venta a ser registrada.</param>
        /// <returns>La venta registrada.</returns>
        public async Task<Venta> Registrar(Venta entidad)
        {
            try
            {
                return await _repositoryVenta.Registrar(entidad);
            } catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());   
                throw;
            }
        }

        // <summary>
        /// Obtiene un historial de ventas basado en criterios como el número de venta, la fecha de inicio y la fecha de fin.
        /// </summary>
        /// <param name="numeroVenta">Número de venta a buscar.</param>
        /// <param name="fechaInicio">Fecha de inicio del rango.</param>
        /// <param name="fechaFin">Fecha de fin del rango.</param>
        /// <returns>Lista de ventas que cumplen con los criterios especificados.</returns>
        public async Task<List<Venta>> Historial(string numeroVenta, string fechaInicio, string fechaFin)
        {
            IQueryable<Venta> query = await _repositoryVenta.Consultar();

            fechaInicio = fechaInicio is null ? "" : fechaInicio;
            fechaFin = fechaFin is null ? "" : fechaFin;

            if(fechaInicio != "" && fechaFin != "")
            {
                DateTime f_inicio = DateTime.ParseExact(fechaInicio, "dd/MM/yyyy", new CultureInfo("es-ES"));
                DateTime f_fin = DateTime.ParseExact(fechaFin, "dd/MM/yyyy", new CultureInfo("es-ES"));

                return query.Where(v =>
                    v.FechaRegistro.Value.Date >= f_inicio.Date &&
                    v.FechaRegistro.Value.Date <= f_fin.Date
                )
                    .Include(tdv => tdv.IdTipoDocumentoVentaNavigation)
                    .Include(usu => usu.IdUsuarioNavigation)
                    .Include(dv => dv.DetalleVenta)
                    .ToList();
            }
            else
            {
                return query.Where(v =>
                    v.NumeroVenta == numeroVenta
                )
                    .Include(tdv => tdv.IdTipoDocumentoVentaNavigation)
                    .Include(usu => usu.IdUsuarioNavigation)
                    .Include(dv => dv.DetalleVenta)
                    .ToList();
            }

        }

        /// <summary>
        /// Obtiene los detalles de una venta específica según su número de venta.
        /// </summary>
        /// <param name="numeroVenta">Número de venta a consultar.</param>
        /// <returns>Detalles de la venta, incluyendo información sobre el tipo de documento, el usuario y los productos vendidos.</returns>
        public async Task<Venta> Detalle(string numeroVenta)
        {
            IQueryable<Venta> query = await _repositoryVenta.Consultar(v => v.NumeroVenta == numeroVenta);
            return query.Include(tdv => tdv.IdTipoDocumentoVentaNavigation)
                    .Include(usu => usu.IdUsuarioNavigation)
                    .Include(dv => dv.DetalleVenta)
                    .First();
        }

        /// <summary>
        /// Genera un informe de ventas para un rango de fechas dado.
        /// </summary>
        /// <param name="fechaInicio">Fecha de inicio del rango.</param>
        /// <param name="fechaFin">Fecha de fin del rango.</param>
        /// <returns>Lista de detalles de ventas para el período especificado.</returns>
        public async Task<List<DetalleVenta>> Reporte(string fechaInicio, string fechaFin)
        {
            DateTime f_inicio = DateTime.ParseExact(fechaInicio, "dd/MM/yyyy", new CultureInfo("es-ES"));
            DateTime f_fin = DateTime.ParseExact(fechaFin, "dd/MM/yyyy", new CultureInfo("es-ES"));

            List<DetalleVenta> lista = await _repositoryVenta.Reporte(f_inicio, f_fin);
            return lista.ToList();
        }
    }
}
