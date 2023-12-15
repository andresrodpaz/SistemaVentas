using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SistemaVenta.Entity;

namespace SistemaVenta.BBL.Interfaces
{
    /// <summary>
    /// Interfaz para definir operaciones relacionadas con la gestión de categorías.
    /// </summary>
    public interface ICategoriaService
    {
        /// <summary>
        /// Obtiene una lista de categorías.
        /// </summary>
        /// <returns>Una tarea que representa la operación asíncrona y devuelve la lista de categorías.</returns>
        Task<List<Categoria>> Lista();

        /// <summary>
        /// Crea una nueva categoría.
        /// </summary>
        /// <param name="entidad">La categoría a ser creada.</param>
        /// <returns>Una tarea que representa la operación asíncrona y devuelve la categoría recién creada.</returns>
        Task<Categoria> Crear(Categoria entidad);

        /// <summary>
        /// Edita una categoría existente.
        /// </summary>
        /// <param name="entidad">La categoría con los cambios a aplicar.</param>
        /// <returns>Una tarea que representa la operación asíncrona y devuelve la categoría modificada.</returns>
        Task<Categoria> Editar(Categoria entidad);

        /// <summary>
        /// Elimina una categoría por su identificador.
        /// </summary>
        /// <param name="idCategoria">El identificador de la categoría a ser eliminada.</param>
        /// <returns>Una tarea que representa la operación asíncrona y devuelve true si la eliminación fue exitosa, false en caso contrario.</returns>
        Task<bool> Elminar(int idCategoria);

    }
}
