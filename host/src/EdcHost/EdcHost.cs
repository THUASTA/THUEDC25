using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text.Json;
using Serilog;

namespace EdcHost;

partial class EdcHost : IEdcHost
{
    const int FrequencyOfReadingCamera = 60;
    const int FrequencyOfSendingToSlave = 20;
    const int FrequencyOfSendingToViewer = 60;
    const int MapHeight = 8;
    const int MapWidth = 8;

    readonly ILogger _logger = Log.ForContext("Component", "EdcHost");
    readonly ConcurrentQueue<EventArgs> _playerEventQueue = new();
    // Note: this info does not ensure that the player is connected.
    readonly ConcurrentDictionary<int, PlayerHardwareInfo> _playerHardwareInfo = new();
    readonly CameraServers.ICameraServer _cameraServer;
    readonly SlaveServers.ISlaveServer _slaveServer;
    readonly ViewerServers.IViewerServer _viewerServer;
    readonly Config _config;

    public bool IsRunning { get; private set; } = false;

    Games.IGame _game;
    Games.IGameRunner _gameRunner;
    CancellationTokenSource? _taskCancellationTokenSource = null;
    Task? _taskForReadingCamera = null;
    Task? _taskForSendingToSlave = null;
    Task? _taskForSendingToViewer = null;

    public EdcHost(Config config)
    {
        _config = config;

        _game = Games.IGame.Create(
            diamondMines: _config.Game.DiamondMines,
            goldMines: _config.Game.GoldMines,
            ironMines: _config.Game.IronMines
        );
        _gameRunner = Games.IGameRunner.Create(_game);
        _cameraServer = CameraServers.ICameraServer.Create();
        _slaveServer = SlaveServers.ISlaveServer.Create();

        _viewerServer = ViewerServers.IViewerServer.Create(_config.ServerPort);

        _game.AfterJudgementEvent += HandleAfterJudgementEvent;

        for (int i = 0; i < _game.Players.Count; i++)
        {
            _game.Players[i].OnAttack += HandlePlayerAttackEvent;
            _game.Players[i].OnPlace += HandlePlayerPlaceEvent;
            _game.Players[i].OnDig += HandlePlayerDigEvent;
        }

        _slaveServer.PlayerTryAttackEvent += HandlePlayerTryAttackEvent;
        _slaveServer.PlayerTryTradeEvent += HandlePlayerTryTradeEvent;
        _slaveServer.PlayerTryPlaceBlockEvent += HandlePlayerTryPlaceBlockEvent;

        _viewerServer.AfterMessageReceiveEvent += HandleAfterMessageReceiveEvent;

        _game.Players.ForEach(player => _playerHardwareInfo.GetOrAdd(player.PlayerId, new PlayerHardwareInfo()));
    }

    public void Start()
    {

        _logger.Information("Starting...");

        Debug.Assert(_taskCancellationTokenSource is null);
        Debug.Assert(_taskForReadingCamera is null);
        Debug.Assert(_taskForSendingToSlave is null);
        Debug.Assert(_taskForSendingToViewer is null);

        try
        {
            _cameraServer.Start();
            _slaveServer.Start();
            _viewerServer.Start();

            _taskCancellationTokenSource = new CancellationTokenSource();
            _taskForReadingCamera = Task.Run(TaskForReadingCameraFunc);
            _taskForSendingToSlave = Task.Run(TaskForSendingToSlaveFunc);
            _taskForSendingToViewer = Task.Run(TaskForSendingToViewerFunc);

            IsRunning = true;

            _logger.Information("Started.");
        }
        catch (Exception ex)
        {
            _logger.Fatal($"An unhandled exception is caught when starting EdcHost: {ex}");
        }

    }

