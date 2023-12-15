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
    /// Implementación de servicio para la realización de operaciones en FireBase.
    /// </summary>
    public class NegocioService : INegocioService
    {
        private readonly IGenericRepository<Negocio> _repository;
        private readonly IFireBaseService _firebaseService;

        /// <summary>
        /// Constructor de la clase NegocioService.
        /// </summary>
        /// <param name="repository">Instancia del repositorio genérico para acceder a la información del negocio.</param>
        /// <param name="fireBaseService">Instancia del servicio de FireBase para operaciones relacionadas con el almacenamiento.</param>
        public NegocioService(IGenericRepository<Negocio> repository, IFireBaseService fireBaseService)
        {
            _repository = repository;
            _firebaseService = fireBaseService;
        }

        /// <summary>
        /// Obtiene la información del negocio.
        /// </summary>
        /// <returns>Una tarea que representa la operación asíncrona y devuelve la entidad de negocio encontrada.</returns>
        public async Task<Negocio> Obtener()
        {
            try
            {
                // Consulta el negocio con el identificador 1 - Ya que solo se maneja 1 negocio, entonces se asigna el ID 1 al negocio en la BBDD
                Negocio negocioEncontrado = await _repository.Obtener(negocio => negocio.IdNegocio == 1);
                return negocioEncontrado;
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// Guarda los cambios en la información del negocio, permitiendo la actualización de datos y del logo.
        /// </summary>
        /// <param name="entidad">La entidad de negocio con los cambios a aplicar.</param>
        /// <param name="Logo">El flujo de datos del nuevo logo, si se proporciona.</param>
        /// <param name="nombreLogo">El nombre del nuevo logo, si se proporciona.</param>
        /// <returns>Una tarea que representa la operación asíncrona y devuelve la entidad de negocio actualizada.</returns>
        public async Task<Negocio> GuardarCambios(Negocio entidad, Stream Logo = null, string nombreLogo = "")
        {
            try
            {
                // Obtiene el negocio actual de la base de datos
                Negocio negocioEncontrado = await _repository.Obtener(negocio => negocio.IdNegocio == 1);

                // Actualiza las propiedades del negocio con los nuevos valores
                negocioEncontrado.NumeroDocumento = entidad.NumeroDocumento;
                negocioEncontrado.Nombre = entidad.Nombre;
                negocioEncontrado.Correo = entidad.Correo;
                negocioEncontrado.Telefono = entidad.Telefono;
                negocioEncontrado.Direccion = entidad.Direccion;
                negocioEncontrado.PorcentajeImpuesto= entidad.PorcentajeImpuesto;
                negocioEncontrado.SimboloMoneda = entidad.SimboloMoneda;
                // Asigna el nuevo nombre de logo si es proporcionado
                negocioEncontrado.NombreLogo = negocioEncontrado.NombreLogo == "" ? nombreLogo : negocioEncontrado.NombreLogo;

                // Si se proporciona un nuevo logo, lo carga al servicio de almacenamiento Firebase
                if (Logo != null)
                {
                    // Sube el nuevo logo y obtiene la URL resultante
                    string UrlFoto = await _firebaseService.SubirStorage(Logo, "carpeta_logo", negocioEncontrado.NombreLogo);
                    negocioEncontrado.UrlLogo = UrlFoto;

                }
                // Realiza la edición en la base de datos
                await _repository.Editar(negocioEncontrado);
                // Retorna la entidad de negocio actualizada
                return negocioEncontrado;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw;
            }
        }

        
    }
}
