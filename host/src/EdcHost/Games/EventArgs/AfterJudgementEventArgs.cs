namespace EdcHost.Games;

public class AfterJudgementEventArgs : EventArgs
{
    /// <summary>
    /// The game.
    /// </summary>
    public IGame Game { get; }

    /// <summary>
    /// Start time of the game.
    /// </summary>
    public IPlayer? Winner { get; }

    public AfterJudgementEventArgs(IGame game, IPlayer? winner)
    {
        Game = game;
        Winner = winner;
    }
}
