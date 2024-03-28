namespace EdcHost.SlaveServers;

public class PlayerTryTradeEventArgs : System.EventArgs
{
    public string PortName { get; }
    public int Item { get; }

    public PlayerTryTradeEventArgs(string portName, int item)
    {
        PortName = portName;
        Item = item;
    }
}
