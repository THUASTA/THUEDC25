namespace EdcHost;

partial class EdcHost : IEdcHost
{
    void HandleAfterJudgementEvent(object? sender, Games.AfterJudgementEventArgs e)
    {
        if (e.Winner is null)
        {
            _logger.Information("No winner.");
        }
        else
        {
            _logger.Information($"Winner is {e.Winner?.PlayerId}");
        }

        _gameRunner.End();
    }
    void HandlePlayerDigEvent(object? sender, Games.PlayerDigEventArgs e)
    {
        // Store the event info to the queue
        _playerEventQueue.Enqueue(e);
    }
    void HandlePlayerAttackEvent(object? sender, Games.PlayerAttackEventArgs e)
    {
        // Store the event info to the queue
        _playerEventQueue.Enqueue(e);
    }
    void HandlePlayerPlaceEvent(object? sender, Games.PlayerPlaceEventArgs e)
    {
        // Store the event info to the queue
        _playerEventQueue.Enqueue(e);
    }
}
