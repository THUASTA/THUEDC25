using Fleck;

namespace EdcHost.ViewerServers;

public interface IWebSocketServerHub
{
    IWebSocketServer Get(int port);
}
