using EdcHost.SlaveServers;

namespace EdcHost.Tests.IntegrationTests;

public partial class SlaveServersTests
{
    class SerialPortHubMock : ISerialPortHub
    {
        public Dictionary<string, SerialPortWrapperMock> SerialPorts = new();

        public List<string> PortNames => SerialPorts.Keys.ToList();

        public ISerialPortWrapper Get(string portName, int baudRate)
        {
            if (!SerialPorts.ContainsKey(portName))
            {
                throw new ArgumentException($"port name does not exist: {portName}");
            }

            return SerialPorts[portName];
        }
    }

    class SerialPortWrapperMock : ISerialPortWrapper
    {
        public bool IsOpen = false;
        public List<byte> WriteBuffer = new();
        public void MockReceive(byte[] bytes)
        {
            AfterReceive?.Invoke(this, new ISerialPortWrapper.AfterReceiveEventArgs(PortName, bytes));
        }

        public event EventHandler<ISerialPortWrapper.AfterReceiveEventArgs>? AfterReceive;

        public string PortName { get; }

        public int BaudRate => 115200;

        public SerialPortWrapperMock(string portName)
        {
            PortName = portName;
        }

        public void Close()
        {
            IsOpen = false;
        }
        public void Dispose()
        {
        }
        public void Open()
        {
            if (IsOpen)
            {
                throw new InvalidOperationException();
            }

            IsOpen = true;
        }
        public void Send(byte[] bytes)
        {
            if (!IsOpen)
            {
                throw new InvalidOperationException();
            }

            WriteBuffer.Clear();
            WriteBuffer.AddRange(bytes);
        }
    }

    static byte CalculateChecksum(byte[] bytes)
    {
        byte checksum = 0x00;
        foreach (byte byte_item in bytes)
        {
            checksum ^= byte_item;
        }
        return checksum;
    }

    static byte[] MakePacketHeader(byte[] data)
    {
        byte[] header = new byte[5];
        header[0] = 0x55;
        header[1] = 0xAA;
        BitConverter.GetBytes((short)data.Length).CopyTo(header, 2);
        header[4] = CalculateChecksum(data);
        return header;
    }

    static byte[] MakePacket(byte[] data)
    {
        byte[] header = MakePacketHeader(data);
        byte[] packet = new byte[header.Length + data.Length];
        header.CopyTo(packet, 0);
        data.CopyTo(packet, header.Length);
        return packet;
    }
}
