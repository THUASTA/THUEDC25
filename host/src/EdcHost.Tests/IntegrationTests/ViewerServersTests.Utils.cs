using System.Collections.Concurrent;
using System.Text;
using EdcHost.ViewerServers;
using Fleck;

namespace EdcHost.Tests.IntegrationTests;

public partial class ViewerServersTests
{
    class WebSocketServerHubMock : IWebSocketServerHub
    {
        public Dictionary<int, IWebSocketServer> Servers { get; init; } = new();

        public IWebSocketServer Get(int port)
        {
            if (!Servers.ContainsKey(port))
            {
                throw new ArgumentException($"no server on port {port}");
            }

            return Servers[port];
        }
    }

    class WebSocketServerMock : IWebSocketServer
    {
        public readonly ConcurrentBag<IWebSocketConnection> Connections = new();
        readonly List<Action<IWebSocketConnection>> _configs = new();
        readonly int _port;

        public bool IsDisposed { get; set; } = false;
        public bool IsStarted { get; set; } = false;

        public WebSocketServerMock(int port)
        {
            _port = port;
        }

        public void Start(Action<IWebSocketConnection> config)
        {
            _configs.Add(config);
            IsStarted = true;
        }

        public void Dispose()
        {
            IsDisposed = true;
        }

        public void AddConnection(IWebSocketConnection connection)
        {
            Connections.Add(connection);
            foreach (Action<IWebSocketConnection> config in _configs)
            {
                config(connection);
            }
        }
    }

    class WebSocketConnectionMock : IWebSocketConnection
    {
        public readonly List<byte> SendBuffer = new();

        public Action OnOpen { get; set; } = () => { };
        public Action OnClose { get; set; } = () => { };
        public Action<string> OnMessage { get; set; } = (message) => { };
        public Action<byte[]> OnBinary { get; set; } = (message) => { };
        public Action<byte[]> OnPing { get; set; } = (message) => { };
        public Action<byte[]> OnPong { get; set; } = (message) => { };
        public Action<Exception> OnError { get; set; } = (exception) => { };
        public IWebSocketConnectionInfo ConnectionInfo { get; set; } = new WebSocketConnectionInfoMock();
        public bool IsAvailable { get; set; } = true;
        public Task Send(string message)
        {
            SendBuffer.Clear();
            SendBuffer.AddRange(Encoding.UTF8.GetBytes(message));
            return Task.CompletedTask;
        }
        public Task Send(byte[] message)
        {
            SendBuffer.Clear();
            SendBuffer.AddRange(message);
            return Task.CompletedTask;
        }
        public Task SendPing(byte[] message)
        {
            SendBuffer.Clear();
            SendBuffer.AddRange(message);
            return Task.CompletedTask;
        }
        public Task SendPong(byte[] message)
        {
            SendBuffer.Clear();
            SendBuffer.AddRange(message);
            return Task.CompletedTask;
        }
        public void Close()
        {
            IsAvailable = false;
        }
        public void Close(int code)
        {
            IsAvailable = false;
        }
    }

    class WebSocketConnectionInfoMock : IWebSocketConnectionInfo
    {
        public string SubProtocol { get; set; } = "";
        public string Origin { get; set; } = "";
        public string Host { get; set; } = "";
        public string Path { get; set; } = "";
        public string ClientIpAddress { get; set; } = "";
        public int ClientPort { get; set; } = 0;
        public IDictionary<string, string> Cookies { get; set; } = new Dictionary<string, string>();
        public IDictionary<string, string> Headers { get; set; } = new Dictionary<string, string>();
        public Guid Id { get; set; } = Guid.Empty;
        public string NegotiatedSubProtocol { get; set; } = "";
    }
}
