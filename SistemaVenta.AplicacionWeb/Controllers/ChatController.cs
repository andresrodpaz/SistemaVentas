using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using SistemaVenta.AplicacionWeb.Utilidades.SignalR;

namespace SistemaVenta.AplicacionWeb.Controllers
{
    public class ChatController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        private readonly IHubContext<ChatHub> _hubContext;

        public ChatController(IHubContext<ChatHub> hubContext)
        {
            _hubContext = hubContext;
        }


        [HttpPost]
        public IActionResult EnviarMensaje(string mensaje)
        {
            // Procesar el mensaje, almacenarlo en la base de datos, realizar alguna lógica, etc.

            // Enviar el mensaje a todos los clientes a través del hub SignalR
            _hubContext.Clients.All.SendAsync("ReceiveMessage", new { text = mensaje });

            // Puedes devolver algo, por ejemplo, un código de estado o un mensaje de éxito
            return Ok("Mensaje enviado con éxito");
        }
    }
}
