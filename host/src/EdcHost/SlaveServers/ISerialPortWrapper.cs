namespace EdcHost.SlaveServers;

public interface ISerialPortWrapper : IDisposable
{
    class AfterReceiveEventArgs : EventArgs
    {
        public byte[] Bytes { get; }
        public string PortName { get; }

        public AfterReceiveEventArgs(string portName, byte[] bytes)
        {
            PortName = portName;
            Bytes = bytes;
        }
    }

    event EventHandler<AfterReceiveEventArgs> AfterReceive;

    string PortName { get; }
    int BaudRate { get; }

    void Close();
    void Open();
    void Send(byte[] bytes);
}
