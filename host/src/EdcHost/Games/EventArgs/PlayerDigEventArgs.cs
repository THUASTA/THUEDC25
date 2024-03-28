namespace EdcHost.Games;

public class PlayerDigEventArgs : EventArgs
{
    /// <summary>
    /// The player that dig.
    /// </summary>
    public IPlayer Player { get; }
    public int TargetChunk { get; }

    public PlayerDigEventArgs(IPlayer player, int targetChunk)
    {
        Player = player;
        TargetChunk = targetChunk;
    }
}
