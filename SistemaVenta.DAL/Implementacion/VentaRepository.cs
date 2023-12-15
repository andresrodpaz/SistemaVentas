using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using SistemaVenta.Entity;
using SistemaVenta.DAL.Interfaces;
using SistemaVenta.DAL.DBContext;


namespace SistemaVenta.DAL.Implementacion
{
    /// <summary>
    /// Implementación concreta del repositorio de ventas que interactúa con la base de datos.
    /// </summary>
    public class VentaRepository : GenericRepository<Venta>, IVentaRepository
    {

        private readonly DbventaContext _dbventaContext;

        /// <summary>
        /// Constructor que recibe el contexto de la base de datos.
        /// </summary>
        /// <param name="dbventaContext">Contexto de la base de datos para la venta.</param>
        public VentaRepository(DbventaContext dbventaContext):base (dbventaContext)
        {
            _dbventaContext = dbventaContext;
        }

        /// <summary>
        /// Registra una nueva venta en la base de datos.
        /// </summary>
        /// <param name="entidad">Venta a ser registrada.</param>
        /// <returns>La venta generada.</returns>
        public async Task<Venta> Registrar(Venta entidad)
        {
            // Inicializa una nueva instancia de Venta para almacenar la venta generada.
            Venta ventaGenerada = new Venta();

            // Utiliza una transacción para asegurar la consistencia de la base de datos en caso de errores.
            using (var transaction = _dbventaContext.Database.BeginTransaction())
            {
                try
                {
                    // Actualiza el stock de productos según la cantidad vendida.
                    foreach (DetalleVenta item in entidad.DetalleVenta)
                    {
                        Producto _productoEncontrado = _dbventaContext.Productos
                            .Where(p => p.IdProducto == item.IdProducto)
                            .First();

                        //Verifico que no de negativo
                        var stock = _productoEncontrado.Stock - item.Cantidad;
                        if(stock > 0) { 
                            _productoEncontrado.Stock = _productoEncontrado.Stock - item.Cantidad;
                        } else
                        {
                            _productoEncontrado.Stock = 0;
                        }
                        
                        _dbventaContext.Productos.Update(_productoEncontrado);
                    }

                    // Guarda los cambios en la base de datos.
                    await _dbventaContext.SaveChangesAsync();

                    // Actualiza el correlativo de la venta para generar un nuevo número de venta.
                    NumeroCorrelativo correlativo = _dbventaContext.NumeroCorrelativos
                        .Where(c => c.Gestion == "venta")
                        .First();

                    correlativo.UltimoNumero = correlativo.UltimoNumero + 1;
                    correlativo.FechaActualizacion = DateTime.Now;

                    _dbventaContext.NumeroCorrelativos.Update(correlativo);
                    await _dbventaContext.SaveChangesAsync();

                    // Genera el número de venta con ceros a la izquierda según la cantidad de dígitos requeridos.
                    string ceros = string.Concat(Enumerable.Repeat("0", correlativo.CantidadDigitos.Value));
                    string numeroVenta = string.Concat(ceros, correlativo.UltimoNumero.ToString());
                    numeroVenta = numeroVenta.Substring(numeroVenta.Length - correlativo.CantidadDigitos.Value, correlativo.CantidadDigitos.Value);

                    // Asigna el número de venta a la entidad de venta.
                    entidad.NumeroVenta = numeroVenta;

                    // Agrega la nueva venta a la base de datos.
                    await _dbventaContext.Venta.AddAsync(entidad);
                    await _dbventaContext.SaveChangesAsync();

                    // Almacena la venta generada.
                    ventaGenerada = entidad;

                    // Confirma la transacción.
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    // En caso de error, realiza un rollback de la transacción y lanza la excepción.
                    transaction.Rollback();
                    throw ex;
                }
                // Retorna la venta
                return ventaGenerada;
            }
        }
        /// <summary>
        /// Genera un reporte de ventas en un rango de fechas especificado.
        /// </summary>
        /// <param name="fechaInicio">Fecha de inicio del rango.</param>
        /// <param name="fechaFin">Fecha de fin del rango.</param>
        /// <returns>Lista de detalles de ventas en el rango de fechas.</returns>
        public async Task<List<DetalleVenta>> Reporte(DateTime fechaInicio, DateTime fechaFin)
        {
            // Obtiene una lista de detalles de ventas con información adicional.
            List<DetalleVenta> listaResumen = await _dbventaContext.DetalleVenta
                .Include(v => v.IdVentaNavigation)
                    .ThenInclude(u => u.IdUsuarioNavigation)
                .Include(v => v.IdVentaNavigation)
                    .ThenInclude(tdv => tdv.IdTipoDocumentoVentaNavigation)
                .Where(dv => dv.IdVentaNavigation.FechaRegistro.Value >= fechaInicio.Date && dv.IdVentaNavigation.FechaRegistro.Value <= fechaFin.Date) //Busca que la fecha de registro de la venta esté entre ambos parametros
                .ToListAsync();
                ;
            return listaResumen;
        }
    }
}
