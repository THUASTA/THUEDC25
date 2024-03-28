namespace EdcHost.Games;

partial class Game : IGame
{
    /// <summary>
    /// Enqueue a event.
    /// </summary>
    /// <param name="sender"Sender of the event></param>
    /// <param name="e">Event args</param>
    void EnqueueEvent(object? sender, EventArgs e)
    {
        if (CurrentStage != IGame.Stage.Running && CurrentStage != IGame.Stage.Battling)
        {
            return;
        }

        _playerEventQueue.Enqueue(e);
    }

    /// <summary>
    /// Handle PlayerMoveEvent
    /// </summary>
    /// <param name="e">Event args</param>
    void HandlePlayerMoveEvent(PlayerMoveEventArgs e)
    {
        if (CurrentStage != IGame.Stage.Running && CurrentStage != IGame.Stage.Battling)
        {
            // Do not handle player events when game is not running
            return;
        }

        try
        {
            if (e.Player.IsAlive == false && e.Player.HasBed == true
                && ElapsedTicks - _playerDeathTickList[e.Player.PlayerId] > TicksBeforeRespawn
                && IsSamePosition(
                    ToIntPosition(e.Position), e.Player.SpawnPoint
                    ) == true)
            {
                Players[e.Player.PlayerId].Spawn(e.Player.MaxHealth);
                _playerDeathTickList[e.Player.PlayerId] = null;
            }

            //Kill fallen player. Use 'if' instead of 'else if' to avoid fake spawn.
            if (e.Player.IsAlive == true && IsValidPosition(ToIntPosition(e.Position)) == false)
            {
                Players[e.Player.PlayerId].Hurt(InstantDeathDamage);
                return;
            }
            if (e.Player.IsAlive == true && GameMap.GetChunkAt(ToIntPosition(e.Position)).IsVoid == true)
            {
                Players[e.Player.PlayerId].Hurt(InstantDeathDamage);
                return;
            }

        }
        catch (Exception exception)
        {
            _logger.Error($"Failed to move: {exception}");
        }
    }

