using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO.Ports;

namespace EdcHost.SlaveServers;

class SerialPortWrapper : ISerialPortWrapper
{
    public event EventHandler<ISerialPortWrapper.AfterReceiveEventArgs>? AfterReceive;

    public string PortName => _serialPort.PortName;
    public int BaudRate => _serialPort.BaudRate;

    const int FrequencyOfReceiving = 20;
    readonly Serilog.ILogger _logger = Serilog.Log.Logger.ForContext("Component", "SlaveServers");
    readonly ConcurrentQueue<byte[]> _queueOfBytesToSend = new();
    readonly SerialPort _serialPort;
    bool _shouldOpen = false;
    Task? _taskForReceiving = null;
    Task? _taskForSending = null;

    public SerialPortWrapper(string portName, int baudRate)
    {
        _serialPort = new(portName: portName, baudRate: baudRate);
    }

    public int BytesToRead => _serialPort.BytesToRead;

    public void Close()
    {
        if (!_serialPort.IsOpen)
        {
            throw new InvalidOperationException("port is not open");
        }

        Debug.Assert(_taskForReceiving != null);
        Debug.Assert(_taskForSending != null);

        _shouldOpen = false;

        _taskForReceiving.Wait();
        _taskForSending.Wait();

        _taskForSending.Dispose();
        _taskForReceiving.Dispose();

        try
        {
            _serialPort.Close();
        }
        catch (Exception)
        {
            _logger.Error("Wtf");
        }
    }

    public void Dispose()
    {
        _serialPort.Dispose();
        _taskForReceiving?.Dispose();
        _taskForSending?.Dispose();
    }

    public void Open()
    {
        if (_serialPort.IsOpen)
        {
            throw new InvalidOperationException("port is already open");
        }

        _shouldOpen = true;
        _serialPort.Open();

        _taskForReceiving = Task.Run(TaskForReceivingFunc);
        _taskForSending = Task.Run(TaskForSendingFunc);
    }

    public void Send(byte[] bytes)
    {
        if (!_serialPort.IsOpen)
        {
            throw new InvalidOperationException("port is not open");
        }

        _queueOfBytesToSend.Enqueue(bytes);
    }

    private void TaskForReceivingFunc()
    {
        DateTime lastTickStartTime = DateTime.Now;

        while (_shouldOpen == true && _serialPort.IsOpen)
        {
            // Wait for next tick
            DateTime currentTickStartTime = lastTickStartTime.AddMilliseconds(
                (double)1000 / FrequencyOfReceiving);
            if (currentTickStartTime > DateTime.Now)
            {
                Task.Delay(currentTickStartTime - DateTime.Now).Wait();
            }
            lastTickStartTime = DateTime.Now;

            try
            {
                if (_serialPort.BytesToRead == 0)
                {
                    continue;
                }

                byte[] bytes = new byte[_serialPort.BytesToRead];
                _serialPort.Read(bytes, 0, bytes.Length);

                AfterReceive?.Invoke(this, new(_serialPort.PortName, bytes));
            }
            catch (Exception)
            {
                _logger.Error("Error while receiving bytes.");

#if DEBUG
                throw;
#endif
            }
        }
    }

    private void TaskForSendingFunc()
    {
        while (_shouldOpen == true && _serialPort.IsOpen)
        {
            try
            {
                if (_queueOfBytesToSend.TryDequeue(out byte[]? bytes))
                {
                    _serialPort.Write(bytes, 0, bytes.Length);
                }
            }
            catch (Exception)
            {
                _logger.Error("Error while sending bytes.");

#if DEBUG
                throw;
#endif
            }
        }
    }
}
