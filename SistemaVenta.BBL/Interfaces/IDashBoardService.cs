using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace SistemaVenta.BBL.Interfaces
{
    /// <summary>
    /// Representa un contrato de servicio para operaciones relacionadas con el panel de control en una aplicación .NET.
    /// </summary>
    public interface IDashBoardService
    {
        /// <summary>
        /// Recupera el número total de ventas en la última semana.
        /// </summary>
        Task<int> TotalVentasUltimaSemana();

        /// <summary>
        /// Recupera el total de ingresos en la última semana como una cadena formateada.
        /// </summary>
        Task<string> TotalIngresosUltimaSemana();

        /// <summary>
        /// Recupera el número total de productos.
        /// </summary>
        Task<int> TotalProductos();

        /// <summary>
        /// Recupera el número total de categorías.
        /// </summary>
        Task<int> TotalCategorias();

        /// <summary>
        /// Recupera un diccionario que representa datos de ventas de la última semana, donde la clave es un día específico
        /// y el valor es el número correspondiente de ventas.
        /// </summary>
        Task<Dictionary<string, int>> VentasUltimaSemana();

        /// <summary>
        /// Recupera un diccionario que representa datos de ventas de los productos principales de la última semana, donde
        /// la clave es el nombre del producto y el valor es el número correspondiente de ventas.
        /// </summary>
        Task<Dictionary<string,int>> ProductosTopUltimaSemana();

    }
}
