namespace EdcHost.SlaveServers;

public class PlayerTryPlaceBlockEventArgs : System.EventArgs
{
    public string PortName { get; }
    public int TargetChunkId { get; }

    public PlayerTryPlaceBlockEventArgs(string portName, int targetChunkId)
    {
        PortName = portName;
        TargetChunkId = targetChunkId;
    }
}
