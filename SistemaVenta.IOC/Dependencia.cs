using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using SistemaVenta.DAL.DBContext;

using SistemaVenta.DAL.Implementacion;
using SistemaVenta.DAL.Interfaces;

using SistemaVenta.BBL.Implementacion;
using SistemaVenta.BBL.Interfaces;

using Microsoft.EntityFrameworkCore;


namespace SistemaVenta.IOC
{
    public static class Dependencia
    {
        public  static void InyectarDependencia(this IServiceCollection services, IConfiguration Configuration) {
            
            services.AddDbContext<DbventaContext>(options => {
                options.UseSqlServer(Configuration.GetConnectionString("CadenaSQL"));
            });

            /*Para poder trabajar con entidades genericas e interfaces*/
            services.AddTransient(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<IVentaRepository, VentaRepository>();
            services.AddScoped<ICorreoService, CorreoService>();
            services.AddScoped<IFireBaseService, FireBaseService>();
            services.AddScoped<IUtilidadesService, UtilidadesService>();
            services.AddScoped<IRolService, RolService>();
            services.AddScoped<IUsuarioService, UsuarioService>();
            services.AddScoped<INegocioService, NegocioService>();
            services.AddScoped<ICategoriaService, CategoriaService>();
            services.AddScoped<IProductoService, ProductoService>();
            services.AddScoped<ITipoDocumentoVentaService, TipoDocumentoVentaService>();    
            services.AddScoped<IVentaService, VentaService>();
            services.AddScoped<IDashBoardService, DashBoardService>();

        }
    }
}
