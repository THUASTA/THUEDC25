namespace EdcHost.Games;

public class PlayerPickUpEventArgs : EventArgs
{
    /// <summary>
    /// The player that dig.
    /// </summary>
    public IPlayer Player { get; }

    /// <summary>
    /// The type of the mine that is picked up.
    /// </summary>
    public IMine.OreKindType MineType { get; }

    /// <summary>
    /// The count of item
    /// </summary>
    public int ItemCount { get; }

    /// <summary>
    /// The id of the mine
    /// </summary>
    public string MineId { get; }
    public PlayerPickUpEventArgs(IPlayer player, IMine.OreKindType mineType, int itemCount, string mineId)
    {
        Player = player;
        MineType = mineType;
        ItemCount = itemCount;
        MineId = mineId;
    }
}