    public void Stop()
    {
        _logger.Information("Stopping...");

        Debug.Assert(_taskCancellationTokenSource is not null);
        Debug.Assert(_taskForReadingCamera is not null);
        Debug.Assert(_taskForSendingToSlave is not null);
        Debug.Assert(_taskForSendingToViewer is not null);

        try
        {
            _taskCancellationTokenSource.Cancel();
            _taskForReadingCamera.Wait();
            _taskForSendingToSlave.Wait();
            _taskForSendingToViewer.Wait();

            _taskCancellationTokenSource.Dispose();
            _taskForReadingCamera.Dispose();

            _cameraServer.Stop();
            _slaveServer.Stop();
            _viewerServer.Stop();

            IsRunning = false;

            _logger.Information("Stopped.");
        }
        catch (Exception ex)
        {
            _logger.Warning($"An unhandled exception occurred while stopping EdcHost: {ex}");
        }
    }

    void TaskForReadingCameraFunc()
    {
        DateTime lastTickStartTime = DateTime.Now;

        while (!_taskCancellationTokenSource?.IsCancellationRequested ?? false)
        {
            try
            {
                // Wait for next tick
                DateTime currentTickStartTime = lastTickStartTime.AddMilliseconds(
                    (double)1000 / FrequencyOfReadingCamera);
                if (currentTickStartTime > DateTime.Now)
                {
                    Task.Delay(currentTickStartTime - DateTime.Now).Wait();
                }
                lastTickStartTime = DateTime.Now;

                foreach (Games.IPlayer player in _game.Players)
                {
                    // Skip if no hardware info is found
                    if (_playerHardwareInfo.TryGetValue(player.PlayerId, out PlayerHardwareInfo? playerHardwareInfo) is false)
                    {
                        continue;
                    }

                    // If the camera is not connected, skip this player
                    int? cameraIndex = playerHardwareInfo.CameraIndex;
                    if (cameraIndex is null)
                    {
                        continue;
                    }

                    CameraServers.ICamera camera = _cameraServer.GetCamera(cameraIndex.Value) ?? throw new Exception($"camera {cameraIndex} is not found");
                    Tuple<float, float>? location = camera.TargetLocation;
                    Games.IPosition<float>? position = location is null ? null : new Games.Position<float>(location.Item1 * MapWidth, location.Item2 * MapHeight);

                    if (position is null)
                    {
                        // This is a magic number that means the player is not found.
                        player.Move(float.MinValue, float.MinValue);
                    }
                    else
                    {
                        player.Move(position.X, position.Y);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Fatal($"An unhandled exception occurred while reading camera: {ex}");
            }
        }
    }

    void TaskForSendingToSlaveFunc()
    {
        DateTime lastTickStartTime = DateTime.Now;

        while (!_taskCancellationTokenSource?.IsCancellationRequested ?? false)
        {
            try
            {
                // Wait for next tick
                DateTime currentTickStartTime = lastTickStartTime.AddMilliseconds(
                    (double)1000 / FrequencyOfSendingToSlave);
                if (currentTickStartTime > DateTime.Now)
                {
                    Task.Delay(currentTickStartTime - DateTime.Now).Wait();
                }
                lastTickStartTime = DateTime.Now;

                List<int> heightOfChunks = new();
                foreach (Games.IChunk chunk in _game.GameMap.Chunks)
                {
                    heightOfChunks.Add(chunk.Height);
                }

                List<int> owningOreKindOfChunks = new();
                foreach (Games.IChunk chunk in _game.GameMap.Chunks)
                {
                    owningOreKindOfChunks.Add((int)chunk.OwningOreKind);
                }

                for (int i = 0; i < 2; i++)
                {
                    string? portName = _playerHardwareInfo.GetValueOrDefault(_game.Players[i].PlayerId)?.PortName;
                    if (portName is null)
                    {
                        continue;
                    }

                    try
                    {
                        _slaveServer.Publish(
                            portName: portName,
                            gameStage: (int)_game.CurrentStage,
                            elapsedTime: _game.ElapsedTicks,
                            heightOfChunks: heightOfChunks,
                            hasBed: _game.Players[i].HasBed,
                            hasBedOpponent: _game.Players.Any(player => player.HasBed && player.PlayerId != _game.Players[i].PlayerId),
                            positionX: (
                                _game.Players[i].PlayerPosition.X
                            ),
                            positionY: (
                                _game.Players[i].PlayerPosition.Y
                            ),
                            positionOpponentX: (
                                _game.Players[(i == 0) ? 1 : 0].IsAlive == true ?
                                _game.Players[(i == 0) ? 1 : 0].PlayerPosition.X :
                                float.MinValue
                            ),
                            positionOpponentY: (
                                _game.Players[(i == 0) ? 1 : 0].IsAlive == true ?
                                _game.Players[(i == 0) ? 1 : 0].PlayerPosition.Y :
                                float.MinValue
                            ),
                            agility: _game.Players[i].ActionPoints,
                            health: _game.Players[i].Health,
                            maxHealth: _game.Players[i].MaxHealth,
                            strength: _game.Players[i].Strength,
                            emeraldCount: _game.Players[i].EmeraldCount,
                            woolCount: _game.Players[i].WoolCount,
                            owningOreKindOfChunks: owningOreKindOfChunks
                        );
                    }
                    catch (Exception e)
                    {
                        _logger.Error($"failed to publish to slave via port {portName}: {e.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Fatal($"An unhandled exception occurred while sending to slave: {ex}");
            }
        }
    }

    void TaskForSendingToViewerFunc()
    {
        DateTime lastTickStartTime = DateTime.Now;

        while (!_taskCancellationTokenSource?.IsCancellationRequested ?? false)
        {
            try
            {
                // Wait for next tick
                DateTime currentTickStartTime = lastTickStartTime.AddMilliseconds(
                    (double)1000 / FrequencyOfSendingToViewer);
                if (currentTickStartTime > DateTime.Now)
                {
                    Task.Delay(currentTickStartTime - DateTime.Now).Wait();
                }
                lastTickStartTime = DateTime.Now;

                List<ViewerServers.CompetitionUpdateMessage.Camera> cameraInfoList = new();
                foreach (int cameraIndex in _cameraServer.AvailableCameraIndexes)
                {
                    CameraServers.ICamera? camera = _cameraServer.GetCamera(cameraIndex);
                    if (camera?.IsOpened ?? false)
                    {
                        cameraInfoList.Add(new ViewerServers.CompetitionUpdateMessage.Camera
                        {
                            cameraId = cameraIndex,
                            height = camera.Height,
                            width = camera.Width,
                            frameData = Convert.ToBase64String(camera.JpegData ?? new byte[] { })
                        });
                    }
                }

                // Events for this tick;
                List<ViewerServers.CompetitionUpdateMessage.Event> currentEvents = new();
                while (!_playerEventQueue.IsEmpty)
                {
                    if (_playerEventQueue.TryDequeue(out EventArgs? playerEvent) && playerEvent is not null)
                    {
                        ViewerServers.CompetitionUpdateMessage.Event currentEvent = new();
                        switch (playerEvent)
                        {
                            case Games.PlayerDigEventArgs digEvent:
                                currentEvent = new()
                                {
                                    playerDigEvent = new()
                                    {
                                        playerId = digEvent.Player.PlayerId,
                                        targetChunk = digEvent.TargetChunk
                                    }
                                };
                                break;
                            case Games.PlayerPickUpEventArgs pickUpEvent:
                                currentEvent = new()
                                {
                                    playerPickUpEvent = new()
                                    {
                                        playerId = pickUpEvent.Player.PlayerId,
                                        itemCount = pickUpEvent.ItemCount,
                                        itemType = (ViewerServers.CompetitionUpdateMessage.Event.PlayerPickUpEvent.ItemType)pickUpEvent.MineType
                                    }
                                };
                                break;
                            case Games.PlayerPlaceEventArgs placeEvent:
                                currentEvent = new()
                                {
                                    playerPlaceBlockEvent = new()
                                    {
                                        playerId = placeEvent.Player.PlayerId
                                    }
                                };
                                break;
                            // TODO: finish other cases
                            default:
                                break;
                        }
                        currentEvents.Add(currentEvent);
                    }
                }


                // Send packet to the viewer
                _viewerServer.Publish(new ViewerServers.CompetitionUpdateMessage()
                {
                    cameras = cameraInfoList,

                    chunks = _game.GameMap.Chunks.Select(chunk => new ViewerServers.CompetitionUpdateMessage.Chunk()
                    {
                        chunkId = chunk.Position != null ? chunk.Position.X + chunk.Position.Y * 8 : -1,
                        height = chunk.Height,
                        position = chunk.Position != null ? new ViewerServers.CompetitionUpdateMessage.Chunk.Position()
                        {
                            x = chunk.Position.X,
                            y = chunk.Position.Y
                        } : null
                    }).ToList(),

                    events = currentEvents,

                    info = new ViewerServers.CompetitionUpdateMessage.Info()
                    {
                        stage = _game.CurrentStage switch
                        {
                            Games.IGame.Stage.Ready => ViewerServers.CompetitionUpdateMessage.Info.Stage.Ready,
                            Games.IGame.Stage.Running => ViewerServers.CompetitionUpdateMessage.Info.Stage.Running,
                            Games.IGame.Stage.Finished => ViewerServers.CompetitionUpdateMessage.Info.Stage.Finished,
                            Games.IGame.Stage.Battling => ViewerServers.CompetitionUpdateMessage.Info.Stage.Battling,
                            _ => throw new NotImplementedException($"{_game.CurrentStage} is not implemented")
                        },
                        elapsedTicks = _game.ElapsedTicks
                    },

                    mines = _game.Mines.Select(mine => new ViewerServers.CompetitionUpdateMessage.Mine()
                    {
                        mineId = mine.MineId.ToString(),
                        accumulatedOreCount = mine.AccumulatedOreCount,
                        oreType = (ViewerServers.CompetitionUpdateMessage.Mine.OreType)mine.OreKind,
                        position = new ViewerServers.CompetitionUpdateMessage.Mine.Position()
                        {
                            x = mine.Position.X,
                            y = mine.Position.Y
                        }
                    }).ToList(),

                    players = _game.Players.Select(player => new ViewerServers.CompetitionUpdateMessage.Player()
                    {
                        playerId = (player.PlayerId),

                        cameraId = player.PlayerId,

                        attributes = new()
                        {
                            agility = player.ActionPoints,
                            strength = player.Strength,
                            maxHealth = player.MaxHealth
                        },
                        health = player.Health,
                        homePosition = new ViewerServers.CompetitionUpdateMessage.Player.HomePosition()
                        {
                            x = player.SpawnPoint.X,
                            y = player.SpawnPoint.Y,
                        },
                        inventory = new ViewerServers.CompetitionUpdateMessage.Player.Inventory()
                        {
                            emerald = player.EmeraldCount,
                            wool = player.WoolCount
                        },
                        position = new ViewerServers.CompetitionUpdateMessage.Player.Position()
                        {
                            x = player.PlayerPosition.X,
                            y = player.PlayerPosition.Y
                        }
                    }).ToList()
                });
            }
            catch (Exception ex)
            {
                _logger.Fatal($"An unhandled exception occurred while sending to viewer: {ex}");
            }
        }
    }

    Config ReLoadConfig()
    {
        Config config = new();
        string path = Path.Combine(Directory.GetCurrentDirectory(), "config.json");
        if (!File.Exists(path))
        {
            Log.Warning($"Config file not found at {path}, creating default config file...");
            File.WriteAllText(path, JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true }));
        }
        else
        {
            try
            {
                config = JsonSerializer.Deserialize<Config>(File.ReadAllText(path))!;
            }
            catch (JsonException)
            {
                Log.Error($"Error parsing config file at {path}");

#if DEBUG
                throw;
#else
                Log.Information($"Using default config.");
#endif
            }
        }

        return config;
    }
}
