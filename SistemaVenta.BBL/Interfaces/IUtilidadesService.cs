using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVenta.BBL.Interfaces
{
    /// <summary>
    /// Interfaz que define servicios utilitarios esenciales en una aplicación.
    /// </summary>
    public interface IUtilidadesService
    {
        /// <summary>
        /// Genera una clave aleatoria.
        /// </summary>
        /// <returns>Clave aleatoria generada.</returns>
        string GenerarClave();

        /// <summary>
        /// Convierte y procesa una clave para operaciones de hash y encriptación.
        /// </summary>
        /// <param name="texto">Texto a convertir.</param>
        /// <returns>Clave procesada.</returns>
        string ConvertirClave(string texto);
    }
}
