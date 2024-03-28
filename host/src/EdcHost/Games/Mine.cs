namespace EdcHost.Games;

/// <summary>
/// Mine represents a mine in the game.
/// </summary>
class Mine : IMine
{
    /// <summary>
    /// The id of the mine
    /// </summary>
    public Guid MineId { get; }

    /// <summary>
    /// The count of accumulated ores.
    /// </summary>
    public int AccumulatedOreCount { get; private set; }

    /// <summary>
    /// The kind of the ore.
    /// </summary>
    public IMine.OreKindType OreKind { get; }

    /// <summary>
    /// The position of the mine.
    /// </summary>
    public IPosition<int> Position { get; }

    public int AccumulateOreInterval
    {
        get => 10 * Game.TicksPerSecondExpected;
    }

    /// <summary>
    /// Last time ore generated.
    /// </summary>
    public int LastOreGeneratedTick { get; private set; }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="oreKind">The ore kind</param>
    /// <param name="position">The position</param>
    public Mine(IMine.OreKindType oreKind, IPosition<int> position, int tick)
    {
        MineId = Guid.NewGuid();
        AccumulatedOreCount = 0;
        OreKind = oreKind;
        Position = position;
        LastOreGeneratedTick = tick;
    }

    /// <summary>
    /// Picks up some ore.
    /// </summary>
    /// <param name="count">The count of ore to pick up.</param>
    public void PickUpOre(int count)
    {
        if (AccumulatedOreCount < count)
        {
            throw new InvalidOperationException("No enough ore.");
        }
        AccumulatedOreCount -= count;
    }

    /// <summary>
    /// Generate ore automaticly.
    /// </summary>
    public void GenerateOre(int tick)
    {
        AccumulatedOreCount++;
        LastOreGeneratedTick = tick;
    }

}
