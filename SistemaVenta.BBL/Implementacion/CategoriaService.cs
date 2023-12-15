using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using SistemaVenta.BBL.Interfaces;
using SistemaVenta.DAL.Interfaces;
using SistemaVenta.Entity;


namespace SistemaVenta.BBL.Implementacion
{
    /// <summary>
    /// Implementación de la interfaz ICategoriaService que define operaciones relacionadas con la gestión de categorías.
    /// </summary>
    public class CategoriaService : ICategoriaService
    {
        private readonly IGenericRepository<Categoria> _genericRepository;

        /// <summary>
        /// Constructor de la clase CategoriaService.
        /// </summary>
        /// <param name="repository">Instancia de IGenericRepository para acceder a la capa de datos.</param>
        public CategoriaService(IGenericRepository<Categoria> repository)
        {
            _genericRepository = repository;
        }

        /// <summary>
        /// Obtiene una lista de todas las categorías.
        /// </summary>
        /// <returns>Una tarea que representa la operación asíncrona y devuelve la lista de categorías.</returns>
        public async Task<List<Categoria>> Lista()
        {
            IQueryable<Categoria> query = await _genericRepository.Consultar();
            return query.ToList();
        }

        /// <summary>
        /// Crea una nueva categoría.
        /// </summary>
        /// <param name="entidad">La categoría a ser creada.</param>
        /// <returns>Una tarea que representa la operación asíncrona y devuelve la categoría recién creada.</returns>
        public async Task<Categoria> Crear(Categoria entidad)
        {
            try
            {
                Categoria categoriaCreada = await _genericRepository.Crear(entidad);

                if(categoriaCreada.IdCategoria == 0)
                {
                    throw new TaskCanceledException("No se ha podido crear la categoria");
                }
                return categoriaCreada;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw;
            }
        }

        /// <summary>
        /// Edita una categoría existente.
        /// </summary>
        /// <param name="entidad">La categoría con los cambios a aplicar.</param>
        /// <returns>Una tarea que representa la operación asíncrona y devuelve la categoría modificada.</returns>
        public async Task<Categoria> Editar(Categoria entidad)
        {
            try
            {
                Categoria categoriaEncontrada = await _genericRepository.Obtener(cat => cat.IdCategoria ==  entidad.IdCategoria); 
                categoriaEncontrada.Descripcion = entidad.Descripcion;
                categoriaEncontrada.EsActivo = entidad.EsActivo;
                bool respuesta = await _genericRepository.Editar(categoriaEncontrada);

                if (!respuesta)
                {
                    throw new TaskCanceledException("No se pudo modificar la categoria");
                }
                return categoriaEncontrada;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
                throw;
            }
        }

        /// <summary>
        /// Elimina una categoría por su identificador.
        /// </summary>
        /// <param name="idCategoria">El identificador de la categoría a ser eliminada.</param>
        /// <returns>Una tarea que representa la operación asíncrona y devuelve true si la eliminación fue exitosa, false en caso contrario.</returns>
        public async Task<bool> Elminar(int idCategoria)
        {
            try
            {
                Categoria categoriaEncontrada = await _genericRepository.Obtener(cat => cat.IdCategoria == idCategoria);

                if(categoriaEncontrada == null)
                {
                    throw new TaskCanceledException("No se ha encontrado la categoria, no existe");
                }

                bool respuesta = await _genericRepository.Eliminar(categoriaEncontrada);
                return respuesta;
            } catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
                throw;
            }
        }

        
    }
}
