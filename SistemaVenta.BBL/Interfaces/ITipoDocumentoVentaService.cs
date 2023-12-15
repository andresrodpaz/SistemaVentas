using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SistemaVenta.Entity;

namespace SistemaVenta.BBL.Interfaces
{
    /// <summary>
    /// Interfaz que define operaciones relacionadas con la gestión de tipos de documentos de venta en la aplicación.
    /// </summary>
    public interface ITipoDocumentoVentaService
    {
        /// <summary>
        /// Obtiene de manera asincrónica una lista de tipos de documentos de venta disponibles en la aplicación.
        /// </summary>
        /// <returns>Una tarea que representa la operación de obtener la lista de tipos de documentos de venta.</returns>
        Task<List<TipoDocumentoVenta>> Lista();
    }
}
