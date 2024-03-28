using EdcHost.Games;
using Xunit;

namespace EdcHost.Tests.IntegrationTests;

public partial class GamesTests
{
    [Fact]
    public void Simple()
    {
        const int TicksBeforeBattling = 12000;
        const int TicksBattling = 2000;

        // Arrange
        var game = IGame.Create();

        IGame.Stage? stage = game.CurrentStage;
        int elapsedTicks = game.ElapsedTicks;
        IPlayer? winner = game.Winner;

        Assert.StrictEqual(IGame.Stage.Ready, stage);
        Assert.StrictEqual(0, elapsedTicks);
        Assert.Null(winner);

        // Act 1
        game.Start();

        stage = game.CurrentStage;
        elapsedTicks = game.ElapsedTicks;
        winner = game.Winner;

        Assert.StrictEqual(IGame.Stage.Running, stage);
        Assert.StrictEqual(0, elapsedTicks);
        Assert.Null(winner);

        for (int tick = 1; tick <= TicksBeforeBattling; ++tick)
        {
            // Act 2~12001
            game.Tick();

            stage = game.CurrentStage;
            elapsedTicks = game.ElapsedTicks;
            winner = game.Winner;

            Assert.StrictEqual(IGame.Stage.Running, stage);
            Assert.StrictEqual(tick, elapsedTicks);
            Assert.Null(winner);
        }

        for (int tick = TicksBeforeBattling + 1; tick <= TicksBeforeBattling + TicksBattling; ++tick)
        {
            // Act 12002~12401
            game.Tick();

            stage = game.CurrentStage;
            elapsedTicks = game.ElapsedTicks;
            winner = game.Winner;

            Assert.StrictEqual(IGame.Stage.Battling, stage);
            Assert.StrictEqual(tick, elapsedTicks);
            Assert.Null(winner);
        }

        // Act 12402
        game.Tick();

        stage = game.CurrentStage;
        elapsedTicks = game.ElapsedTicks;
        winner = game.Winner;

        Assert.StrictEqual(IGame.Stage.Finished, stage);
        Assert.StrictEqual(TicksBeforeBattling + TicksBattling + 1, elapsedTicks);
        Assert.Null(winner);
    }
}
