using Fleck;
namespace EdcHost.ViewerServers;

/// <summary>
/// ViewerServer handles the API requests from viewers via WebSocket.
/// </summary>
public interface IViewerServer
{
    static IViewerServer Create(int port)
    {
        WebSocketServerHub wsServerHub = new();
        return new ViewerServer(port, wsServerHub);
    }

    event EventHandler<AfterMessageReceiveEventArgs>? AfterMessageReceiveEvent;

    /// <summary>
    /// Starts the server.
    /// </summary>
    void Start();

    /// <summary>
    /// Stops the server.
    /// </summary>
    void Stop();

    void Publish(Message message);
}
