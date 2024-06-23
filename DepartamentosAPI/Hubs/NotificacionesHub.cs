using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;
using System.Security.Cryptography.X509Certificates;

namespace DepartamentosAPI.Hubs
{
    public class NotificacionesHub:Hub
    {
        public async Task EnviarNotificacion(string msg)
        {
            await Clients.All.SendAsync("RecibirMensaje", msg);
        }
    }
}
