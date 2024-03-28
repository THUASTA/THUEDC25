namespace EdcHost.Games;

public class PlayerDieEventArgs : EventArgs
{
    /// <summary>
    /// The player that moved.
    /// </summary>
    public IPlayer Player { get; }

    public PlayerDieEventArgs(IPlayer player)
    {
        Player = player;
    }
}
