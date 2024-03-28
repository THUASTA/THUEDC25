namespace EdcHost.Games;

public class AfterGameStartEventArgs : EventArgs
{
    /// <summary>
    /// The game.
    /// </summary>
    public IGame Game { get; }

    public AfterGameStartEventArgs(IGame game)
    {
        Game = game;
    }
}
