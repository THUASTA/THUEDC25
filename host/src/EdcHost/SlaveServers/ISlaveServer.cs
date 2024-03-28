namespace EdcHost.SlaveServers;

/// <summary>
/// SlaveServer handles the communication with the slaves via UART.
/// </summary>
public interface ISlaveServer : IDisposable
{
    static ISlaveServer Create()
    {
        ISerialPortHub serialPortHub = new SerialPortHub();

        return new SlaveServer(serialPortHub);
    }

    public record PortInfo
    {
        public int BaudRate { get; init; } = 0;
    }

    event EventHandler<PlayerTryAttackEventArgs>? PlayerTryAttackEvent;
    event EventHandler<PlayerTryPlaceBlockEventArgs>? PlayerTryPlaceBlockEvent;
    event EventHandler<PlayerTryTradeEventArgs>? PlayerTryTradeEvent;

    /// <summary>
    /// Gets the available port names.
    /// </summary>
    List<string> AvailablePortNames { get; }

    List<string> OpenPortNames { get; }

    /// <summary>
    /// Starts the server.
    /// </summary>
    void Start();

    /// <summary>
    /// Stops the server.
    /// </summary>
    void Stop();

    void OpenPort(string portName, int baudRate);

    void ClosePort(string portName);

    PortInfo? GetPortInfo(string portName);

    void Publish(
        string portName,
        int gameStage,
        int elapsedTime,
        List<int> heightOfChunks,
        bool hasBed,
        bool hasBedOpponent,
        double positionX,
        double positionY,
        double positionOpponentX,
        double positionOpponentY,
        int agility,
        int health,
        int maxHealth,
        int strength,
        int emeraldCount,
        int woolCount,
        List<int> owningOreKindOfChunks
    );
}
