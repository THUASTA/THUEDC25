namespace EdcHost.SlaveServers;

public interface IPacketFromHost : IPacket
{
    public int GameStage { get; }
    public int ElapsedTime { get; }
    public List<int> InformationOfChunks { get; }
    public bool HasBed { get; }
    public bool HasBedOpponent { get; }
    public float PositionX { get; }
    public float PositionY { get; }
    public float PositionOpponentX { get; }
    public float PositionOpponentY { get; }
    public int Agility { get; }
    public int Health { get; }
    public int MaxHealth { get; }
    public int Strength { get; }
    public int EmeraldCount { get; }
    public int WoolCount { get; }
}
