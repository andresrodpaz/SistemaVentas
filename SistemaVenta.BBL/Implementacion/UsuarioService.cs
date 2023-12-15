using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using System.Net;

using SistemaVenta.BBL.Interfaces;
using SistemaVenta.DAL.Interfaces;
using SistemaVenta.Entity;

namespace SistemaVenta.BBL.Implementacion
{
    /// <summary>
    /// Clase que proporciona servicios relacionados con la gestión de usuarios en el sistema.
    /// </summary>
    public class UsuarioService : IUsuarioService
    {
        private readonly IGenericRepository<Usuario> _genericRepository;
        private readonly IFireBaseService _fireBaseService;
        private readonly IUtilidadesService _utilidadesService;
        private readonly ICorreoService _correoService;

        /// <summary>
        /// Constructor de la clase <see cref="UsuarioService"/>.
        /// </summary>
        /// <param name="genericRepository">Repositorio genérico para operaciones CRUD en la base de datos.</param>
        /// <param name="fireBaseService">Servicio para interactuar con Firebase Storage.</param>
        /// <param name="utilidadesService">Servicio para utilidades generales.</param>
        /// <param name="correoService">Servicio para el envío de correos electrónicos.</param>
        public UsuarioService(
            IGenericRepository<Usuario> genericRepository,
            IFireBaseService fireBaseService,
            IUtilidadesService utilidadesService,
            ICorreoService correoService
            )
        {
            _genericRepository = genericRepository;
            _fireBaseService = fireBaseService;
            _utilidadesService = utilidadesService;
            _correoService = correoService;
        }

        /// <summary>
        /// Obtiene una lista de todos los usuarios en el sistema, incluyendo información sobre el rol asociado a cada usuario.
        /// </summary>
        /// <returns>Lista de usuarios.</returns>
        public async Task<List<Usuario>> Lista()
        {
            IQueryable<Usuario> query = await _genericRepository.Consultar();
            return query.Include(rol => rol.IdRolNavigation).ToList();
        }

