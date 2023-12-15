using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Linq.Expressions;

namespace SistemaVenta.DAL.Interfaces
{
    // <summary>
    /// Interfaz genérica para definir operaciones de acceso a datos comunes para entidades específicas.
    /// </summary>
    /// <typeparam name="TEntity">Tipo de entidad para la que se definen las operaciones.</typeparam>
    public interface IGenericRepository<TEntity> where TEntity : class
    {

        /// <summary>
        /// Recupera una entidad que cumple con ciertos criterios especificados por una expresión lambda.
        /// </summary>
        /// <param name="filtro">Expresión lambda que define los criterios de selección.</param>
        /// <returns>Una tarea que representa la operación y devuelve la entidad recuperada.</returns>
        Task<TEntity> Obtener(Expression<Func<TEntity, bool>> filtro);

        /// <summary>
        /// Crea una nueva entidad en la fuente de datos.
        /// </summary>
        /// <param name="entidad">La entidad que se va a crear.</param>
        /// <returns>Una tarea que representa la operación y devuelve la entidad creada.</returns>
        Task<TEntity> Crear(TEntity entidad);

        /// <summary>
        /// Actualiza una entidad existente en la fuente de datos.
        /// </summary>
        /// <param name="entidad">La entidad que se va a actualizar.</param>
        /// <returns>Una tarea que representa la operación y devuelve un valor booleano que indica el éxito de la operación.</returns>
        Task<bool> Editar(TEntity entidad);

        /// </summary>
        /// <param name="entidad">La entidad que se va a eliminar.</param>
        /// <returns>Una tarea que representa la operación y devuelve un valor booleano que indica el éxito de la operación.</returns>
        Task<bool> Eliminar(TEntity entidad);

        /// <summary>
        /// Recupera una colección de entidades que cumplen con ciertos criterios especificados por una expresión lambda opcional.
        /// </summary>
        /// <param name="filtro">Expresión lambda que define los criterios de selección (opcional).</param>
        /// <returns>Una tarea que representa la operación y devuelve una colección de entidades.</returns>
        Task<IQueryable<TEntity>> Consultar(Expression<Func<TEntity, bool>> filtro = null);
    }
}
