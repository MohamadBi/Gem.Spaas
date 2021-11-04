using WebSocketSharp;
using WebSocketSharp.Server;

namespace Gem.Spaas.Service.WebSockets
{
    public class ProductionPlanSocket : WebSocketBehavior
    {
        protected override void OnMessage(MessageEventArgs e)
        {
            Send(e.Data);
        }
    }

    public static class ProductionPlanWebSocketServer
    {
        private static readonly WebSocketServer server = new WebSocketServer("ws://localhost:5555");
        public static void Start()
        {
            server.AddWebSocketService<ProductionPlanSocket>("/pp");
            server.Start();
        }

        public static void Stop()
        {
            server.Stop();
        }

        public static void Broadcast(string message)
        {
            server.WebSocketServices["/pp"].Sessions.Broadcast(message);
        }
    }
}
