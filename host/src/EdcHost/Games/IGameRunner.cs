namespace EdcHost.Games;

public interface IGameRunner
{
    static IGameRunner Create(IGame game)
    {
        return new GameRunner(game);
    }

    double ActualTps { get; }
    IGame Game { get; }

    void Start();
    void End();

    bool IsRunning { get; }
}
