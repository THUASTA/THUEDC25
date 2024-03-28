namespace EdcHost.Games;

/// <summary>
/// Game handles the game logic.
/// </summary>
public interface IGame
{
    static IGame Create(List<Tuple<int, int>>? diamondMines = null,
        List<Tuple<int, int>>? goldMines = null, List<Tuple<int, int>>? ironMines = null)
    {
        return new Game(
            diamondMines: diamondMines,
            goldMines: goldMines,
            ironMines: ironMines
        );
    }

    /// <summary>
    /// Stage of game.
    /// </summary>
    enum Stage
    {
        Ready,
        Running,
        Battling,
        Finished,
    }

    event EventHandler<AfterGameStartEventArgs>? AfterGameStartEvent;
    event EventHandler<AfterGameTickEventArgs>? AfterGameTickEvent;
    event EventHandler<AfterJudgementEventArgs>? AfterJudgementEvent;

    /// <summary>
    /// Current stage of the game.
    /// </summary>
    Stage CurrentStage { get; }

    /// <summary>
    /// Elapsed time of the game.
    /// </summary>
    int ElapsedTicks { get; }

    /// <summary>
    /// Winner of the game.
    /// </summary>
    /// <remarks>
    /// Winner can be null in case there is no winner.
    /// </remarks>
    IPlayer? Winner { get; }

    /// <summary>
    /// The players.
    /// </summary>
    List<IPlayer> Players { get; }

    /// <summary>
    /// The game map.
    /// </summary>
    IMap GameMap { get; }

    /// <summary>
    /// The mines.
    /// </summary>
    List<IMine> Mines { get; }

    /// <summary>
    /// Starts the game.
    /// </summary>
    void Start();

    /// <summary>
    /// Ends the game.
    /// </summary>
    void End();

    void Tick();

}
