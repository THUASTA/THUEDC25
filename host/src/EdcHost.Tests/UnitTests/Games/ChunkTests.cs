using EdcHost.Games;
using Xunit;

namespace EdcHost.Tests.UnitTests.Games;

public class ChunkTests
{
    public class MockPosition : IPosition<int>
    {
        public int X { get; set; }
        public int Y { get; set; }
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(7)]
    public void CanPlaceBlock_LessThanMaxHeight_ReturnsTrue(int height)
    {
        IChunk chunk = IChunk.Create(height, new MockPosition { X = 0, Y = 0 });

        bool actual = chunk.CanPlaceBlock;

        Assert.True(actual);
    }

    [Fact]
    public void CanPlaceBlock_EqualToMaxHeight_ReturnsFalse()
    {
        int MaxHeight = 8;
        IChunk chunk = IChunk.Create(MaxHeight, new MockPosition { X = 0, Y = 0 });

        bool actual = chunk.CanPlaceBlock;

        Assert.False(actual);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(7)]
    [InlineData(8)]
    public void CanRemoveBlock_MoreThanZero_ReturnsTrue(int height)
    {
        IChunk chunk = IChunk.Create(height, new MockPosition { X = 0, Y = 0 });

        bool actual = chunk.CanRemoveBlock;

        Assert.True(actual);
    }

    [Fact]
    public void CanRemoveBlock_Zero_ReturnsFalse()
    {
        IChunk chunk = IChunk.Create(0, new MockPosition { X = 0, Y = 0 });

        bool actual = chunk.CanRemoveBlock;

        Assert.False(actual);
    }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(1, 1)]
    [InlineData(2, 2)]
    [InlineData(3, 3)]
    [InlineData(7, 7)]
    [InlineData(8, 8)]
    public void Height_DoNothing_ReturnsConstructorValue(int height, int expectedHeight)
    {
        IChunk chunk = IChunk.Create(height, new MockPosition { X = 0, Y = 0 });

        int actual = chunk.Height;

        Assert.Equal(expectedHeight, actual);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(7)]
    [InlineData(8)]
    public void IsVoid_MoreThanZero_ReturnsFalse(int height)
    {
        IChunk chunk = IChunk.Create(height, new MockPosition { X = 0, Y = 0 });

        bool actual = chunk.IsVoid;

        Assert.False(actual);
    }

    [Fact]
    public void IsVoid_Zero_ReturnsTrue()
    {
        IChunk chunk = IChunk.Create(0, new MockPosition { X = 0, Y = 0 });

        bool actual = chunk.IsVoid;

        Assert.True(actual);
    }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(1, 1)]
    [InlineData(-1, -1)]
    [InlineData(int.MinValue, int.MinValue)]
    [InlineData(int.MaxValue, int.MaxValue)]
    public void Position_DoNothing_ReturnsConstructorValue(int x, int y)
    {
        IPosition<int> expected = IPosition<int>.Create(x, y);
        IChunk chunk = IChunk.Create(0, expected);

        IPosition<int> actual = chunk.Position;

        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(7)]
    public void PlaceBlock_LessThanMaxHeight_IncrementsHeight(int height)
    {
        IChunk chunk = IChunk.Create(height, new MockPosition { X = 0, Y = 0 });

        chunk.PlaceBlock();

        Assert.Equal(height + 1, chunk.Height);
    }

    [Fact]
    public void PlaceBlock_EqualToMaxHeight_ThrowsInvalidOperationException()
    {
        int MaxHeight = 8;
        IChunk chunk = IChunk.Create(MaxHeight, new MockPosition { X = 0, Y = 0 });

        Assert.Throws<InvalidOperationException>(() => chunk.PlaceBlock());
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(7)]
    [InlineData(8)]
    public void RemoveBlock_MoreThanZero_DecrementsHeight(int height)
    {
        IChunk chunk = IChunk.Create(height, new MockPosition { X = 0, Y = 0 });

        chunk.RemoveBlock();

        Assert.Equal(height - 1, chunk.Height);
    }

    [Fact]
    public void RemoveBlock_Zero_ThrowsInvalidOperationException()
    {
        IChunk chunk = IChunk.Create(0, new MockPosition { X = 0, Y = 0 });

        Assert.Throws<InvalidOperationException>(() => chunk.RemoveBlock());
    }
}
