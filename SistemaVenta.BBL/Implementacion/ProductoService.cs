using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using SistemaVenta.BBL.Interfaces;
using SistemaVenta.DAL.Interfaces;
using SistemaVenta.Entity;

namespace SistemaVenta.BBL.Implementacion
{
    /// <summary>
    /// Implementación de servicio para la gestión de productos.
    /// </summary>
    public class ProductoService : IProductoService
    {
        private readonly IGenericRepository<Producto> _repository;
        private readonly IFireBaseService _fireBaseService;

        /// <summary>
        /// Constructor de la clase ProductoService.
        /// </summary>
        /// <param name="repository">Instancia del repositorio genérico para acceder a la información de productos.</param>
        /// <param name="fireBase">Instancia del servicio de FireBase para operaciones relacionadas con almacenamiento.</param>
        public ProductoService(IGenericRepository<Producto> repository,IFireBaseService fireBase)
        {
            _repository = repository;
            _fireBaseService = fireBase;
        }

        /// <summary>
        /// Obtiene una lista de productos, incluyendo la información de categoría asociada.
        /// </summary>
        /// <returns>Una tarea que representa la operación asíncrona y devuelve la lista de productos con información de categoría  <see cref="Categoria"/>.</returns>
        public async Task<List<Producto>> Lista()
        {
            IQueryable<Producto> query = await _repository.Consultar();
            return query.Include(c => c.IdCategoriaNavigation).ToList();
        }

        /// <summary>
        /// Crea un nuevo producto, permitiendo la asociación de una imagen.
        /// </summary>
        /// <param name="entidad">La entidad del producto a crear.</param>
        /// <param name="imagen">El flujo de datos de la imagen asociada, si se proporciona.</param>
        /// <param name="nombreImagen">El nombre de la imagen, si se proporciona.</param>
        /// <returns>Una tarea que representa la operación asíncrona y devuelve el producto creado.</returns>
        public async Task<Producto> Crear(Producto entidad, Stream imagen = null, string nombreImagen = "")
        {
            Producto productoEncontrado = await _repository.Obtener(prod => prod.CodigoBarra == entidad.CodigoBarra);

            if(productoEncontrado != null)
            {
                throw new TaskCanceledException("Error, ya hay un producto con ese codigo de barras");
            }
            try
            {
                entidad.NombreImagen = nombreImagen;
                if(imagen  != null)
                {
                    string urlImagen = await _fireBaseService.SubirStorage(imagen, "carpeta_producto", nombreImagen);
                    entidad.UrlImagen = urlImagen;
                }
                Producto productoCreado = await _repository.Crear(entidad);
                if( productoCreado.IdProducto == 0)
                {
                    throw new TaskCanceledException("No se ha podido crear el producto");
                }

                IQueryable<Producto> query = await _repository.Consultar(prod => prod.IdProducto == entidad.IdProducto);
                productoCreado = query.Include(c => c.IdCategoriaNavigation).First();

                return productoCreado;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw;
            }

        }

        /// <summary>
        /// Edita un producto existente, permitiendo la actualización de información y de la imagen asociada.
        /// </summary>
        /// <param name="entidad">La entidad del producto con los cambios a aplicar.</param>
        /// <param name="imagen">El flujo de datos de la nueva imagen asociada, si se proporciona.</param>
        /// <param name="nombreImagen">El nombre de la nueva imagen, si se proporciona.</param>
        /// <returns>Una tarea que representa la operación asíncrona y devuelve el producto editado.</returns>
        public async Task<Producto> Editar(Producto entidad, Stream imagen = null, string nombreImagen = "")
        {
            Producto productoExiste = await _repository.Obtener(prod => prod.CodigoBarra == entidad.CodigoBarra && prod.IdProducto != entidad.IdProducto);
            if(productoExiste != null)
            {
                throw new TaskCanceledException("El codigo de barra ya existe");
            }
            try
            {
                IQueryable<Producto> queryProducto = await _repository.Consultar(P=>P.IdProducto== entidad.IdProducto);
                Producto productoEditar = queryProducto.First();
                productoEditar.CodigoBarra = entidad.CodigoBarra;
                productoEditar.Marca= entidad.Marca;
                productoEditar.Descripcion = entidad.Descripcion;
                productoEditar.IdCategoria = entidad.IdCategoria;
                productoEditar.Stock = entidad.Stock;
                productoEditar.Precio = entidad.Precio;
                productoEditar.EsActivo = entidad.EsActivo;

                if(productoEditar.NombreImagen == "")
                {
                    productoEditar.NombreImagen = nombreImagen;
                }

                if(imagen!=null)
                {
                    string urlImagen = await _fireBaseService.SubirStorage(imagen, "carpeta_producto", productoEditar.NombreImagen);
                    productoEditar.UrlImagen = urlImagen;
                }
                bool respuesta = await _repository.Editar(productoEditar);
                if (!respuesta)
                {
                    throw new TaskCanceledException("Error al editar el producto");
                }
                Producto productoEditado = queryProducto.Include(c=>c.IdCategoriaNavigation).FirstOrDefault();
                return productoEditado;

            } catch (Exception ex) { 
                Console.WriteLine(ex.ToString());
                throw;
            }
        
        }
        /// <summary>
        /// Elimina un producto existente, incluyendo la eliminación de la imagen asociada.
        /// </summary>
        /// <param name="idProducto">El identificador del producto a eliminar.</param>
        /// <returns>Una tarea que representa la operación asíncrona y devuelve true si la eliminación fue exitosa.</returns>
        public async Task<bool> Eliminar(int idProducto)
        {
            try
            {
                Producto productoEncontrado = await _repository.Obtener(prod => prod.IdProducto == idProducto);

                if(productoEncontrado == null)
                {
                    throw new TaskCanceledException("No existe el producto");
                }
                string nombreImagen = productoEncontrado.NombreImagen;
                bool resultado = await _repository.Eliminar(productoEncontrado);

                if(resultado)
                {
                    await _fireBaseService.EliminarStorage("carpeta_producto", nombreImagen);
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
                return false;
                throw;
            }
        }

        
    }
}
