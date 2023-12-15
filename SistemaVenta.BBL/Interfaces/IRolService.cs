using SistemaVenta.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVenta.BBL.Interfaces
{
    /// <summary>
    /// Interfaz que define operaciones relacionadas con la gestión de roles en la aplicación.
    /// </summary>
    public interface IRolService
    {
        /// <summary>
        /// Obtiene de manera asincrónica una lista de roles disponibles en la aplicación.
        /// </summary>
        /// <returns>Una tarea que representa la operación de obtener la lista de roles.</returns>
        Task<List<Rol>> Lista();

    }
}
