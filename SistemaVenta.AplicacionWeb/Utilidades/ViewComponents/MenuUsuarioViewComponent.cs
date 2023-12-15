using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;


namespace SistemaVenta.AplicacionWeb.Utilidades.ViewComponents
{
    public class MenuUsuarioViewComponent : ViewComponent
    {
        /*
         Método InvokeAsync para obtener datos del usuario autenticado y proporcionarlos a la vista.
         */
        public async Task<IViewComponentResult> InvokeAsync()
        {
            //1- Obtención del ClaimsPrincipal
            ClaimsPrincipal claimUser = HttpContext.User;
            string nombreUsuario = "";
            string urlFotoUsuario = "";

            //2- Verificamos si está autenticado
            if(claimUser.Identity.IsAuthenticated)
            {
                //3- Recuperamos los datos del Usuario
                nombreUsuario = claimUser.Claims
                    .Where(c => c.Type == ClaimTypes.Name)//ClaimTypes.Name hace referencia al Claims que creamos al hacer Login, al crearla creamos la propiedad Name
                    .Select(c => c.Value).SingleOrDefault();

                urlFotoUsuario = ((ClaimsIdentity)claimUser.Identity).FindFirst("UrlFoto").Value;    
            }
            //4- Asignación de Datos a ViewDat
            ViewData["nombreUsuario"] = nombreUsuario;
            ViewData["urlFotoUsuario"] = urlFotoUsuario;

            //5- Renderizo la vista
            return View();

        }
    }
}
