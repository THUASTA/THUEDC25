namespace EdcHost.Games;

class Player : IPlayer
{
    const int InitialEmeraldCount = 0;
    const int InitialWoolCount = 8;
    const int InitialHealth = 20;
    const int InitialMaxHealth = 20;
    const int InitialStrength = 1;
    const int InitialAgility = 0;
    public int PlayerId { get; private set; }
    public int EmeraldCount { get; private set; }
    public bool HasBed { get; private set; }
    public bool HasBedOpponent { get; private set; }
    public bool IsAlive { get; private set; }
    public IPosition<int> SpawnPoint { get; private set; }
    public IPosition<float> PlayerPosition { get; private set; }
    public int WoolCount { get; private set; }

    public int Health { get; private set; } /// Health property
    public int MaxHealth { get; private set; } /// Max health property
    public int Strength { get; private set; } /// Strength property
    public int ActionPoints { get; private set; } /// Action points property

    public event EventHandler<PlayerMoveEventArgs> OnMove = delegate { };
    public event EventHandler<PlayerAttackEventArgs> OnAttack = delegate { };
    public event EventHandler<PlayerPlaceEventArgs> OnPlace = delegate { };
    public event EventHandler<PlayerDieEventArgs> OnDie = delegate { };

    public event EventHandler<PlayerDigEventArgs> OnDig = delegate { };
    public event EventHandler<PlayerPickUpEventArgs> OnPickUp = delegate { };
    public event EventHandler<PlayerTradeEventArgs> OnTrade = delegate { };

    public void PickUpEventInvoker(IMine.OreKindType mineType, int count, string mineId)
    {
        OnPickUp?.Invoke(this, new PlayerPickUpEventArgs(this, mineType, count, mineId));
    }
    public void DigEventInvoker(int targetChunk)
    {
        OnDig?.Invoke(this, new PlayerDigEventArgs(this, targetChunk));
    }

    public void EmeraldAdd(int count)
    {
        /// update the player's Emeraldcount
        EmeraldCount += count;
    }

    public void Move(float newX, float newY)
    {
        /// Trigger the OnMove event to notify other parts that the player has moved
        OnMove?.Invoke(this, new PlayerMoveEventArgs(this, PlayerPosition, new Position<float>(newX, newY)));
        /// Update the player's position information
        PlayerPosition.X = newX;
        PlayerPosition.Y = newY;
    }

    public void Attack(int newX, int newY)
    {
        /// Trigger the OnAttack event to notify the attacked block
        OnAttack?.Invoke(this, new PlayerAttackEventArgs(this, Strength, new Position<int>(newX, newY)));
    }
    public void Place(int newX, int newY)
    {
        /// Check if the player has wool in their inventory, and if so, process wool data.
        ///  Trigger the OnPlace event to notify the placed block
        if (WoolCount > 0)
        {
            OnPlace?.Invoke(this, new PlayerPlaceEventArgs(this, new Position<int>(newX, newY)));
        }
    }
    public void Hurt(int EnemyStrength)
    {
        /// Implement the logic for being hurt
        if (Health > EnemyStrength)
        {
            Health -= EnemyStrength;
        }
        else
        {
            Health = 0;
            IsAlive = false;
            OnDie?.Invoke(this, new PlayerDieEventArgs(this));
        }
    }
    public void Spawn(int MaxHealth)
    {
        if (HasBed == true)
        {
            IsAlive = true;
            Health = MaxHealth;
            /// <remarks>
            /// SpawnPoint should not change.
            /// </remarks>
        }
    }
    public void DestroyBed()
    {
        /// Destroy a player's bed.
        HasBed = false;
    }
    public void DestroyBedOpponent()
    {
        /// Destroy a player's bed.
        HasBedOpponent = false;
    }
    public void DecreaseWoolCount()
    {
        /// Decrease wool count by 1.
        WoolCount -= 1;
    }
    public Player(int id = 1, int spawnPointX = 0, int spawnPointY = 0, float X = 0, float Y = 0)
    {
        PlayerId = id;
        IsAlive = true;
        HasBed = true;
        HasBedOpponent = true;
        SpawnPoint = new Position<int>(spawnPointX, spawnPointY);
        PlayerPosition = new Position<float>(X, Y);

        EmeraldCount = InitialEmeraldCount;
        WoolCount = InitialWoolCount;

        Health = InitialHealth; /// Initial health
        MaxHealth = InitialMaxHealth; /// Initial max health
        Strength = InitialStrength; /// Initial strength
        ActionPoints = InitialAgility; /// Initial action points
    }
    public void PerformActionPosition(IPlayer.ActionKindType actionKind, int X, int Y)
    {
        switch (actionKind)
        {
            case IPlayer.ActionKindType.Attack:
                /// Implement the logic for attacking
                Attack(X, Y);
                break;
            case IPlayer.ActionKindType.PlaceBlock:
                /// Implement the logic for placing a block
                Place(X, Y);
                break;
            default:
                /// Handle unknown action types
                break;
        }
    }
    public void SendTradeRequest(IPlayer.CommodityKindType commodityKind)
    {
        OnTrade?.Invoke(this, new PlayerTradeEventArgs(this, commodityKind));
    }
    public bool Trade(IPlayer.CommodityKindType commodityKind)
    {
        int price = commodityKind switch
        {
            IPlayer.CommodityKindType.AgilityBoost => 32,
            IPlayer.CommodityKindType.HealthBoost => 32,
            IPlayer.CommodityKindType.StrengthBoost => 64,
            IPlayer.CommodityKindType.Wool => 2,
            IPlayer.CommodityKindType.HealthPotion => 4,
            _ => throw new ArgumentOutOfRangeException(
                $"No commodity {commodityKind}."
            )
        };

        if (EmeraldCount < price)
        {
            Serilog.Log.Error(
                $"Failed to trade: Player {PlayerId} doesn't have enough emerald."
            );
            return false;
        }

        switch (commodityKind)
        {
            case IPlayer.CommodityKindType.AgilityBoost:
                EmeraldCount -= price;
                ActionPoints += 1;
                return true;
            case IPlayer.CommodityKindType.HealthBoost:
                EmeraldCount -= price;
                MaxHealth += 3;
                Health += 3;
                return true;
            case IPlayer.CommodityKindType.HealthPotion:
                EmeraldCount -= price;
                if (Health < MaxHealth)
                {
                    Health += 1;
                }
                return true;
            case IPlayer.CommodityKindType.StrengthBoost:
                EmeraldCount -= price;
                Strength += 1;
                return true;
            case IPlayer.CommodityKindType.Wool:
                if (WoolCount >= Game.MaximumItemCount)
                {
                    Serilog.Log.Error(
                        $"Failed to trade: Player {PlayerId} cannot hold more wools."
                    );
                    return false;
                }
                EmeraldCount -= price;
                WoolCount += 1;
                return true;
            default:
                throw new ArgumentOutOfRangeException(
                    $"No commodity {commodityKind}."
                );
        }
    }
}
