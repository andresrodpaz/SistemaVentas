using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SistemaVenta.DAL.DBContext;
using SistemaVenta.DAL.Interfaces;

using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace SistemaVenta.DAL.Implementacion
{

    /// <summary>
    /// Implementación genérica del repositorio que proporciona operaciones básicas de CRUD para entidades específicas en la base de datos.
    /// </summary>
    /// <typeparam name="TEntity">Tipo de entidad con la que opera el repositorio.</typeparam>
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
    {

        
        /*TEntity es una entidad generica!*/
        private readonly DbventaContext _dbContext;

        /// <summary>
        /// Constructor que inicializa una nueva instancia del repositorio genérico con el contexto de la base de datos.
        /// </summary>
        /// <param name="dbContext">Contexto de la base de datos para la entidad específica.</param>
        public GenericRepository(DbventaContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Obtiene una entidad específica de la base de datos basándose en el filtro proporcionado.
        /// </summary>
        /// <param name="filtro">Expresión de filtro para buscar la entidad.</param>
        /// <returns>La entidad encontrada, o nulo si no se encuentra.</returns>
        public async Task<TEntity> Obtener(Expression<Func<TEntity, bool>> filtro)
        {
            try
            {
                TEntity entidad = await _dbContext.Set<TEntity>().FirstOrDefaultAsync(filtro);
                return entidad;
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// Crea una nueva entidad en la base de datos.
        /// </summary>
        /// <param name="entidad">La entidad a ser creada.</param>
        /// <returns>La entidad recién creada.</returns>
        public async Task<TEntity> Crear(TEntity entidad)
        {
            try
            {
                _dbContext.Set<TEntity>().Add(entidad);
                await _dbContext.SaveChangesAsync();
                return entidad;
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// Edita una entidad existente en la base de datos.
        /// </summary>
        /// <param name="entidad">La entidad a ser editada.</param>
        /// <returns>True si la edición fue exitosa, False si no.</returns>
        public async Task<bool> Editar(TEntity entidad)
        {
            try
            {
                _dbContext.Set<TEntity>().Update(entidad);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// Elimina una entidad existente de la base de datos.
        /// </summary>
        /// <param name="entidad">La entidad a ser eliminada.</param>
        /// <returns>True si la eliminación fue exitosa, False si no.</returns>
        public async Task<bool> Eliminar(TEntity entidad)
        {
            try
            {
                _dbContext.Set<TEntity>().Remove(entidad);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {

                throw;
            }
        }
        /// <summary>
        /// Consulta entidades en la base de datos basándose en un filtro opcional.
        /// </summary>
        /// <param name="filtro">Expresión de filtro para limitar los resultados (opcional).</param>
        /// <returns>Una consulta que representa las entidades encontradas.</returns>
        public async Task<IQueryable<TEntity>> Consultar(Expression<Func<TEntity, bool>> filtro = null)
        {
            /*1ª validamos si el filtro es igual a nulo, si es así entonces retorna la consulta*/
            IQueryable<TEntity> queryEntidad = filtro==null ? _dbContext.Set<TEntity>():_dbContext.Set<TEntity>().Where(filtro);
            return queryEntidad;

            /*
             IQueryable<TEntity> queryEntidad;

                if (filtro == null)
                {
                    queryEntidad = _dbContext.Set<TEntity>();
                }
                else
                {
                    queryEntidad = _dbContext.Set<TEntity>().Where(filtro);
                }

             */
        }
    }
}
