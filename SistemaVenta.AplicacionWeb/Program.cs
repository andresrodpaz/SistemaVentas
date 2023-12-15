using SistemaVenta.IOC;
using SistemaVenta.AplicacionWeb.Utilidades.AutoMapper;


//PDF
using SistemaVenta.AplicacionWeb.Utilidades.Extensiones;
using DinkToPdf;
using DinkToPdf.Contracts;

//Cookies
using Microsoft.AspNetCore.Authentication.Cookies;
using SistemaVenta.AplicacionWeb.Utilidades.SignalR;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();


//Add SignalR
builder.Services.AddSignalR();

// Configuraci�n de la autenticaci�n basada en cookies para la aplicaci�n ASP.NET Core
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        // Establece la ruta a la cual redirigir a los usuarios no autenticados cuando intentan acceder a una p�gina protegida
        options.LoginPath = "/Acceso/Login";

        // Establece el tiempo de expiraci�n de la cookie de autenticaci�n a 20 minutos
        options.ExpireTimeSpan = TimeSpan.FromMinutes(20);
    });


builder.Services.InyectarDependencia(builder.Configuration);

builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

// Crear un nuevo contexto de carga de ensamblajes personalizado.
var context = new CustomAssemblyLoadContext();

// Combinar el directorio actual con la ruta relativa de la biblioteca no administrada "libwkhtmltox.dll".
var absolutePath = Path.Combine(Directory.GetCurrentDirectory(), "Utilidades/LibreriaPDF/libwkhtmltox.dll");

// Cargar la biblioteca no administrada utilizando el contexto personalizado.
// Esta l�nea utiliza el m�todo LoadUnmanagedLibrary definido en el contexto de carga personalizado.
context.LoadUnmanagedLibrary(absolutePath);

// Configurar el servicio Singleton para la interfaz IConverter.
// Utiliza la implementaci�n SynchronizedConverter con PdfTools como proveedor de herramientas PDF.
// SynchronizedConverter garantiza la sincronizaci�n en entornos multiproceso.
// Este servicio se utiliza para convertir documentos a PDF.
builder.Services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<ChatHub>("/chatHub");
});


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Acceso}/{action=Login}/{id?}");

app.Run();