    /// <summary>
    /// Handle PlayerAttackEvent
    /// </summary>
    /// <param name="e">Event args</param>
    void HandlePlayerAttackEvent(PlayerAttackEventArgs e)
    {
        if (CurrentStage != IGame.Stage.Running && CurrentStage != IGame.Stage.Battling)
        {
            return;
        }

        if (e.Player.IsAlive == false)
        {
            _logger.Error($"Player {e.Player.PlayerId} is dead. PlayerAttack rejected.");
            return;
        }
        if (ElapsedTicks - _playerLastAttackTickList[e.Player.PlayerId] < AttackTickInterval(e.Player))
        {
            _logger.Error(@$"Player {e.Player.PlayerId} has already attacked recently.
                PlayerAttack rejected.");
            return;
        }
        if (IsAdjacent(ToIntPosition(e.Player.PlayerPosition), e.Position) == false)
        {
            _logger.Error(@$"Position ({e.Position.X}, {e.Position.Y})
                is not adjacent to player {e.Player.PlayerId}. PlayerAttack rejected.");
            return;
        }
        if (IsValidPosition(e.Position) == false)
        {
            _logger.Error(@$"Position ({e.Position.X}, {e.Position.Y}) is not valid.
                PlayerAttack rejected.");
            return;
        }

        bool attacked = false;
        for (int i = 0; i < PlayerNum; i++)
        {
            if (Players[i].PlayerId != e.Player.PlayerId && Players[i].IsAlive == true
                && IsSamePosition(e.Position,
                    ToIntPosition(Players[i].PlayerPosition)) == true)
            {
                Players[i].Hurt(e.Player.Strength);
                attacked = true;
            }
        }

        if (attacked == true)
        {
            _playerLastAttackTickList[e.Player.PlayerId] = ElapsedTicks;
        }
        else
        {
            if (GameMap.GetChunkAt(e.Position).CanRemoveBlock == false)
            {
                _logger.Error("Target chunk is empty. PlayerAttack rejected.");
                return;
            }

            try
            {
                GameMap.GetChunkAt(e.Position).RemoveBlock();
                _playerLastAttackTickList[e.Player.PlayerId] = ElapsedTicks;

                if (GameMap.GetChunkAt(e.Position).IsVoid == true)
                {
                    for (int i = 0; i < PlayerNum; i++)
                    {
                        if (Players[i].HasBed == true && IsSamePosition(
                            Players[i].SpawnPoint, e.Position) == true)
                        {
                            Players[i].DestroyBed();
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                _logger.Error($"Failed to dig: {exception}");
                return;
            }
        }
    }

    /// <summary>
    /// Handle PlayerPlaceEvent
    /// </summary>
    /// <param name="e">Event args</param>
    void HandlePlayerPlaceEvent(PlayerPlaceEventArgs e)
    {
        if (CurrentStage != IGame.Stage.Running && CurrentStage != IGame.Stage.Battling)
        {
            return;
        }

        if (e.Player.IsAlive == false)
        {
            _logger.Error($"Player {e.Player.PlayerId} is dead. PlayerPlace rejected.");
            return;
        }
        if (e.Player.WoolCount <= 0)
        {
            _logger.Error($"Player {e.Player.PlayerId} doesn't have enough wool. PlayerPlace rejected.");
        }
        if (IsAdjacent(ToIntPosition(e.Player.PlayerPosition), e.Position) == false)
        {
            _logger.Error(@$"Position ({e.Position.X}, {e.Position.Y})
                is not adjecant to player {e.Player.PlayerId}. PlayerPlace rejected.");
            return;
        }
        if (IsValidPosition(e.Position) == false)
        {
            _logger.Error(@$"Position ({e.Position.X}, {e.Position.Y}) is not valid.
                PlayerPlace rejected.");
            return;
        }

        try
        {
            if (GameMap.GetChunkAt(e.Position).CanPlaceBlock == true)
            {
                GameMap.GetChunkAt(e.Position).PlaceBlock();
                Players[e.Player.PlayerId].DecreaseWoolCount();
            }
            else
            {
                _logger.Error(@$"The chunk at position ({e.Position.X}, {e.Position.Y})
                    has already reached its maximum height. PlayerPlace rejected.");
            }
        }
        catch (Exception exception)
        {
            _logger.Error($"Failed to place wool: {exception}");
            return;
        }
    }

    /// <summary>
    /// Handle PlayerDieEvent
    /// </summary>
    /// <param name="e">Event args</param>
    void HandlePlayerDieEvent(PlayerDieEventArgs e)
    {
        if (CurrentStage != IGame.Stage.Running && CurrentStage != IGame.Stage.Battling)
        {
            return;
        }

        if (_playerDeathTickList[e.Player.PlayerId] is not null)
        {
            _logger.Error($"Player {e.Player.PlayerId} is already dead. PlayerDie rejected.");
            return;
        }

        _playerDeathTickList[e.Player.PlayerId] = ElapsedTicks;
    }

    /// <summary>
    /// Handle PlayerTradeEvent
    /// </summary>
    /// <param name="e">Event args</param>
    void HandlePlayerTradeEvent(PlayerTradeEventArgs e)
    {
        if (CurrentStage != IGame.Stage.Running)
        {
            _logger.Error($"Failed to trade: Trade is allowed at stage Running");
            return;
        }
        if (Players[e.Player.PlayerId].IsAlive == false)
        {
            _logger.Error($"Failed to trade: Player {e.Player.PlayerId} is dead.");
            return;
        }
        if (IsSamePosition(
            ToIntPosition(Players[e.Player.PlayerId].PlayerPosition), Players[e.Player.PlayerId].SpawnPoint
            ) == false)
        {
            _logger.Error($"Failed to trade: Player {e.Player.PlayerId} is not at spawn point");
            return;
        }

        try
        {
            bool result = Players[e.Player.PlayerId].Trade(e.CommodityKind);
            if (result == false)
            {
                _logger.Error($"Failed to trade: Player {e.Player.PlayerId} cannot buy {e.CommodityKind}");
            }
        }
        catch (Exception ex)
        {
            _logger.Error($"Failed to trade: {ex}");
        }
    }
}
