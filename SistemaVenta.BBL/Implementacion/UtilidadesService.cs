using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SistemaVenta.BBL.Interfaces;
using System.Security.Cryptography;

namespace SistemaVenta.BBL.Implementacion
{
    /// <summary>
    /// Clase que proporciona servicios utilitarios en el sistema.
    /// </summary>
    public class UtilidadesService : IUtilidadesService
    {
        /// <summary>
        /// Genera una clave aleatoria.
        /// </summary>
        /// <returns>Clave generada de 6 digitos sin caracteres especiales.</returns>
        public string GenerarClave()
        {
            //Guid de 6 digitos sin caracteres especiales
            string clave = Guid.NewGuid().ToString("N").Substring(0,6);
            return clave;
        }

        /// <summary>
        /// Convierte y encripta una contraseña utilizando el algoritmo SHA256.
        /// </summary>
        /// <param name="texto">Texto a convertir/encriptar.</param>
        /// <returns>Clave encriptada.</returns>
        public string ConvertirClave(string texto)
        {
            StringBuilder sb = new StringBuilder();
            //Convertir/encriptar contraseña a SHA256
            using(SHA256 sha = SHA256Managed.Create())
            {
                Encoding enc = Encoding.UTF8;

                byte[] result = sha.ComputeHash(enc.GetBytes(texto));
                foreach (byte item in result)
                {
                    sb.Append(item.ToString("X2"));
                }
            }

            return sb.ToString();
        }
    }
}
