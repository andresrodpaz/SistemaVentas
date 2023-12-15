using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVenta.BBL.Interfaces
{
    /// <summary>
    /// Interfaz que define operaciones relacionadas con el envío de correos.
    /// </summary>
    public interface ICorreoService
    {
        // <summary>
        /// Envía un correo electrónico.
        /// </summary>
        /// <param name="CorreoDestino">La dirección de correo electrónico de destino.</param>
        /// <param name="Asunto">El asunto del correo electrónico.</param>
        /// <param name="Mensaje">El contenido del correo electrónico.</param>
        /// <returns>Una tarea que representa la operación asíncrona y devuelve true si el correo se envió correctamente, false en caso contrario.</returns>
        Task<bool> enviarCorreo(string CorreoDestino, string Asunto, string Mensaje);
    }
}
