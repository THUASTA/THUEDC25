
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using EDCViewer.Messages;

namespace EDCViewer.Client
{

    public class Client : IClient
    {
        public event EventHandler<IMessage>? AfterMessageReceiveEvent;


        private const int BufferSize = 134217728;
        private const int ByteCountPeriod = 1000;
        private const int MessageQueueCapacity = 200;
        private const int MessageSendPeriod = 1;

        /// <summary>
        /// Gets the bandwidth (in Mbps) between the client and the server.
        /// </summary>
        public decimal BandWidth { get; private set; } = 0;

        /// <summary>
        /// Gets the latency between the client and the server.
        /// </summary>
        public decimal Latency { get; private set; }
        public ClientWebSocket ClientWebSocket { get; private set; }

        private int _byteCountInThisPeriod = 0;
        private readonly System.Timers.Timer _byteCountTimer;

        private readonly ConcurrentQueue<IMessage> _messageQueue = new();
        private readonly byte[] _receiveBuffer = new byte[BufferSize];
        private readonly Uri _uri;
        private readonly Task _receiveMessageTask;
        private readonly CancellationTokenSource _cancellationTokenSource = new();
        private bool _isWebsocketCancelled = false;
        public void CloseReceiveMessageTask()
        {
            _cancellationTokenSource.Cancel();
            _isWebsocketCancelled = true;
            //_receiveMessageTask.Dispose();
        }
        public Client(string host, int port)
        {
            _uri = new($"ws://{host}:{port}");

            ClientWebSocket = TryConnect(2000,3);
            if(ClientWebSocket is null)
            {
                UnityEngine.Debug.Log($"cannot connect ws://{host}:{port}");
                return;
            }

            Console.WriteLine("Connected to server");

            _byteCountTimer = new System.Timers.Timer(ByteCountPeriod);
            _byteCountTimer.Elapsed += (sender, e) =>
            {
                BandWidth = BandWidth * 0.5m + (decimal)_byteCountInThisPeriod * 8 / 1e3m / ByteCountPeriod * 0.5m;
                _byteCountInThisPeriod = 0;
            };
            _byteCountTimer.Start();

            _receiveMessageTask = Task.Run(() =>
            {
                //!_cancellationTokenSource.Token.IsCancellationRequested||
                while (!_isWebsocketCancelled)
                {
                    ReceiveMessage();
                }
                UnityEngine.Debug.Log("task end!");
            });

            Task.Run(() =>
            {
                while (!_isWebsocketCancelled)
                {
                    Thread.Sleep(MessageSendPeriod);
                    if (_messageQueue.TryDequeue(out IMessage? message))
                    {
                        ClientWebSocket.SendAsync(GetBuffer(message.JsonString), WebSocketMessageType.Text, true, CancellationToken.None);
                    }
                }
                UnityEngine.Debug.Log("task end!");
            });
        }


        public void Send(IMessage message)
        {
            if (_messageQueue.Count >= MessageQueueCapacity)
            {
                UnityEngine.Debug.LogError("Message queue overflow");
                return;
            }

            _messageQueue.Enqueue(message);
        }

        /// <summary>Get buffer from a byte array</summary>
        /// <param name="arr">byte array</param>
        /// <returns> ArraySegment<byte> </returns>
        private ArraySegment<byte> GetBuffer(byte[] array)
        {
            return new ArraySegment<byte>(array);
        }

        /// <summary>Get buffer from a string</summary>
        /// <param name="str">string</param>
        /// <returns> ArraySegment<byte> </returns>
        private ArraySegment<byte> GetBuffer(string str)
        {
            return GetBuffer(System.Text.Encoding.UTF8.GetBytes(str));
        }

        private void ReceiveMessage()
        {
            int count = 0;

            try
            {
                WebSocketReceiveResult result = ClientWebSocket.ReceiveAsync(new ArraySegment<byte>(_receiveBuffer), CancellationToken.None).Result;

                if (result.MessageType == WebSocketMessageType.Close)
                {
                    throw new Exception("Server closed connection");
                }

                count = result.Count;

            }
            catch (Exception e)
            {
                Console.WriteLine($"Failed to receive message: {e.Message}");
                ClientWebSocket = TryConnect(2000,3);
                return;
            }

            _byteCountInThisPeriod += count;

            if (count >= _receiveBuffer.Length)
            {
                UnityEngine.Debug.LogError("Buffer overflow");
                return;
            }

            try
            {
                IMessage message = Parser.Parse(
                  System.Text.Encoding.UTF8.GetString(_receiveBuffer[..count]));
                AfterMessageReceiveEvent?.Invoke(this, message);

            }
            catch (Exception e)
            {
                UnityEngine.Debug.Log($"{e.StackTrace}\nFailed to parse message: {e.Message}: {System.Text.Encoding.UTF8.GetString(_receiveBuffer[..Math.Min(1024, count)])}...");
            }
        }

        private ClientWebSocket TryConnect(int timeout,int tryConnectCount)
        {
            ClientWebSocket clientWebSocket = new();
            UnityEngine.Debug.Log($"Trying to connect to server at {_uri}...");

            int connectCount = 0;
            while (!_isWebsocketCancelled && tryConnectCount > connectCount)
            {
                try
                {
                    clientWebSocket.ConnectAsync(_uri, CancellationToken.None).Wait(timeout);
                    break;
                }
                catch (Exception e)
                {
                    UnityEngine.Debug.Log($"Failed to connect to server: {e.Message}");
                    clientWebSocket = null;
                }
                UnityEngine.Debug.Log("Retrying...");
                connectCount++;
            }

            if(tryConnectCount == connectCount)
            {
                clientWebSocket = null;
            }

            return clientWebSocket;
        }
    }

}
