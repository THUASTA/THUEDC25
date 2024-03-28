using EdcHost.Games;
using EdcHost.SlaveServers;

namespace EdcHost;

partial class EdcHost : IEdcHost
{
    void HandlePlayerTryAttackEvent(object? sender, PlayerTryAttackEventArgs e)
    {
        try
        {
            // Store the event info to the queue
            _playerEventQueue.Enqueue(e);

            string portName = e.PortName;

            int? playerId = _playerHardwareInfo
                .Where(kvp => kvp.Value.PortName == portName)
                .Select(kvp => (int?)kvp.Key)
                .FirstOrDefault((int?)null);

            if (playerId is null)
            {
                return;
            }

            IPosition<float> current = _game.Players[playerId.Value].PlayerPosition;
            _game.Players[playerId.Value].Attack(e.TargetChunkId % MapWidth, e.TargetChunkId / MapWidth);

            _logger.Information($"Player {playerId} attempted to attack ({e.TargetChunkId % MapWidth}, {e.TargetChunkId / MapWidth})");
        }
        catch (Exception ex)
        {
            _logger.Error($"PlayerTryAttack failed: {ex.Message}");
        }
    }

    void HandlePlayerTryPlaceBlockEvent(object? sender, PlayerTryPlaceBlockEventArgs e)
    {
        try
        {
            // Store the event info to the queue
            _playerEventQueue.Enqueue(e);

            string portName = e.PortName;

            int? playerId = _playerHardwareInfo
                .Where(kvp => kvp.Value.PortName == portName)
                .Select(kvp => (int?)kvp.Key)
                .FirstOrDefault((int?)null);

            if (playerId is null)
            {
                return;
            }

            IPosition<float> current = _game.Players[playerId.Value].PlayerPosition;
            _game.Players[playerId.Value].Place(e.TargetChunkId % MapWidth, e.TargetChunkId / MapWidth);

            _logger.Information($"Player {playerId} attempted to place a block at ({e.TargetChunkId % MapWidth}, {e.TargetChunkId / MapWidth})");
        }
        catch (Exception ex)
        {
            _logger.Error($"PlayerTryPlaceBlock failed: {ex.Message}");
        }
    }

    void HandlePlayerTryTradeEvent(object? sender, PlayerTryTradeEventArgs e)
    {
        try
        {
            // Store the event info to the queue
            _playerEventQueue.Enqueue(e);

            string portName = e.PortName;

            int? playerId = _playerHardwareInfo
                .Where(kvp => kvp.Value.PortName == portName)
                .Select(kvp => (int?)kvp.Key)
                .FirstOrDefault((int?)null);

            if (playerId is null)
            {
                return;
            }

            switch ((ItemKind)e.Item)
            {
                case ItemKind.AgilityBoost:
                    _game.Players[playerId.Value].SendTradeRequest(IPlayer.CommodityKindType.AgilityBoost);
                    break;

                case ItemKind.HealthBoost:
                    _game.Players[playerId.Value].SendTradeRequest(IPlayer.CommodityKindType.HealthBoost);
                    break;

                case ItemKind.StrengthBoost:
                    _game.Players[playerId.Value].SendTradeRequest(IPlayer.CommodityKindType.StrengthBoost);
                    break;

                case ItemKind.Wool:
                    _game.Players[playerId.Value].SendTradeRequest(IPlayer.CommodityKindType.Wool);
                    break;

                case ItemKind.PotionOfHealing:
                    _game.Players[playerId.Value].SendTradeRequest(IPlayer.CommodityKindType.HealthPotion);
                    break;

                default:
                    _logger.Error($"No item with id {e.Item}. PlayerTryTrade rejected."); // Do not throw exception here.
                    break;
            }

            _logger.Information($"Player {playerId} attempted to buy a item of {(ItemKind)e.Item}");
        }
        catch (Exception ex)
        {
            _logger.Error($"PlayerTryTrade failed: {ex.Message}");
        }
    }
}
