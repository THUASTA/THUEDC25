namespace EdcHost.Games;

public class AfterGameTickEventArgs : EventArgs
{
    /// <summary>
    /// The game.
    /// </summary>
    public IGame Game { get; }

    /// <summary>
    /// Start time of the game.
    /// </summary>
    public int CurrentTick { get; }

    public AfterGameTickEventArgs(IGame game, int currentTick)
    {
        Game = game;
        CurrentTick = currentTick;
    }
}
