using Fleck;

namespace EdcHost.ViewerServers;

public class WebSocketServerHub : IWebSocketServerHub
{
    public IWebSocketServer Get(int port)
    {
        return new WebSocketServer($"ws://0.0.0.0:{port}");
    }
}
