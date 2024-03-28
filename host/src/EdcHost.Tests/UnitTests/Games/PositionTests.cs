using EdcHost.Games;
using Xunit;

namespace EdcHost.Tests.UnitTests.Games;

public class PositionTests
{
    [Theory]
    [InlineData(0, 0)]
    [InlineData(1, 1)]
    [InlineData(-1, -1)]
    [InlineData(int.MinValue, int.MinValue)]
    [InlineData(int.MaxValue, int.MaxValue)]
    public void IntX_DoNothing_ReturnsConstructorValue(int x, int expectedX)
    {
        IPosition<int> position = IPosition<int>.Create(x, 0);

        int actualX = position.X;

        Assert.Equal(expectedX, actualX);
    }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(1, 1)]
    [InlineData(-1, -1)]
    [InlineData(int.MinValue, int.MinValue)]
    [InlineData(int.MaxValue, int.MaxValue)]
    public void IntX_SingleNumber_ReturnsSameNumber(int x, int expectedX)
    {
#pragma warning disable IDE0017
        IPosition<int> position = IPosition<int>.Create(0, 0);

        position.X = x;
        int actualX = position.X;

        Assert.Equal(expectedX, actualX);
    }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(1, 1)]
    [InlineData(-1, -1)]
    [InlineData(int.MinValue, int.MinValue)]
    [InlineData(int.MaxValue, int.MaxValue)]
    public void IntY_DoNothing_ReturnsConstructorValue(int y, int expectedY)
    {
        IPosition<int> position = IPosition<int>.Create(0, y);

        int actualY = position.Y;

        Assert.Equal(expectedY, actualY);
    }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(1, 1)]
    [InlineData(-1, -1)]
    [InlineData(int.MinValue, int.MinValue)]
    [InlineData(int.MaxValue, int.MaxValue)]
    public void IntY_SingleNumber_ReturnsSameNumber(int y, int expectedY)
    {
#pragma warning disable IDE0017
        IPosition<int> position = IPosition<int>.Create(0, 0);

        position.Y = y;
        int actualY = position.Y;

        Assert.Equal(expectedY, actualY);
    }

    [Theory]
    [InlineData(0.0, 0.0)]
    [InlineData(0.1, 0.1)]
    [InlineData(-0.1, -0.1)]
    [InlineData(float.MinValue, float.MinValue)]
    [InlineData(float.MaxValue, float.MaxValue)]
    [InlineData(float.PositiveInfinity, float.PositiveInfinity)]
    [InlineData(float.NegativeInfinity, float.NegativeInfinity)]
    [InlineData(float.NaN, float.NaN)]
    [InlineData(float.Epsilon, float.Epsilon)]
    public void FloatX_DoNothing_ReturnsConstructorValue(float x, float expectedX)
    {
        IPosition<float> position = IPosition<float>.Create(x, 0);

        float actualX = position.X;

        Assert.Equal(expectedX, actualX);
    }

    [Theory]
    [InlineData(0.0, 0.0)]
    [InlineData(0.1, 0.1)]
    [InlineData(-0.1, -0.1)]
    [InlineData(float.MinValue, float.MinValue)]
    [InlineData(float.MaxValue, float.MaxValue)]
    [InlineData(float.PositiveInfinity, float.PositiveInfinity)]
    [InlineData(float.NegativeInfinity, float.NegativeInfinity)]
    [InlineData(float.NaN, float.NaN)]
    [InlineData(float.Epsilon, float.Epsilon)]
    public void FloatX_SingleNumber_ReturnsSameNumber(float x, float expectedX)
    {
#pragma warning disable IDE0017
        IPosition<float> position = IPosition<float>.Create(0, 0);

        position.X = x;
        float actualX = position.X;

        Assert.Equal(expectedX, actualX);
    }

    [Theory]
    [InlineData(0.0, 0.0)]
    [InlineData(0.1, 0.1)]
    [InlineData(-0.1, -0.1)]
    [InlineData(float.MinValue, float.MinValue)]
    [InlineData(float.MaxValue, float.MaxValue)]
    [InlineData(float.PositiveInfinity, float.PositiveInfinity)]
    [InlineData(float.NegativeInfinity, float.NegativeInfinity)]
    [InlineData(float.NaN, float.NaN)]
    [InlineData(float.Epsilon, float.Epsilon)]
    public void FloatY_DoNothing_ReturnsConstructorValue(float y, float expectedY)
    {
        IPosition<float> position = IPosition<float>.Create(0, y);

        float actualY = position.Y;

        Assert.Equal(expectedY, actualY);
    }

    [Theory]
    [InlineData(0.0, 0.0)]
    [InlineData(0.1, 0.1)]
    [InlineData(-0.1, -0.1)]
    [InlineData(float.MinValue, float.MinValue)]
    [InlineData(float.MaxValue, float.MaxValue)]
    [InlineData(float.PositiveInfinity, float.PositiveInfinity)]
    [InlineData(float.NegativeInfinity, float.NegativeInfinity)]
    [InlineData(float.NaN, float.NaN)]
    [InlineData(float.Epsilon, float.Epsilon)]
    public void FloatY_SingleNumber_ReturnsSameNumber(float y, float expectedY)
    {
#pragma warning disable IDE0017
        IPosition<float> position = IPosition<float>.Create(0, 0);

        position.Y = y;
        float actualY = position.Y;

        Assert.Equal(expectedY, actualY);
    }
}

