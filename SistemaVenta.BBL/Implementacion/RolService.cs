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
    /// Implementación del servicio de roles en el sistema.
    /// </summary>
    public class RolService : IRolService
    {
        private readonly IGenericRepository<Rol> _repository;
        /// <summary>
        /// Constructor de la clase RolService.
        /// </summary>
        /// <param name="repository">Instancia del repositorio genérico para interactuar con la capa de datos.</param>
        public RolService(IGenericRepository<Rol> repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// Obtiene una lista de todos los roles disponibles en el sistema.
        /// </summary>
        /// <returns>Lista de objetos de tipo <see cref="Rol"/>.</returns>
        public async Task<List<Rol>> Lista()
        {
            IQueryable<Rol> query = await _repository.Consultar();
            return query.ToList();
        }
    }
}
