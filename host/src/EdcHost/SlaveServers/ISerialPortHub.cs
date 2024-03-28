namespace EdcHost.SlaveServers;

public interface ISerialPortHub
{
    List<string> PortNames { get; }

    ISerialPortWrapper Get(string portName, int baudRate);
}
