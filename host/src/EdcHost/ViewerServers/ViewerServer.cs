using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
using Fleck;
using Serilog;

namespace EdcHost.ViewerServers;

/// <summary>
/// ViewerServer handles the API requests from viewers via WebSocket.
/// </summary>
public class ViewerServer : IViewerServer
{
    public event EventHandler<AfterMessageReceiveEventArgs>? AfterMessageReceiveEvent;

    readonly ILogger _logger = Log.Logger.ForContext("Component", "ViewerServers");
    readonly int _port;
    readonly ConcurrentDictionary<Guid, IWebSocketConnection> _sockets = new();
    readonly IWebSocketServerHub _wsServerHub;

    bool _isRunning = false;
    IWebSocketServer? _wsServer = null;


    public ViewerServer(int port, IWebSocketServerHub wsServerHub)
    {
        _port = port;
        _wsServerHub = wsServerHub;
    }

    /// <summary>
    /// Starts the server.
    /// </summary>
    public void Start()
    {
        if (_isRunning)
        {
            throw new InvalidOperationException("already running");
        }

        Debug.Assert(_wsServer is null);

        _logger.Information("Starting...");

        try
        {
            _wsServer = _wsServerHub.Get(_port);
            StartWsServer();

            _isRunning = true;

            _logger.Information("Started.");
        }
        catch (Exception ex)
        {
            _logger.Error($"Failed to start viewer server: {ex}");
        }

    }

    /// <summary>
    /// Stops the server.
    /// </summary>
    public void Stop()
    {
        if (!_isRunning)
        {
            throw new InvalidOperationException("not running");
        }

        _logger.Information("Stopping...");

        Debug.Assert(_wsServer is not null);

        _wsServer.Dispose();

        _wsServer = null;

        _isRunning = false;

        _logger.Information("Stopped.");
    }

    public void Publish(Message message)
    {
        string jsonString = message.Json;

        foreach (IWebSocketConnection socket in _sockets.Values)
        {
            try
            {
                socket.Send(jsonString).Wait();
            }
            catch (Exception ex)
            {
                _logger.Error($"Failed to send message to {socket.ConnectionInfo.ClientIpAddress}: {ex.Message}");

                // Do not throw even in debug mode to allow program to continue after a client disconnects.
            }
        }
    }

    void ParseMessage(string text)
    {
        Message? generalMessage = JsonSerializer.Deserialize<Message>(text) ?? throw new Exception("failed to deserialize message");

        switch (generalMessage.MessageType)
        {
            case "COMPETITION_CONTROL_COMMAND":
                AfterMessageReceiveEvent?.Invoke(this, new AfterMessageReceiveEventArgs(
                    JsonSerializer.Deserialize<CompetitionControlCommandMessage>(text)
                    ?? throw new Exception("failed to deserialize CompetitionControlCommandMessage")
                ));
                break;

            case "HOST_CONFIGURATION_FROM_CLIENT":
                AfterMessageReceiveEvent?.Invoke(this, new AfterMessageReceiveEventArgs(
                    JsonSerializer.Deserialize<HostConfigurationFromClientMessage>(text)
                    ?? throw new Exception("failed to deserialize HostConfigurationFromClientMessage")
                ));
                break;

            default:
                throw new Exception($"invalid message type: {generalMessage.MessageType}");
        }
    }


    void StartWsServer()
    {
        Debug.Assert(_wsServer is not null);

        _wsServer.Start(socket =>
        {
            socket.OnOpen = () =>
            {
                _logger.Debug("Connection from {ClientIpAddress} opened.", socket.ConnectionInfo.ClientIpAddress);

                // Remove the socket if it already exists.
                _sockets.TryRemove(socket.ConnectionInfo.Id, out _);

                // Add the socket.
                _sockets.TryAdd(socket.ConnectionInfo.Id, socket);
            };

            socket.OnClose = () =>
            {
                _logger.Debug("Connection from {ClientIpAddress} closed.", socket.ConnectionInfo.ClientIpAddress);

                // Remove the socket.
                _sockets.TryRemove(socket.ConnectionInfo.Id, out _);
            };

            socket.OnMessage = text =>
            {
                try
                {
                    ParseMessage(text);
                }
                catch (Exception exception)
                {
                    _logger.Error($"Failed to parse message: {exception}");
                }
            };

            socket.OnBinary = bytes =>
            {
                try
                {
                    string text = Encoding.UTF8.GetString(bytes);
                    ParseMessage(text);
                }
                catch (Exception exception)
                {
                    _logger.Error($"Failed to parse message: {exception}");
                }
            };

            socket.OnError = exception =>
            {
                _logger.Error("Socket error.");

                // Close the socket.
                socket.Close();

                // Remove the socket.
                _sockets.TryRemove(socket.ConnectionInfo.Id, out _);
            };
        });
    }
}
