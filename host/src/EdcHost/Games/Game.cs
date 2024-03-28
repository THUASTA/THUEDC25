using Serilog;

namespace EdcHost.Games;

/// <summary>
/// Game handles the game logic.
/// </summary>
partial class Game : IGame
{
    const int TickBattlingModeStart = 12000;
    public const int TicksPerSecondExpected = 20;
    const int TicksBattlingDamageInterval = 100;
    const int BattlingDamage = 1;

    /// <summary>
    /// Current stage of the game.
    /// </summary>
    public IGame.Stage CurrentStage { get; private set; } = IGame.Stage.Ready;

    /// <summary>
    /// Elapsed ticks of the game.
    /// </summary>
    public int ElapsedTicks { get; private set; } = 0;

    /// <summary>
    /// Winner of the game.
    /// </summary>
    /// <remarks>
    /// Winner can be null in case there is no winner.
    /// </remarks>
    public IPlayer? Winner { get; private set; } = null;

    /// <summary>
    /// The game map.
    /// </summary>
    public IMap GameMap { get; private set; }

    /// <summary>
    /// All mines.
    /// </summary>
    public List<IMine> Mines { get; private set; }

    /// <summary>
    /// Default players.
    /// </summary>
    readonly IPlayer[] _defaultPlayerList = {
        new Player(0, 0, 0, 0.5f, 0.5f),
        new Player(1, 7, 7, 7.5f, 7.5f)
    };

    /// <summary>
    /// Default spawn points.
    /// </summary>
    readonly IPosition<int>[] _defaultSpawnPoints = {
        new Position<int>(0, 0),
        new Position<int>(7, 7)
    };

    /// <summary>
    /// Used to lock some blocks.
    /// </summary>
    readonly object _gameLocker = new();

    readonly ILogger _logger = Log.Logger.ForContext("Component", "Games");

    public Game(
        List<Tuple<int, int>>? diamondMines = null,
        List<Tuple<int, int>>? goldMines = null,
        List<Tuple<int, int>>? ironMines = null,
        List<IPlayer>? playerList = null,
        IPosition<int>[]? spawnPoints = null
    )
    {
        spawnPoints ??= _defaultSpawnPoints;
        playerList ??= new(_defaultPlayerList);

        PlayerNum = playerList.Count;

        if (PlayerNum != spawnPoints.Length)
        {
            throw new ArgumentException(
                "Number of players does not match number of spawn points");
        }

        GameMap = new Map(spawnPoints);

        _playerLastAttackTickList = new();
        for (int i = 0; i < PlayerNum; i++)
        {
            _playerLastAttackTickList.Add(Never);
        }

        _playerDeathTickList = new();
        for (int i = 0; i < PlayerNum; i++)
        {
            _playerDeathTickList.Add(null);
        }

        _playerEventQueue = new();
        _playerEventQueue.Clear();

        Mines = new();
        GenerateMines(
            diamondMines: diamondMines,
            goldMines: goldMines,
            ironMines: ironMines
        );

        _isAllBedsDestroyed = false;

        Players = new(playerList);

        for (int i = 0; i < PlayerNum; i++)
        {
            Players[i].OnMove += EnqueueEvent;
            Players[i].OnAttack += EnqueueEvent;
            Players[i].OnPlace += EnqueueEvent;
            Players[i].OnDie += EnqueueEvent;
            Players[i].OnTrade += EnqueueEvent;
        }

        for (int i = 0; i < PlayerNum; i++)
        {
            _playerLastAttackTickList[i] = Never;
        }

        for (int i = 0; i < PlayerNum; i++)
        {
            _playerDeathTickList[i] = null;
        }

        Winner = null;

        _isAllBedsDestroyed = false;
    }

    public void Start()
    {
        _logger.Information("Starting...");

        if (CurrentStage != IGame.Stage.Ready)
        {
            throw new InvalidOperationException("the game has already started");
        }

        _playerEventQueue.Clear();

        CurrentStage = IGame.Stage.Running;
        AfterGameStartEvent?.Invoke(this, new AfterGameStartEventArgs(this));

        _logger.Information("Started.");
    }

    public void End()
    {
        _logger.Information("Ending...");

        if (CurrentStage != IGame.Stage.Running && CurrentStage != IGame.Stage.Battling)
        {
            throw new InvalidOperationException("the game is not running");
        }

        CurrentStage = IGame.Stage.Finished;

        _logger.Information("Ended.");
    }

    public void Tick()
    {
        ++ElapsedTicks;

        try
        {
            lock (_gameLocker)
            {
                HandlePlayerEvents();

                if (IsFinished())
                {
                    Judge();
                    CurrentStage = IGame.Stage.Finished;
                    return;
                }

                if (ElapsedTicks > TickBattlingModeStart && CurrentStage == IGame.Stage.Running)
                {
                    CurrentStage = IGame.Stage.Battling;
                }

                if (CurrentStage == IGame.Stage.Battling)
                {
                    if (_isAllBedsDestroyed == false)
                    {
                        for (int i = 0; i < PlayerNum; i++)
                        {
                            Players[i].DestroyBed();
                        }
                        _isAllBedsDestroyed = true;
                    }

                    if ((ElapsedTicks - TickBattlingModeStart) % TicksBattlingDamageInterval == 0)
                    {
                        for (int i = 0; i < PlayerNum; i++)
                        {
                            Players[i].Hurt(BattlingDamage);
                        }
                    }
                }

                UpdatePlayerInfo();
                UpdateMines();
            }

            AfterGameTickEvent?.Invoke(
                this, new AfterGameTickEventArgs(this, ElapsedTicks));
        }
        catch (Exception ex)
        {
            _logger.Fatal($"An unhandled exception occurred at tick {ElapsedTicks}: {ex}");
        }
    }

