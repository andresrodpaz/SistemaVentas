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
    /// Implementación del servicio para la gestión de tipos de documento de venta en el sistema.
    /// </summary>
    public class TipoDocumentoVentaService : ITipoDocumentoVentaService
    {
        private readonly IGenericRepository<TipoDocumentoVenta> _repository;
        /// <summary>
        /// Constructor de la clase TipoDocumentoVentaService.
        /// </summary>
        /// <param name="repository">Instancia del repositorio genérico para interactuar con la capa de datos.</param>
        public TipoDocumentoVentaService(IGenericRepository<TipoDocumentoVenta> repository)
        {
            _repository = repository;
        }
        /// <summary>
        /// Obtiene una lista de todos los tipos de documento de venta disponibles en el sistema.
        /// </summary>
        /// <returns>Lista de objetos de tipo <see cref="TipoDocumentoVenta"/>.</returns>
        public async Task<List<TipoDocumentoVenta>> Lista()
        {
            IQueryable<TipoDocumentoVenta> query = await _repository.Consultar();
            return query.ToList();
        }
    }
}
