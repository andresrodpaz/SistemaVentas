using SistemaVenta.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVenta.BBL.Interfaces
{
    /// <summary>
    /// Interfaz que define operaciones relacionadas con la gestión de usuarios en la aplicación.
    /// </summary>
    public interface IUsuarioService
    {
        /// <summary>
        /// Obtiene de manera asincrónica una lista de usuarios registrados en la aplicación.
        /// </summary>
        Task<List<Usuario>> Lista();

        /// <summary>
        /// Crea un nuevo usuario de manera asincrónica con la posibilidad de adjuntar una foto de perfil y una plantilla de correo.
        /// </summary>
        /// <param name="usuario">Datos del usuario a crear.</param>
        /// <param name="fotoUsuario">Imagen de perfil del usuario.</param>
        /// <param name="nombreFoto">Nombre del archivo de la imagen de perfil.</param>
        /// <param name="UrlPlantillaCorreo">URL de la plantilla de correo.</param>
        /// <returns>Una tarea que representa la operación de creación de usuario.</returns>
        Task<Usuario> CrearUsuaio(Usuario usuario, Stream fotoUsuario = null, string nombreFoto ="", string UrlPlantillaCorreo="");

        /// <summary>
        /// Edita de manera asincrónica los datos de un usuario existente, permitiendo la actualización de la foto de perfil.
        /// </summary>
        /// <param name="usuario">Datos del usuario a editar.</param>
        /// <param name="fotoUsuario">Nueva imagen de perfil del usuario.</param>
        /// <param name="nombreFoto">Nombre del archivo de la nueva imagen de perfil.</param>
        /// <returns>Una tarea que representa la operación de edición de usuario.</returns>
        Task<Usuario> EditarUsuario(Usuario usuario, Stream fotoUsuario = null, string nombreFoto = "");

        /// <summary>
        /// Elimina de manera asincrónica un usuario existente por su identificador.
        /// </summary>
        /// <param name="id">Identificador único del usuario a eliminar.</param>
        /// <returns>Una tarea que representa la operación de eliminación de usuario.</returns>
        Task<bool> DeleteUsuario(int id);

        /// <summary>
        /// Obtiene de manera asincrónica un usuario por sus credenciales (correo y clave).
        /// </summary>
        /// <param name="email">Correo electrónico del usuario.</param>
        /// <param name="clave">Clave del usuario.</param>
        /// <returns>Una tarea que representa la operación de obtener un usuario por sus credenciales.</returns>
        Task<Usuario> ObtenerPorCredenciales(string email, string clave);

        /// <summary>
        /// Obtiene de manera asincrónica un usuario por su identificador único.
        /// </summary>
        /// <param name="id">Identificador único del usuario.</param>
        /// <returns>Una tarea que representa la operación de obtener un usuario por su identificador.</returns>
        Task<Usuario> ObtenerPorId(int id);

        /// <summary>
        /// Guarda de manera asincrónica los cambios realizados en el perfil de un usuario.
        /// </summary>
        /// <param name="usuario">Datos del usuario con los cambios en el perfil.</param>
        /// <returns>Una tarea que representa la operación de guardar el perfil del usuario.</returns>
        Task<bool> GuardarPerfil(Usuario usuario);

        /// <summary>
        /// Cambia de manera asincrónica la clave de un usuario específico.
        /// </summary>
        /// <param name="idUsuario">Identificador único del usuario.</param>
        /// <param name="claveAntigua">Clave antigua del usuario.</param>
        /// <param name="claveNueva">Nueva clave del usuario.</param>
        /// <returns>Una tarea que representa la operación de cambiar la clave del usuario.</returns>
        Task<bool> CambiarClave(int idUsuario, string claveAntigua, string claveNueva);

        /// <summary>
        /// Restablece de manera asincrónica la clave de un usuario utilizando su correo electrónico y una plantilla de correo.
        /// </summary>
        /// <param name="correo">Correo electrónico del usuario.</param>
        /// <param name="plantillaCorreo">Plantilla de correo para restablecer la clave.</param>
        /// <returns>Una tarea que representa la operación de restablecer la clave del usuario.</returns>
        Task<bool> RestablecerClave(string correo, string plantillaCorreo);
    }
}
