namespace EdcHost.Games;

partial class Game : IGame
{
    /// <summary>
    /// Raised after game starts.
    /// </summary>
    public event EventHandler<AfterGameStartEventArgs>? AfterGameStartEvent;

    /// <summary>
    /// Raised after a new game tick.
    /// </summary>
    public event EventHandler<AfterGameTickEventArgs>? AfterGameTickEvent;

    /// <summary>
    /// Raised after judjement.
    /// </summary>
    public event EventHandler<AfterJudgementEventArgs>? AfterJudgementEvent;
}
