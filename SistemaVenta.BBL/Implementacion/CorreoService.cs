using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//Imports necesarios para el envio de email
using System.Net;
using System.Net.Mail;


using SistemaVenta.BBL.Interfaces;
using SistemaVenta.DAL.Interfaces;
using SistemaVenta.Entity;

namespace SistemaVenta.BBL.Implementacion
{
    /// <summary>
    /// Implementación de servicio para el envío de correos electrónicos.
    /// </summary>
    public class CorreoService : ICorreoService
    {
        private readonly IGenericRepository<Configuracion> _repositorio;

        /// <summary>
        /// Constructor de la clase CorreoService.
        /// </summary>
        /// <param name="repositoriy">Instancia del repositorio de configuración para acceder a la información necesaria.</param>
        public CorreoService(IGenericRepository<Configuracion> repository)
        {
            _repositorio = repository;
        }

        /// <summary>
        /// Envía un correo electrónico utilizando un servicio de correo configurado.
        /// </summary>
        /// <param name="CorreoDestino">La dirección de correo electrónico de destino.</param>
        /// <param name="Asunto">El asunto del correo electrónico.</param>
        /// <param name="Mensaje">El cuerpo del correo electrónico en formato HTML.</param>
        /// <returns>
        /// `true` si el correo electrónico se envía con éxito, de lo contrario, `false`.
        /// </returns>
        /// <remarks>
        /// Este método utiliza la configuración almacenada en la base de datos para obtener la información necesaria del servicio de correo.
        /// Luego, construye un correo electrónico con la dirección de destino, asunto y mensaje proporcionados.
        /// Finalmente, utiliza un cliente SMTP para enviar el correo electrónico mediante el servicio de correo configurado.
        /// </remarks>
        public async Task<bool> enviarCorreo(string CorreoDestino, string Asunto, string Mensaje)
        {
            try
            {
                // Consultar configuraciones desde la base de datos --- Daba un error, por eso introduzco credenciales manualmente ---
                IQueryable<Configuracion> query = await _repositorio.Consultar(c => c.Recurso.Equals("Servicio_Correo"));
                Dictionary<string, string> config = query.ToDictionary(keySelector: c => c.Propiedad, elementSelector: c => c.Valor);

                // Configurar credenciales de acceso al servicio de correo --- Daba un error, por eso introduzco credenciales manualmente ---
                var credenciales = new NetworkCredential(config["correo"], config["clave"]);

                // Configurar el objeto de correo electrónico
                var correo = new MailMessage()
                {
                    From = new MailAddress("arodriguez.proyecto@gmail.com", "StockHub"),
                    //From = new MailAddress(config["correo"], config["alias"]),
                    Subject = Asunto,
                    Body = Mensaje,
                    IsBodyHtml = true
                };
                correo.To.Add(new MailAddress(CorreoDestino));

                // Configurar el cliente SMTP para enviar el correo electrónico
                var clienteServidor = new SmtpClient()
                {
                    Host = "smtp.gmail.com", // Servidor SMTP de Gmail
                    Port = 587, // Puerto de Gmail para TLS
                    Credentials = new NetworkCredential("arodriguez.proyecto@gmail.com", "ymknjhyctsmsryyl"),
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    EnableSsl = true,
                };

                // Enviar el correo electrónico
                clienteServidor.Send(correo);

                //Correo enviado con exiro
                return true;

            }
            catch (Exception)
            {
                return false; // Si ha habido algún fallo, retorna false
                throw;
            }
        }
    }
}
