using EdcHost.Games;
using Xunit;

namespace EdcHost.Tests.UnitTests.Games;

public class GameTest
{
    [Fact]
    public void Game_DoNothing_PublicMembersCorrectlyInitialized()
    {
        IGame game = IGame.Create();
        Assert.Equal(IGame.Stage.Ready, game.CurrentStage);
        Assert.Null(game.Winner);
        Assert.Equal(0, game.ElapsedTicks);
        Assert.Equal(0, game.GameMap.Chunks[0].Position.X);
        Assert.Equal(0, game.GameMap.Chunks[0].Position.Y);
        Assert.Equal(1, game.GameMap.Chunks[0].Height);
        Assert.Equal(7, game.GameMap.Chunks[63].Position.X);
        Assert.Equal(7, game.GameMap.Chunks[63].Position.Y);
        Assert.Equal(1, game.GameMap.Chunks[63].Height);
    }

    [Fact]
    public void Start_StartedYet_ThrowsCorrectException()
    {
        var game = IGame.Create();
        game.Start();
        Assert.Throws<InvalidOperationException>(game.Start);
    }

    [Fact]
    public void Start_DoNothing_ReturnsCorrectValue()
    {
        var game = IGame.Create();
        game.Start();
        Assert.Equal(0, game.Players[0].PlayerId);
        Assert.Equal(0, game.Players[0].SpawnPoint.X);
        Assert.Equal(0.5f, game.Players[0].PlayerPosition.Y);
        Assert.Equal(1, game.Players[1].PlayerId);
        Assert.Equal(7, game.Players[1].SpawnPoint.X);
        Assert.Equal(7.5f, game.Players[1].PlayerPosition.Y);
        Assert.Equal(IGame.Stage.Running, game.CurrentStage);
        Assert.Equal(0, game.ElapsedTicks);
        Assert.True(game.Players[0].IsAlive);
        Assert.True(game.Players[1].IsAlive);
        Assert.Null(game.Winner);
    }

    [Fact]
    public void Start_AfterGameStartEvent_IsRaised()
    {
        bool eventReceived = false;
        var game = IGame.Create();
        game.AfterGameStartEvent += (sender, args) =>
        {
            eventReceived = true;
        };
        game.Start();
        Assert.True(eventReceived);
    }
}