    void HandlePlayerEvents()
    {
        while (_playerEventQueue.IsEmpty == false)
        {
            if (_playerEventQueue.TryDequeue(out EventArgs? playerEvent) && playerEvent is not null)
            {
                switch (playerEvent)
                {
                    case PlayerAttackEventArgs attackEvent:
                        HandlePlayerAttackEvent(attackEvent);
                        break;
                    case PlayerMoveEventArgs moveEvent:
                        HandlePlayerMoveEvent(moveEvent);
                        break;
                    case PlayerPlaceEventArgs placeEvent:
                        HandlePlayerPlaceEvent(placeEvent);
                        break;
                    case PlayerDieEventArgs dieEvent:
                        HandlePlayerDieEvent(dieEvent);
                        break;
                    case PlayerTradeEventArgs tradeEvent:
                        HandlePlayerTradeEvent(tradeEvent);
                        break;

                    default:
                        _logger.Error($"Unknown event: {playerEvent}");
                        break;
                }
            }
        }
    }

    void UpdatePlayerInfo()
    {
        for (int i = 0; i < PlayerNum; i++)
        {
            if (Players[i].HasBed == true
                && GameMap.GetChunkAt(Players[i].SpawnPoint).IsVoid == true)
            {
                Players[i].DestroyBed();
            }

            if (Players[i].IsAlive == false && Players[i].HasBed == true
                && ElapsedTicks - _playerDeathTickList[i] > TicksBeforeRespawn
                && IsSamePosition(
                    ToIntPosition(Players[i].PlayerPosition), Players[i].SpawnPoint
                    ) == true)
            {
                Players[i].Spawn(Players[i].MaxHealth);
                _playerDeathTickList[i] = null;

            }

            if (Players[i].IsAlive == true && IsValidPosition(
                ToIntPosition(Players[i].PlayerPosition)) == false)
            {
                Players[i].Hurt(InstantDeathDamage);
            }
            else if (Players[i].IsAlive == true && GameMap.GetChunkAt(
                ToIntPosition(Players[i].PlayerPosition)).IsVoid == true)
            {
                Players[i].Hurt(InstantDeathDamage);
            }
        }
    }

    /// <summary>
    /// Update mines.
    /// </summary>
    void UpdateMines()
    {
        foreach (IMine mine in Mines)
        {
            if (CurrentStage == IGame.Stage.Running
                && ElapsedTicks - mine.LastOreGeneratedTick >= mine.AccumulateOreInterval)
            {
                mine.GenerateOre(ElapsedTicks);
            }

            List<int> existingPlayerId = new();
            existingPlayerId.Clear();
            for (int i = 0; i < PlayerNum; i++)
            {
                if (Players[i].IsAlive == true
                    && IsSamePosition(
                        ToIntPosition(Players[i].PlayerPosition), mine.Position
                        ) == true)
                {
                    existingPlayerId.Add(Players[i].PlayerId);
                }
            }

            // Collect ore only if there is just one player.
            if (existingPlayerId.Count == 1)
            {
                int playerId = existingPlayerId[0];
                //Remaining capacity of a player
                int capacity = MaximumItemCount - Players[playerId].EmeraldCount;

                //Value of an ore
                int value = mine.OreKind switch
                {
                    IMine.OreKindType.IronIngot => 1,
                    IMine.OreKindType.GoldIngot => 4,
                    IMine.OreKindType.Diamond => 16,
                    _ => throw new ArgumentOutOfRangeException(nameof(mine.OreKind), "No such ore kind.")
                };

                //Collected ore count
                int collectedOre = Math.Min(capacity / value, mine.AccumulatedOreCount);

                Players[playerId].EmeraldAdd(collectedOre * value);
                // Invoke the event
                Players[playerId].PickUpEventInvoker(mine.OreKind, collectedOre, mine.MineId.ToString());

                mine.PickUpOre(collectedOre);
            }
        }
    }

    /// <summary>
    /// Whether the game is finished or not.
    /// </summary>
    /// <returns>True if finished, false otherwise.</returns>
    bool IsFinished()
    {
        int remainingPlayers = PlayerNum;
        for (int i = 0; i < PlayerNum; i++)
        {
            if (Players[i].IsAlive == false && Players[i].HasBed == false)
            {
                remainingPlayers--;
            }
        }
        return remainingPlayers <= 1;
    }

    /// <summary>
    /// Judge the game. Choose a winner or report there is no winner.
    /// </summary>
    void Judge()
    {
        int remainingPlayers = 0;
        for (int i = 0; i < PlayerNum; i++)
        {
            if (Players[i].IsAlive == true || Players[i].HasBed == true)
            {
                remainingPlayers++;
            }
        }

        if (remainingPlayers == 0 || remainingPlayers > 1)
        {
            Winner = null;
        }
        else
        {
            for (int i = 0; i < PlayerNum; i++)
            {
                if (Players[i].IsAlive == true || Players[i].HasBed == true)
                {
                    Winner = Players[i];
                    break;
                }
            }
        }

        AfterJudgementEvent?.Invoke(this, new AfterJudgementEventArgs(this, Winner));
    }
}