        /// <summary>
        /// Crea un nuevo usuario en el sistema.
        /// </summary>
        /// <param name="usuario">Usuario a crear.</param>
        /// <param name="fotoUsuario">Stream que representa la foto del usuario (opcional).</param>
        /// <param name="nombreFoto">Nombre de la foto del usuario (opcional).</param>
        /// <param name="UrlPlantillaCorreo">URL de la plantilla de correo (opcional).</param>
        /// <returns>Usuario creado.</returns>
        /// <exception cref="TaskCanceledException">Se lanza cuando ya existe un usuario registrado con el mismo correo.</exception>
        public async Task<Usuario> CrearUsuaio(Usuario usuario, Stream fotoUsuario = null, string nombreFoto = "", string UrlPlantillaCorreo = "")
        {
            Usuario usuarioExistente = await _genericRepository.Obtener(usuarioQuery => usuarioQuery.Correo == usuario.Correo);

            if (usuarioExistente != null)
            {
                throw new TaskCanceledException("Ya existe un usuario registrado con este email");
            }
            try
            {
                string claveGenerada = _utilidadesService.GenerarClave();
                usuario.Clave = _utilidadesService.ConvertirClave(claveGenerada);
                usuario.NombreFoto = nombreFoto;

                if (fotoUsuario != null)
                {
                    string urlFoto = await _fireBaseService.SubirStorage(fotoUsuario, "carpeta_usuario", nombreFoto);
                    usuario.UrlFoto = urlFoto;
                }

                Usuario usuarioCreado = await _genericRepository.Crear(usuario);
                if (usuarioCreado.IdUsuario == 0)
                {
                    throw new TaskCanceledException("No se pudo crear el usuario");
                }

                if (UrlPlantillaCorreo != "")
                {
                    UrlPlantillaCorreo = UrlPlantillaCorreo.Replace("[correo]", usuarioCreado.Correo).Replace("[clave]", claveGenerada);
                    string HTMLCorreo = "";

                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(UrlPlantillaCorreo);
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        using (Stream dataStream = response.GetResponseStream())
                        {
                            StreamReader readerStream = null;

                            if (response.CharacterSet == null)
                            {
                                readerStream = new StreamReader(dataStream);
                            }
                            else
                            {
                                readerStream = new StreamReader(dataStream, Encoding.GetEncoding(response.CharacterSet));
                            }
                            HTMLCorreo = readerStream.ReadToEnd();
                            response.Close();
                            readerStream.Close();
                        }
                    }
                    if (HTMLCorreo != "")
                    {
                        await _correoService.enviarCorreo(usuarioCreado.Correo, "Cuenta creada con exito!", HTMLCorreo);
                    }

                }
                IQueryable<Usuario> query = await _genericRepository.Consultar(user => user.IdUsuario == usuarioCreado.IdUsuario);
                usuarioCreado = query.Include(rol => rol.IdRolNavigation).First();

                return usuarioCreado;
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// Edita la información de un usuario en el sistema.
        /// </summary>
        /// <param name="usuario">Usuario con la información actualizada.</param>
        /// <param name="fotoUsuario">Stream que representa la nueva foto del usuario (opcional).</param>
        /// <param name="nombreFoto">Nuevo nombre de la foto del usuario (opcional).</param>
        /// <returns>Usuario editado.</returns>
        /// <exception cref="TaskCanceledException">Se lanza cuando ya existe un usuario registrado con el mismo correo.</exception>
        public async Task<Usuario>  EditarUsuario(Usuario usuario, Stream fotoUsuario = null, string nombreFoto = "")
        {
            Usuario usuarioExistente = await _genericRepository.Obtener(usuarioQuery => usuarioQuery.Correo == usuario.Correo && usuarioQuery.IdUsuario != usuario.IdUsuario);

            if (usuarioExistente != null)
            {
                throw new TaskCanceledException("Ya existe un usuario registrado con este email");
            }

            try
            {

                IQueryable<Usuario> queryUsuario = await _genericRepository.Consultar(u => u.IdUsuario == usuario.IdUsuario);

                Usuario usuarioEditar = queryUsuario.First();

                usuarioEditar.Nombre = usuario.Nombre;
                usuarioEditar.Correo = usuario.Correo;
                usuarioEditar.Telefono = usuario.Telefono;
                usuarioEditar.IdRol = usuario.IdRol;
                usuarioEditar.EsActivo = usuario.EsActivo;

                if(usuarioEditar.NombreFoto == "")
                {
                    usuarioEditar.NombreFoto = nombreFoto;
                }
                if(fotoUsuario != null)
                {
                    string urlFoto = await _fireBaseService.SubirStorage(fotoUsuario, "carpeta_usuario", usuarioEditar.NombreFoto);
                    usuarioEditar.UrlFoto = urlFoto;

                }
                bool respuesta = await _genericRepository.Editar(usuarioEditar);

                if (!respuesta)
                {
                    throw new TaskCanceledException("No ha sido posible actualizar el perfil");
                }

                Usuario usuario_editado = queryUsuario.Include(rol => rol.IdRolNavigation).First();
                return usuario_editado;
            } catch (Exception) {
                throw;
            }
        }
        /// <summary>
        /// Elimina un usuario del sistema.
        /// </summary>
        /// <param name="id">Identificador del usuario a eliminar.</param>
        /// <returns>True si la eliminación fue exitosa, False si falla.</returns>
        /// <exception cref="TaskCanceledException">Se lanza cuando el usuario a eliminar no existe.</exception>
        public async Task<bool> DeleteUsuario(int id)
        {
            try
            {
                Usuario usuarioEncontrado = await _genericRepository.Obtener(user => user.IdUsuario == id);

                if(usuarioEncontrado == null) {
                    throw new TaskCanceledException("El usuario no existe");
                }
                string nombreFoto = usuarioEncontrado.NombreFoto;
                bool respuesta = await _genericRepository.Eliminar(usuarioEncontrado);

                if (respuesta)
                {
                    await _fireBaseService.EliminarStorage("carpeta_usuario", nombreFoto);
                    return true;
                }
                else
                {
                    throw new TaskCanceledException("Error al eliminar el usuario");
                    return false;
                }



            }   catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// Obtiene un usuario por sus credenciales (correo y clave).
        /// </summary>
        /// <param name="email">Correo electrónico del usuario.</param>
        /// <param name="clave">Clave del usuario.</param>
        /// <returns>Usuario encontrado.</returns>
        public async Task<Usuario> ObtenerPorCredenciales(string email, string clave)
        {
            string claveEncriptada = _utilidadesService.ConvertirClave(clave);
            Usuario usuario_encontrado = await _genericRepository.Obtener(u => u.Correo.Equals(email) && u.Clave.Equals(claveEncriptada));

            return usuario_encontrado;

        }
        /// <summary>
        /// Obtiene un usuario por su identificador.
        /// </summary>
        /// <param name="id">Identificador del usuario.</param>
        /// <returns>Usuario encontrado.</returns>
        public async Task<Usuario> ObtenerPorId(int id)
        {
            IQueryable<Usuario> query = await _genericRepository.Consultar(u => u.IdUsuario == id);
            Usuario usuario = query.Include(rol => rol.IdRolNavigation).FirstOrDefault();
            return usuario;
        }

        /// <summary>
        /// Guarda la información del perfil de un usuario.
        /// </summary>
        /// <param name="usuario">Usuario con la información actualizada.</param>
        /// <returns>True si la actualización fue exitosa, False si falla.</returns>
        /// <exception cref="TaskCanceledException">Se lanza cuando el usuario a actualizar no existe.</exception>
        public async Task<bool> GuardarPerfil(Usuario usuario)
        {
            try
            {
                Usuario usuario_encontrado = await _genericRepository.Obtener(u => u.IdUsuario == usuario.IdUsuario);

                if(usuario_encontrado == null)
                {
                    throw new TaskCanceledException("No se encuentra el usuario");
                }
                
                usuario_encontrado.Correo = usuario.Correo;
                usuario_encontrado.Telefono = usuario.Telefono;
                
                bool respuesta = await _genericRepository.Editar(usuario_encontrado);
                return respuesta;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Cambia la clave de un usuario.
        /// </summary>
        /// <param name="idUsuario">Identificador del usuario.</param>
        /// <param name="claveAntigua">Clave actual del usuario.</param>
        /// <param name="claveNueva">Nueva clave del usuario.</param>
        /// <returns>True si el cambio de clave fue exitoso, False si falla.</returns>
        /// <exception cref="TaskCanceledException">Se lanza cuando el usuario no existe o la clave actual es incorrecta.</exception>
        public async Task<bool> CambiarClave(int idUsuario, string claveAntigua, string claveNueva)
        {
            try 
            { 
                Usuario usuario_encontrado = await _genericRepository.Obtener(u=> u.IdUsuario ==idUsuario);

                if (usuario_encontrado == null)
                {
                    throw new TaskCanceledException("El usuario no existe");
                }
                if(usuario_encontrado.Clave != _utilidadesService.ConvertirClave(claveAntigua))
                {
                    throw new TaskCanceledException("La contraseña ingresada como actual es incorrecta");
                }

                usuario_encontrado.Clave = _utilidadesService.ConvertirClave(claveNueva);

                bool respuesta = await _genericRepository.Editar(usuario_encontrado);

                return respuesta;

            } 
            catch (Exception ex) 
            {
                throw;
            }
        }

        /// <summary>
        /// Restablece la clave de un usuario y envía la nueva clave por correo electrónico.
        /// </summary>
        /// <param name="correo">Correo electrónico del usuario.</param>
        /// <param name="plantillaCorreo">Plantilla HTML para el correo electrónico (opcional).</param>
        /// <returns>True si el restablecimiento de clave fue exitoso, False si falla.</returns>
        /// <exception cref="TaskCanceledException">Se lanza cuando el usuario no existe o hay un error en el envío del correo.</exception>
        public async Task<bool> RestablecerClave(string correo, string plantillaCorreo)
        {
            try 
            { 
                Usuario usuario_encontrado = await _genericRepository.Obtener(u => u.Correo == correo);

                if(usuario_encontrado == null)
                {
                    throw new TaskCanceledException("No se encuentra el usuario");
                }
                string claveGenerada = _utilidadesService.GenerarClave();
                usuario_encontrado.Clave = _utilidadesService.ConvertirClave(claveGenerada);
                if (plantillaCorreo != "")
                {
                    plantillaCorreo = plantillaCorreo.Replace("[correo]", usuario_encontrado.Correo).Replace("[clave]", claveGenerada);
                    string HTMLCorreo = "";

                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(plantillaCorreo);
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        using (Stream dataStream = response.GetResponseStream())
                        {
                            StreamReader readerStream = null;

                            if (response.CharacterSet == null)
                            {
                                readerStream = new StreamReader(dataStream);
                            }
                            else
                            {
                                readerStream = new StreamReader(dataStream, Encoding.GetEncoding(response.CharacterSet));
                            }
                            HTMLCorreo = readerStream.ReadToEnd();
                            response.Close();
                            readerStream.Close();
                        }
                    }
                    bool correo_enviado = false;
                    if (HTMLCorreo != "")
                    {
                        correo_enviado = await _correoService.enviarCorreo(correo, "Contraseña restablecida", HTMLCorreo);
                    }
                    if(!correo_enviado)
                    {
                        throw new TaskCanceledException("Error, intentalo de nuevo más tarde!");
                    }
                    

                }
                bool respuesta = await _genericRepository.Editar(usuario_encontrado);
                return respuesta;
            }
            catch (Exception ex) 
            {
                return false;
                throw;
                
            }
        }
    }
}
