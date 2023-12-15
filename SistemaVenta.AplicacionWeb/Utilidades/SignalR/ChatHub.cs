using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace SistemaVenta.AplicacionWeb.Utilidades.SignalR
{
    public class ChatHub : Hub
    {
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", new { text = message });
        }
    }

    public class ChatMessage
    {
        public string Text { get; set; }
        public string File { get; set; }
    }
}
