using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVenta.BBL.Interfaces
{
    /// <summary>
    /// Interfaz que define operaciones relacionadas con el servicio de almacenamiento Firebase.
    /// </summary>
    public interface IFireBaseService
    {
        /// <summary>
        /// Sube un archivo al sistema de almacenamiento Firebase.
        /// </summary>
        /// <param name="streamArchivo">Flujo de datos del archivo a subir.</param>
        /// <param name="carpetaDestino">Carpeta de destino en Firebase.</param>
        /// <param name="nombreArchivo">Nombre del archivo en Firebase.</param>
        /// <returns>URL de descarga del archivo almacenado.</returns>
        Task<string> SubirStorage(Stream streamArchivo, string carpetaDestino, string nombreArchivo);

        /// <summary>
        /// Elimina un archivo del sistema de almacenamiento Firebase.
        /// </summary>
        /// <param name="carpetaDestino">Carpeta de destino en Firebase.</param>
        /// <param name="nombreArchivo">Nombre del archivo en Firebase.</param>
        /// <returns>True si la eliminación fue exitosa, de lo contrario, False.</returns>
        Task<bool> EliminarStorage(string carpetaDestino, string nombreArchivo);
    }
}
