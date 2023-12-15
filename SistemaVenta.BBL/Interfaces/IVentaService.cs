using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SistemaVenta.Entity;

namespace SistemaVenta.BBL.Interfaces
{
    /// <summary>
    /// Interfaz que define servicios relacionados con operaciones de ventas en el sistema.
    /// </summary>
    public interface IVentaService
    {
        /// <summary>
        /// Obtiene una lista de productos que coinciden con la cadena de búsqueda.
        /// </summary>
        /// <param name="busqueda">Cadena de búsqueda para filtrar productos.</param>
        /// <returns>Lista de productos filtrados.</returns>
        Task<List<Producto>> ObtenerProductos(string busqueda);

        /// <summary>
        /// Registra una nueva venta en el sistema.
        /// </summary>
        /// <param name="entidad">Información de la venta a registrar.</param>
        /// <returns>Información de la venta registrada.</returns>
        Task<Venta> Registrar(Venta entidad);

        /// <summary>
        /// Obtiene el historial de ventas en un rango de fechas y/o número de venta.
        /// </summary>
        /// <param name="numeroVenta">Número de venta a filtrar (opcional).</param>
        /// <param name="fechaInicio">Fecha de inicio del rango (opcional).</param>
        /// <param name="fechaFin">Fecha de fin del rango (opcional).</param>
        /// <returns>Lista de ventas que cumplen con los criterios de búsqueda.</returns>
        Task<List<Venta>> Historial(string numeroVenta, string fechaInicio, string fechaFin);

        /// <summary>
        /// Obtiene los detalles de una venta específica.
        /// </summary>
        /// <param name="numeroVenta">Número de venta a consultar.</param>
        /// <returns>Información detallada de la venta.</returns>
        Task<Venta> Detalle(string numeroVenta);

        /// <summary>
        /// Genera un reporte de ventas en un rango de fechas.
        /// </summary>
        /// <param name="fechaInicio">Fecha de inicio del rango.</param>
        /// <param name="fechaFin">Fecha de fin del rango.</param>
        /// <returns>Lista de detalles de ventas para el reporte.</returns>
        Task<List<DetalleVenta>> Reporte(string fechaInicio, string fechaFin);

    }
}
