using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SistemaVenta.Entity;

namespace SistemaVenta.DAL.Interfaces
{
    /// <summary>
    /// Interfaz que extiende las operaciones genéricas de acceso a datos para la entidad Venta y define operaciones específicas relacionadas con ventas.
    /// </summary>
    public interface IVentaRepository:IGenericRepository<Venta>
    {
        /// <summary>
        /// Registra una nueva venta en la fuente de datos.
        /// </summary>
        /// <param name="entidad">La venta que se va a registrar.</param>
        /// <returns>Una tarea que representa la operación y devuelve la venta registrada.</returns>
        Task<Venta> Registrar(Venta entidad);

        /// <summary>
        /// Genera un informe de ventas para el rango de fechas especificado.
        /// </summary>
        /// <param name="fechaInicio">Fecha de inicio del rango.</param>
        /// <param name="fechaFin">Fecha de fin del rango.</param>
        /// <returns>Una tarea que representa la operación y devuelve una lista de detalles de venta para el informe.</returns>
        Task<List<DetalleVenta>> Reporte(DateTime fechaInicio, DateTime fechaFin);
    }
}
