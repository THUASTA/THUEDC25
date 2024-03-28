using EdcHost.Games;
using Xunit;

namespace EdcHost.Tests.UnitTests.Games;

public class PlayerTests
{
    const int Expected_MaxHealth = 20;
    const int Expected_Strength = 1;
    const int Expected_Initial_ActionPoints = 0;
    const int Expected_Initial_WoolCount = 8;

    [Fact]
    public void Player_Initialization_DefaultConstructor()
    {
        // Arrange & Act
        IPlayer player = IPlayer.Create();

        // Assert
        Assert.Equal(1, player.PlayerId);
        Assert.Equal(0, player.EmeraldCount);
        Assert.True(player.IsAlive);
        Assert.True(player.HasBed);
        Assert.NotNull(player.SpawnPoint);
        Assert.NotNull(player.PlayerPosition);
        Assert.Equal(Expected_Initial_WoolCount, player.WoolCount);
        Assert.Equal(Expected_MaxHealth, player.Health);
        Assert.Equal(Expected_MaxHealth, player.MaxHealth);
        Assert.Equal(Expected_Strength, player.Strength);
        Assert.Equal(Expected_Initial_ActionPoints, player.ActionPoints);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    [InlineData(6)]
    [InlineData(7)]
    [InlineData(8)]
    [InlineData(int.MaxValue)]
    public void IdX_DoNothing_ReturnsConstructorValue(int X)
    {
        IPlayer player = IPlayer.Create(X, 0, 0, 0, 0);
        Assert.Equal(X, player.PlayerId);
        Assert.Equal(0, player.EmeraldCount);
        Assert.True(player.IsAlive);
        Assert.True(player.HasBed);
        Assert.Equal(0, player.SpawnPoint.X);
        Assert.Equal(0, player.SpawnPoint.Y);
        Assert.Equal(0, player.PlayerPosition.X);
        Assert.Equal(0, player.PlayerPosition.Y);
        Assert.Equal(Expected_Initial_WoolCount, player.WoolCount);
        Assert.Equal(Expected_MaxHealth, player.Health);
        Assert.Equal(Expected_MaxHealth, player.MaxHealth);
        Assert.Equal(Expected_Strength, player.Strength);
        Assert.Equal(Expected_Initial_ActionPoints, player.ActionPoints);
    }
    [Theory]
    [InlineData(0, 0)]
    [InlineData(1, 1)]
    [InlineData(-1, -0.5f)]
    [InlineData(int.MaxValue, int.MaxValue)]
    [InlineData(int.MinValue, int.MinValue)]
    [InlineData(int.MaxValue, int.MinValue)]
    public void SpawnPoint_DoNothing_ReturnsConstructorValue(int X, int Y)
    {
        IPlayer player = IPlayer.Create(1, X, Y, 0, 0);
        Assert.Equal(X, player.SpawnPoint.X);
        Assert.Equal(Y, player.SpawnPoint.Y);
    }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(0.5f, 0.5f)]
    [InlineData(-0.5f, -0.5f)]
    [InlineData(0.5f, -0.5f)]
    [InlineData(1.0f / 3, 1.0f / 3)]
    [InlineData(float.MaxValue, float.MaxValue)]
    [InlineData(float.MinValue, float.MinValue)]
    [InlineData(float.MaxValue, float.MinValue)]
    public void PlayerPosition_DoNothing_ReturnsConstructorValue(float X, float Y)
    {
        IPlayer player = IPlayer.Create(1, 0, 0, X, Y);
        Assert.Equal(X, player.PlayerPosition.X);
        Assert.Equal(Y, player.PlayerPosition.Y);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(0)]
    [InlineData(int.MaxValue)]
    public void EmeraldAdd_IncreaseBy(int x)
    {
        IPlayer player = IPlayer.Create();
        player.EmeraldAdd(x);
        Assert.Equal(x, player.EmeraldCount);
    }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(0.5f, 0.5f)]
    [InlineData(-0.5f, -0.5f)]
    [InlineData(0.5f, -0.5f)]
    [InlineData(1.0f / 3, 1.0f / 3)]
    [InlineData(float.MaxValue, float.MaxValue)]
    [InlineData(float.MinValue, float.MinValue)]
    [InlineData(float.MaxValue, float.MinValue)]
    public void Move_ToNewPosition_ReturnsNewCoordinate(float newX, float newY)
    {
        IPlayer player = IPlayer.Create();
        player.OnMove += (this_x, args) =>
        {
            Assert.Equal(0, args.PositionBeforeMovement.X);
            Assert.Equal(0, args.PositionBeforeMovement.Y);
            Assert.Equal(newX, args.Position.X);
            Assert.Equal(newY, args.Position.Y);
            Assert.Equal(player, args.Player);
            Assert.Equal(player, this_x);
        };
        player.Move(newX, newY);
        Assert.Equal(newX, player.PlayerPosition.X);
        Assert.Equal(newY, player.PlayerPosition.Y);
    }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(1, 1)]
    [InlineData(-1, -1)]
    [InlineData(int.MaxValue, int.MaxValue)]
    [InlineData(int.MinValue, int.MinValue)]
    [InlineData(int.MaxValue, int.MinValue)]
    public void Attack_TestPosition_CheckEvent(int newX, int newY)
    {
        IPlayer player = IPlayer.Create();
        bool event_triggered = false;
        player.OnAttack += (this_x, args) =>
        {
            Assert.Equal(Expected_Strength, args.Strength);
            Assert.Equal(newX, args.Position.X);
            Assert.Equal(newY, args.Position.Y);
            Assert.Equal(player, args.Player);
            Assert.Equal(player, this_x);
            event_triggered = true;
        };
        player.Attack(newX, newY);
        Assert.True(event_triggered);
    }
    //TODO: wait for implementation of WoolCount adding.

    [Theory]
    [InlineData(0, 0)]
    [InlineData(1, 1)]
    [InlineData(-1, -1)]
    [InlineData(int.MaxValue, int.MaxValue)]
    [InlineData(int.MinValue, int.MinValue)]
    [InlineData(int.MaxValue, int.MinValue)]
    public void Place_TestPosition_HasWool_EventTriggered(int newX, int newY)
    {
        IPlayer player = IPlayer.Create();
        player.EmeraldAdd(1);
        player.Trade(IPlayer.CommodityKindType.Wool);
        bool event_triggered = false;
        player.OnPlace += (this_x, args) =>
        {
            Assert.Equal(newX, args.Position.X);
            Assert.Equal(newY, args.Position.Y);
            Assert.Equal(player, args.Player);
            Assert.Equal(player, this_x);
            event_triggered = true;
        };
        player.Place(newX, newY);
        Assert.True(event_triggered);
    }
    [Fact]
    public void Place_NoWool_EventNotTriggered()
    {
        IPlayer player = IPlayer.Create();
        bool event_triggered = false;
        player.OnPlace += (this_x, args) =>
        {
            event_triggered = true;
        };
        for (int i = 1; i <= Expected_Initial_WoolCount; i++)
        {
            player.DecreaseWoolCount();
            Assert.Equal(Expected_Initial_WoolCount - i, player.WoolCount);
        }
        player.Place(0, 0);
        Assert.False(event_triggered);
    }
    [Theory]
    [InlineData(1)]
    [InlineData(19)]
    public void Hurt_LessthanHealth_CheckNewHealth(int EnemyStrength)
    {
        IPlayer player = IPlayer.Create();
        bool event_triggered = false;
        player.OnDie += (this_x, args) =>
        {
            event_triggered = true;
        };
        player.Hurt(EnemyStrength);
        Assert.Equal(Expected_MaxHealth - EnemyStrength, player.Health);
        Assert.False(event_triggered);
        Assert.True(player.IsAlive);
    }
    [Theory]
    [InlineData(20)]
    [InlineData(int.MaxValue)]
    public void Hurt_CoversHealth_PlayerDie(int EnemyStrength)
    {
        IPlayer player = IPlayer.Create();
        bool event_triggered = false;
        player.OnDie += (this_x, args) =>
        {
            Assert.Equal(player, this_x);
            Assert.Equal(player, args.Player);
            event_triggered = true;
        };
        player.Hurt(EnemyStrength);
        Assert.Equal(0, player.Health);
        Assert.True(event_triggered);
        Assert.False(player.IsAlive);
    }
    [Fact]
    public void KillandSpawn_HasBed_Alive()
    {
        IPlayer player = IPlayer.Create();
        player.Hurt(100);
        Assert.Equal(0, player.Health);
        Assert.False(player.IsAlive);
        player.Spawn(player.MaxHealth);
        Assert.Equal(Expected_MaxHealth, player.Health);
        Assert.True(player.IsAlive);
    }
    [Fact]
    public void KillandSpawn_BedDestroyed_NothingHappens()
    {
        IPlayer player = IPlayer.Create();
        player.DestroyBed();
        player.Hurt(100);
        Assert.Equal(0, player.Health);
        Assert.False(player.IsAlive);
        player.Spawn(player.MaxHealth);
        Assert.Equal(0, player.Health);
        Assert.False(player.IsAlive);
    }
    [Fact]
    public void DecreaseWoolCount_DecreaseByOne()
    {
        IPlayer player = IPlayer.Create();
        player.EmeraldAdd(2);
        player.Trade(IPlayer.CommodityKindType.Wool);
        Assert.Equal(Expected_Initial_WoolCount + 1, player.WoolCount);
        player.DecreaseWoolCount();
        Assert.Equal(Expected_Initial_WoolCount, player.WoolCount);
    }
    [Fact]
    public void PerformActionPosition_Attack_CheckEvent()
    {
        IPlayer player = IPlayer.Create();
        bool event_triggered = false;
        player.OnAttack += (this_x, args) =>
        {
            event_triggered = true;
        };
        player.PerformActionPosition(IPlayer.ActionKindType.Attack, 0, 0);
        Assert.True(event_triggered);
    }
    [Fact]
    public void PerformActionPosition_Place_CheckEvent()
    {
        IPlayer player = IPlayer.Create();
        bool event_triggered = false;
        player.OnPlace += (this_x, args) =>
        {
            event_triggered = true;
        };
        while (player.WoolCount > 0)
        {
            player.DecreaseWoolCount();
        }
        player.PerformActionPosition(IPlayer.ActionKindType.PlaceBlock, 0, 0);
        Assert.False(event_triggered);

        player.EmeraldAdd(2);
        player.Trade(IPlayer.CommodityKindType.Wool);
        player.PerformActionPosition(IPlayer.ActionKindType.PlaceBlock, 0, 0);
        Assert.True(event_triggered);
    }
    //TODO: TEST Trade
    [Fact]
    public void Trade_AgilityBoost_CommodityKindType()
    {
        // Arrange
        IPlayer player = IPlayer.Create();
        player.EmeraldAdd(64);

        // Act
        bool result = player.Trade(IPlayer.CommodityKindType.AgilityBoost);

        // Assert
        Assert.True(result);
        Assert.Equal(32, player.EmeraldCount);
        Assert.Equal(1, player.ActionPoints);

        // Act
        result = player.Trade(IPlayer.CommodityKindType.AgilityBoost);

        // Assert
        Assert.True(result);
        Assert.Equal(0, player.EmeraldCount);
        Assert.Equal(2, player.ActionPoints);

        // Act
        result = player.Trade(IPlayer.CommodityKindType.AgilityBoost);

        // Assert
        Assert.False(result);
        Assert.Equal(0, player.EmeraldCount);
        Assert.Equal(2, player.ActionPoints);
    }

    [Fact]
    public void Trade_HealthBoost_CommodityKindType()
    {
        // Arrange
        IPlayer player = IPlayer.Create();
        player.EmeraldAdd(64);

        // Act
        bool result = player.Trade(IPlayer.CommodityKindType.HealthBoost);

        // Assert
        Assert.True(result);
        Assert.Equal(32, player.EmeraldCount);
        Assert.Equal(23, player.MaxHealth);

        // Act
        result = player.Trade(IPlayer.CommodityKindType.HealthBoost);

        // Assert
        Assert.True(result);
        Assert.Equal(0, player.EmeraldCount);
        Assert.Equal(26, player.MaxHealth);

        // Act
        result = player.Trade(IPlayer.CommodityKindType.HealthBoost);

        // Assert
        Assert.False(result);
        Assert.Equal(0, player.EmeraldCount);
        Assert.Equal(26, player.MaxHealth);
    }

    [Fact]
    public void Trade_HealthPotion_CommodityKindType()
    {
        // Arrange
        IPlayer player = IPlayer.Create();
        player.EmeraldAdd(5);
        player.Hurt(10);

        // Act
        bool result = player.Trade(IPlayer.CommodityKindType.HealthPotion);

        // Assert
        Assert.True(result);
        Assert.Equal(1, player.EmeraldCount);
        Assert.Equal(11, player.Health);

        // Act
        result = player.Trade(IPlayer.CommodityKindType.HealthPotion);

        // Assert
        Assert.False(result);
        Assert.Equal(1, player.EmeraldCount);
        Assert.Equal(11, player.Health);
    }

    [Fact]
    public void Trade_UnknownCommodityKindType()
    {
        // Arrange
        IPlayer player = IPlayer.Create();
        player.EmeraldAdd(10);

        // Act

        // Assert
        Assert.Throws<ArgumentOutOfRangeException>(
            () => player.Trade((IPlayer.CommodityKindType)99)
        ); // Unknown commodity kind
        Assert.Equal(10, player.EmeraldCount); // Emerald count remains unchanged
    }
    //
}
