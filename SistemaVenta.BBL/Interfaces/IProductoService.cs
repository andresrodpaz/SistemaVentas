using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SistemaVenta.Entity;

namespace SistemaVenta.BBL.Interfaces
{
    /// <summary>
    /// Interfaz que define operaciones para la gestión de la entidad de producto en una aplicación .NET.
    /// </summary>
    public interface IProductoService
    {

        /// <summary>
        /// Obtiene una lista de productos.
        /// </summary>
        /// <returns>Una tarea que, al completarse, proporciona una lista de productos.</returns>
        Task<List<Producto>> Lista();

        /// <summary>
        /// Crea un nuevo producto, con la opción de asociar una imagen.
        /// </summary>
        /// <param name="entidad">La entidad de producto a crear.</param>
        /// <param name="imagen">Flujo de datos de la imagen del producto (opcional).</param>
        /// <param name="nombreImagen">Nombre de la imagen del producto (opcional).</param>
        /// <returns>Una tarea que, al completarse, devuelve la entidad de producto creada.</returns>
        Task<Producto> Crear(Producto entidad, Stream imagen = null, string nombreImagen ="");

        /// <summary>
        /// Edita un producto existente, con la opción de actualizar su imagen.
        /// </summary>
        /// <param name="entidad">La entidad de producto a editar.</param>
        /// <param name="imagen">Flujo de datos de la nueva imagen del producto (opcional).</param>
        /// <param name="nombreImagen">Nombre de la nueva imagen del producto (opcional).</param>
        /// <returns>Una tarea que, al completarse, devuelve la entidad de producto actualizada.</returns>
        Task<Producto> Editar(Producto entidad, Stream imagen = null, string nombreImagen = "");

        /// <summary>
        /// Elimina un producto por su identificador.
        /// </summary>
        /// <param name="idProducto">El identificador del producto a eliminar.</param>
        /// <returns>Una tarea que, al completarse, indica si la operación de eliminación fue exitosa.</returns>
        Task<bool> Eliminar(int idProducto);
    }
}
