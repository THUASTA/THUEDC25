namespace EdcHost.Games;

/// <summary>
/// Player represents a player in the game.
/// </summary>
public interface IPlayer
{
    static IPlayer Create(int id = 1, int spawnX = 0, int spawnY = 0, float x = 0, float y = 0)
    {
        return new Player(id, spawnX, spawnY, x, y);
    }

    /// <summary>
    /// The kind of a commodity.
    /// </summary>
    enum CommodityKindType
    {
        AgilityBoost,
        HealthBoost,
        HealthPotion,
        StrengthBoost,
        Wool,
    }

    /// <summary>
    /// The kind of an action.
    /// </summary>
    enum ActionKindType
    {
        Attack,
        PlaceBlock

    }

    /// <summary>
    /// Triggered when the player moves.
    /// </summary>
    event EventHandler<PlayerMoveEventArgs> OnMove;
    event EventHandler<PlayerAttackEventArgs> OnAttack;
    event EventHandler<PlayerPlaceEventArgs> OnPlace;
    event EventHandler<PlayerDieEventArgs> OnDie;
    event EventHandler<PlayerDigEventArgs> OnDig;
    event EventHandler<PlayerPickUpEventArgs> OnPickUp;
    event EventHandler<PlayerTradeEventArgs> OnTrade;

    void EmeraldAdd(int count);
    void Move(float newX, float newY);
    void Attack(int newX, int newY);
    void Place(int newX, int newY);
    void Hurt(int EnemyStrength);
    void Spawn(int MaxHealth);
    void DestroyBed();
    void DestroyBedOpponent();
    void DecreaseWoolCount();
    void DigEventInvoker(int targetChunk);
    void PickUpEventInvoker(IMine.OreKindType mineType, int count, string mineId);

    /// <summary>
    /// The Id of  the player
    /// </summary>
    int PlayerId { get; }
    /// <summary>
    /// The count of emeralds the player has.
    /// </summary>
    int EmeraldCount { get; }
    /// <summary>
    /// Whether the player is alive.
    /// </summary>
    bool IsAlive { get; }
    /// <summary>
    /// Whether the player has a bed.
    /// </summary>
    bool HasBed { get; }
    /// <summary>
    /// Whether the player has a bed.
    /// </summary>
    bool HasBedOpponent { get; }
    /// <summary>
    /// The spawn point of the player.
    /// </summary>
    IPosition<int> SpawnPoint { get; }
    /// <summary>
    /// The position of the player.
    /// </summary>
    IPosition<float> PlayerPosition { get; }
    /// <summary>
    /// The count of wool blocks the player has.
    /// </summary>
    int WoolCount { get; }
    int Health { get; }
    int MaxHealth { get; }
    int Strength { get; }
    int ActionPoints { get; }
    /// <summary>
    /// Performs an action.
    /// </summary>
    /// <param name="actionKind">The action kind.</param>
    void PerformActionPosition(ActionKindType actionKind, int X, int Y);
    /// <summary>
    /// Send a trade request.
    /// </summary>
    /// <param name="commodityKind">The commodity kind.</param>
    void SendTradeRequest(CommodityKindType commodityKind);
    /// <summary>
    /// Trades a commodity.
    /// </summary>
    /// <param name="commodityKind">The commodity kind.</param>
    bool Trade(CommodityKindType commodityKind);
}
