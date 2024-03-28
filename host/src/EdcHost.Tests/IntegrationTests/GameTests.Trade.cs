using EdcHost.Games;
using Xunit;

namespace EdcHost.Tests.IntegrationTests;

public partial class GameTests
{
    const int MaxHealth = 20;
    const int OreAccumulationInterval = 200;
    const int HealthPotionPrice = 4;
    const int HealthBoostPrice = 32;
    const int AgilityBoostPrice = 32;

    [Fact]
    public void TradeTests()
    {
        List<Tuple<int, int>> diamond = new()
        {
            Tuple.Create(1, 0),
            Tuple.Create(0, 1)
        };
        var game = IGame.Create(diamond, null, null);
        game.Start();

        // Act1 Accumulate diamond, EmeraldCount = 64
        Assert.StrictEqual(0, game.Players[0].EmeraldCount);
        game.Players[0].Place(1, 0);
        game.Tick();
        game.Players[0].Place(0, 1);
        game.Tick();
        game.Players[0].Move(1.4f, 0.4f);
        for (int i = 0; i < 4 * OreAccumulationInterval; i++)
        {
            game.Tick();
        }
        game.Tick();
        Assert.StrictEqual(64, game.Players[0].EmeraldCount);

        // Act2 Trade for health, EmeraldCount = 28

        game.Players[0].Trade(IPlayer.CommodityKindType.HealthPotion);
        game.Tick();
        Assert.StrictEqual(MaximumItemCount / 2 - HealthPotionPrice, game.Players[0].EmeraldCount);
        Assert.StrictEqual(MaxHealth, game.Players[0].Health);
        Assert.StrictEqual(MaxHealth, game.Players[0].MaxHealth);
        Assert.True(game.Players[0].Trade(IPlayer.CommodityKindType.HealthBoost));
        game.Tick();
        Assert.StrictEqual(MaximumItemCount / 2 - HealthPotionPrice - HealthBoostPrice, game.Players[0].EmeraldCount);
        Assert.StrictEqual(MaxHealth + 3, game.Players[0].MaxHealth);
        Assert.StrictEqual(MaxHealth + 3, game.Players[0].Health);

        //Act3 Accumulate diamond, EmeraldCount = 128
        game.Players[0].Move(0.4f, 0.4f);
        game.Tick();
        game.Players[0].Move(0.4f, 1.4f);
        game.Tick();
        for (int i = 0; i < 4 * OreAccumulationInterval; i++)
        {
            game.Tick();
        }
        game.Tick();
        Assert.StrictEqual(MaximumItemCount - HealthPotionPrice, game.Players[0].EmeraldCount);

        //Act4 Trade for Agility, EmeralCount = 0
        game.Players[0].Trade(IPlayer.CommodityKindType.AgilityBoost);
        game.Tick();
        Assert.StrictEqual(1, game.Players[0].ActionPoints);
        Assert.StrictEqual(MaximumItemCount - HealthPotionPrice, game.Players[0].EmeraldCount);
        for (int i = 0; i < 31; i++)
        {
            game.Players[0].Trade(IPlayer.CommodityKindType.HealthPotion);
            game.Tick();
        }
        Assert.StrictEqual(0, game.Players[0].EmeraldCount);

        //Act5 Trade for Strength
        Assert.False(game.Players[0].Trade(IPlayer.CommodityKindType.StrengthBoost));
        game.Tick();
        for (int i = 0; i < 4 * OreAccumulationInterval; i++)
        {
            game.Tick();
        }
        Assert.StrictEqual(MaximumItemCount / 2, game.Players[0].EmeraldCount);
        Assert.True(game.Players[0].Trade(IPlayer.CommodityKindType.StrengthBoost));
        game.Tick();
        Assert.StrictEqual(2, game.Players[0].Strength);
        Assert.StrictEqual(0, game.Players[0].EmeraldCount);

        game.End();

    }
}
