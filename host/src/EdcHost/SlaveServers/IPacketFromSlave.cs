namespace EdcHost.SlaveServers;

public interface IPacketFromSlave : IPacket
{
    public int ActionType { get; }
    public int Param { get; }
}
