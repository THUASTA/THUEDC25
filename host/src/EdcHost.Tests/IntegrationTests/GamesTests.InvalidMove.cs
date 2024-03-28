using EdcHost.Games;
using Xunit;

namespace EdcHost.Tests.IntegrationTests;

public partial class GamesTests
{
    const int TicksBeforeRespawn = 300;
    [Fact]
    public void Game_InvalidMoveTests()
    {
        var game = IGame.Create();

        // Act1 ValidMoveMent
        game.Start();
        for (int i = 1; i < 8; i++)
        {
            float x0 = game.Players[0].SpawnPoint.X;
            float y0 = game.Players[0].SpawnPoint.Y;
            float x1 = game.Players[1].SpawnPoint.X;
            float y1 = game.Players[1].SpawnPoint.Y;
            float move = i;
            game.Players[0].Move(x0 + move, y0 + move);
            game.Players[1].Move(x1 - move, y1 - move);
            game.Tick();
            Assert.StrictEqual(IGame.Stage.Running, game.CurrentStage);
            Assert.StrictEqual(x0 + move, game.Players[0].PlayerPosition.X);
            Assert.StrictEqual(y0 + move, game.Players[0].PlayerPosition.Y);
            Assert.StrictEqual(x1 - move, game.Players[1].PlayerPosition.X);
            Assert.StrictEqual(y1 - move, game.Players[1].PlayerPosition.Y);
            Assert.False(game.Players[0].IsAlive);
        }

        // Act2 InvalidMoveMent and Relive
        game.Players[1].Move(8f, 8f);
        game.Tick();
        Assert.False(game.Players[1].IsAlive);
        Assert.Null(game.Winner);
        for (int i = 0; i < TicksBeforeRespawn; i++)
        {
            game.Tick();
            Assert.False(game.Players[1].IsAlive);
        }
        game.Players[1].Move(game.Players[1].SpawnPoint.X, game.Players[1].SpawnPoint.Y);
        game.Players[0].Move(game.Players[0].SpawnPoint.X, game.Players[0].SpawnPoint.Y);
        game.Tick();
        Assert.True(game.Players[1].IsAlive);
        Assert.True(game.Players[0].IsAlive);

        // Act3 InvalidMoveMent and Die
        game.Players[0].Move(1.4f, 0.4f);
        game.Tick();
        Assert.False(game.Players[0].IsAlive);
        Assert.Null(game.Winner);
        for (int i = 0; i < TicksBeforeRespawn / 2; i++)
        {
            game.Tick();
        }
        game.Players[1].Move(game.Players[0].SpawnPoint.X, game.Players[0].SpawnPoint.Y);
        game.Tick();
        game.Players[1].Attack(game.Players[0].SpawnPoint.X, game.Players[0].SpawnPoint.Y);
        game.Tick();
        Assert.StrictEqual(game.Players[1], game.Winner);
        Assert.StrictEqual(IGame.Stage.Finished, game.CurrentStage);

    }

}
