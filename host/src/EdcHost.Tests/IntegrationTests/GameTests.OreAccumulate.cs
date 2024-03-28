using EdcHost.Games;
using Xunit;

namespace EdcHost.Tests.IntegrationTests;

public partial class GameTests
{
    const int WoolCount = 8;
    const int MaxHeight = 8;
    const int MaximumItemCount = 128;
    public class MockPosition : IPosition<int>
    {
        public int X { get; set; }
        public int Y { get; set; }
    }

    [Fact]
    public void OreAccumulationTests()
    {
        Tuple<int, int> iron1 = Tuple.Create(0, 0);
        Tuple<int, int> iron2 = Tuple.Create(7, 7);
        Tuple<int, int> gold1 = Tuple.Create(3, 3);
        Tuple<int, int> gold2 = Tuple.Create(4, 4);
        Tuple<int, int> diamond1 = Tuple.Create(5, 5);
        List<Tuple<int, int>>? iron = new();
        List<Tuple<int, int>>? gold = new();
        List<Tuple<int, int>>? diamond = new();
        iron.Add(iron1);
        iron.Add(iron2);
        gold.Add(gold1);
        gold.Add(gold2);
        diamond.Add(diamond1);
        var game = IGame.Create(diamond, gold, iron);
        game.Start();

        //Act1 Increase the height of bed to max
        Assert.StrictEqual(0, game.Players[0].EmeraldCount);
        IPosition<int> position = new MockPosition { X = 0, Y = 0 };
        for (int i = 1; i < 8; i++)
        {
            game.Players[0].Place(game.Players[0].SpawnPoint.X, game.Players[0].SpawnPoint.Y);
            game.Tick();
            Assert.StrictEqual(i + 1, game.GameMap.GetChunkAt(position).Height);
            Assert.StrictEqual(WoolCount - i, game.Players[0].WoolCount);
        }
        game.Players[0].Place(game.Players[0].SpawnPoint.X, game.Players[0].SpawnPoint.Y);
        game.Tick();
        Assert.StrictEqual(1, game.Players[0].WoolCount);
        Assert.StrictEqual(MaxHeight, game.GameMap.GetChunkAt(position).Height);

        //Act2 accumulate iron 7～11
        for (int i = 0; i < OreAccumulationInterval; i++)
        {
            game.Tick();
        }
        Assert.StrictEqual(1, game.Players[0].EmeraldCount);
        for (int i = 0; i < 9 * OreAccumulationInterval; i++)
        {
            game.Tick();
        }
        Assert.StrictEqual(10, game.Players[0].EmeraldCount);

        //Act3 Trade and Invalid Placement
        game.Players[0].Place(3, 2);
        IPosition<int> position0 = new MockPosition { X = 3, Y = 2 };
        Assert.StrictEqual(0, game.GameMap.GetChunkAt(position0).Height);
        Assert.StrictEqual(1, game.Players[0].WoolCount);
        game.Players[0].Place(-1, 0);
        Assert.StrictEqual(1, game.Players[0].WoolCount);
        for (int i = 1; i < 6; i++)
        {
            game.Players[0].Trade(IPlayer.CommodityKindType.Wool);
            game.Tick();
            Assert.StrictEqual(10 - 2 * i, game.Players[0].EmeraldCount);
            Assert.StrictEqual(i + 1, game.Players[0].WoolCount);
        }
        Assert.False(game.Players[0].Trade(IPlayer.CommodityKindType.Wool));

        //Act4 Valid Placement and Accumulate gold and diamond
        IPosition<int> position1 = new MockPosition { X = 1, Y = 0 };
        Assert.StrictEqual(0, game.GameMap.GetChunkAt(position1).Height);
        game.Players[0].Place(1, 0);
        game.Tick();
        Assert.StrictEqual(1, game.GameMap.GetChunkAt(position1).Height);
        game.Players[0].Move(1.4f, 0.4f);
        game.Tick();
        game.Players[0].Place(1, 1);
        game.Tick();
        game.Players[0].Move(1.4f, 1.4f);
        game.Tick();
        game.Players[0].Place(2, 1);
        game.Tick();
        game.Players[0].Move(2.4f, 1.4f);
        game.Tick();
        game.Players[0].Place(3, 1);
        game.Tick();
        game.Players[0].Move(3.4f, 1.4f);
        game.Tick();
        game.Players[0].Place(3, 2);
        game.Tick();
        game.Players[0].Move(3.4f, 2.4f);
        game.Tick();
        game.Players[0].Place(3, 3);
        game.Tick();
        game.Players[0].Move(3.4f, 3.4f);
        game.Tick();
        Assert.StrictEqual(0, game.Players[0].WoolCount);
        Assert.StrictEqual(40, game.Players[0].EmeraldCount);
        for (int i = 1; i < 21; i++)
        {
            game.Players[0].Trade(IPlayer.CommodityKindType.Wool);
            game.Tick();
            Assert.StrictEqual(i, game.Players[0].WoolCount);
        }
        Assert.StrictEqual(0, game.Players[0].EmeraldCount);
        game.Players[0].Place(4, 3);
        game.Tick();
        game.Players[0].Move(4.4f, 3.4f);
        game.Tick();
        game.Players[0].Place(5, 3);
        game.Tick();
        game.Players[0].Move(5.4f, 3.4f);
        game.Tick();
        game.Players[0].Place(5, 4);
        game.Tick();
        game.Players[0].Move(5.4f, 4.4f);
        game.Tick();
        game.Players[0].Place(5, 5);
        game.Tick();
        game.Players[0].Move(5.4f, 5.4f);
        game.Tick();
        Assert.StrictEqual(16, game.Players[0].WoolCount);
        Assert.StrictEqual(MaximumItemCount, game.Players[0].EmeraldCount);

        game.End();
    }

}
