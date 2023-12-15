using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SistemaVenta.Entity;


namespace SistemaVenta.BBL.Interfaces
{
    /// <summary>
    /// Interfaz que define operaciones para la gestión de la entidad de negocio en una aplicación .NET.
    /// </summary>
    public interface INegocioService
    {
        /// <summary>
        /// Obtiene información sobre el negocio.
        /// </summary>
        /// <returns>Una tarea que, al completarse, proporciona la entidad de negocio.</returns>
        Task<Negocio> Obtener();

        /// <summary>
        /// Guarda cambios en la entidad de negocio, permitiendo opcionalmente asociar un logotipo.
        /// </summary>
        /// <param name="entidad">La entidad de negocio a actualizar o crear.</param>
        /// <param name="Logo">Flujo de datos del logotipo (opcional).</param>
        /// <param name="nombreLogo">Nombre del logotipo (opcional).</param>
        /// <returns>Una tarea que, al completarse, devuelve la entidad de negocio actualizada o creada.</returns>
        Task<Negocio> GuardarCambios(Negocio entidad, Stream Logo = null, string nombreLogo="");
    }
}
