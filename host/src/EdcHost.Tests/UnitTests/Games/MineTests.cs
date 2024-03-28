using EdcHost.Games;
using Xunit;

namespace EdcHost.Tests.UnitTests.Games;

public interface Iint
{
    int GetCurrentint();
}

public class IMineTest
{
    public class MockPosition : IPosition<int>
    {
        public int X { get; set; }
        public int Y { get; set; }
    }

    [Fact]
    public void IntAccumulatedOreCount_NotGenerate_ReturnsCorrectValue()
    {
        IMine mine = IMine.Create(IMine.OreKindType.Diamond, new MockPosition { X = 0, Y = 0 }, 0);
        Assert.Equal(0, mine.AccumulatedOreCount);
    }

    [Theory]
    [InlineData(IMine.OreKindType.IronIngot, IMine.OreKindType.IronIngot)]
    [InlineData(IMine.OreKindType.GoldIngot, IMine.OreKindType.GoldIngot)]
    [InlineData(IMine.OreKindType.Diamond, IMine.OreKindType.Diamond)]
    public void OreKind_ReturnsCorrectValue(IMine.OreKindType oreKindType, IMine.OreKindType expectedOreKind)
    {
        IMine mine = IMine.Create(oreKindType, new MockPosition { X = 0, Y = 0 }, 0);
        Assert.Equal(expectedOreKind, mine.OreKind);
    }

    [Theory]
    [InlineData(3, 3)]
    [InlineData(4, 4)]
    [InlineData(30, 30)]
    [InlineData(100, 100)]
    [InlineData(200, 200)]
    public void GenerateOre_AccumulatedOreCountAdd_ReturnsCorrectValue(int generate, int expectedValue)
    {
        IMine mine = IMine.Create(IMine.OreKindType.Diamond, new MockPosition { X = 0, Y = 0 }, 0);
        for (int i = 0; i < generate; i++)
        {
            mine.GenerateOre(0);
        }
        Assert.Equal(expectedValue, mine.AccumulatedOreCount);
    }

    [Fact]
    public void PickUpOre_CountLessThanAccumulatedOreCount_ReturnsCorrctValue()
    {
        IMine mine = IMine.Create(IMine.OreKindType.Diamond, new MockPosition { X = 0, Y = 0 }, 0);
        for (int i = 0; i < 200; i++)
        {
            mine.GenerateOre(0);
        }
        int count = 64;
        int expectedValue = 136;
        mine.PickUpOre(count);
        Assert.Equal(expectedValue, mine.AccumulatedOreCount);
    }

    [Fact]
    public void PickUpOre_CountMoreThanAccumulatedOreCount_ReturnsCorrctValue()
    {
        IMine mine = IMine.Create(IMine.OreKindType.Diamond, new MockPosition { X = 0, Y = 0 }, 0);
        for (int i = 0; i < 30; i++)
        {
            mine.GenerateOre(0);
        }
        int count = 60;
        InvalidOperationException ex = Assert.Throws<InvalidOperationException>(() => { mine.PickUpOre(count); });
        Assert.Equal("No enough ore.", ex.Message);
    }

}
