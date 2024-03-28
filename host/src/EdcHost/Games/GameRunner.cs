using System.Diagnostics;

namespace EdcHost.Games;

class GameRunner : IGameRunner
{
    const int TicksPerSecondExpected = 20;

    public bool IsRunning { get; private set; } = false;

    public double ActualTps => Game.CurrentStage is not IGame.Stage.Running ? 0 : 1000 / _lastTickDuration.TotalMilliseconds;
    public IGame Game { get; }

    TimeSpan _lastTickDuration = TimeSpan.MaxValue;
    Task? _task = null;

    public GameRunner(IGame game)
    {
        Game = game;
    }

    public void Start()
    {
        if (Game.CurrentStage is not IGame.Stage.Ready)
        {
            throw new InvalidOperationException($"Game is already at stage {Game.CurrentStage}");
        }

        IsRunning = true;

        Game.Start();

        _task = Task.Run(TaskFunc);
    }

    public void End()
    {
        if (Game.CurrentStage is not IGame.Stage.Running && Game.CurrentStage is not IGame.Stage.Battling)
        {
            throw new InvalidOperationException($"Game is already at stage {Game.CurrentStage}");
        }

        Debug.Assert(_task is not null);

        Game.End();
        IsRunning = false;
        _task.Wait();
    }

    void TaskFunc()
    {
        DateTime lastTickStartTime = DateTime.Now;

        while (IsRunning)
        {
            if (Game.CurrentStage is not IGame.Stage.Running && Game.CurrentStage is not IGame.Stage.Battling)
            {
                break;
            }

            // Wait for next tick
            DateTime currentTickStartTime = lastTickStartTime.AddMilliseconds((double)1000 / TicksPerSecondExpected);
            if (currentTickStartTime > DateTime.Now)
            {
                Task.Delay(currentTickStartTime - DateTime.Now).Wait();
            }
            currentTickStartTime = DateTime.Now;

            _lastTickDuration = DateTime.Now - lastTickStartTime;

            Game.Tick();

            lastTickStartTime = currentTickStartTime;
        }
    }
}
