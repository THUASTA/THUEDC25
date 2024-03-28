namespace EdcHost.Games;

/// <summary>
/// Mine represents a mine in the game.
/// </summary>
public interface IMine
{
    static IMine Create(OreKindType oreKind, IPosition<int> position, int tick)
    {
        return new Mine(oreKind, position, tick);
    }

    /// <summary>
    /// The ore kind.
    /// </summary>
    enum OreKindType
    {
        IronIngot = 0,
        GoldIngot = 1,
        Diamond = 2,
        None = 3
    }
    /// <summary>
    /// The id of the mine
    /// </summary>
    Guid MineId { get; }

    /// <summary>
    /// The count of accumulated ores.
    /// </summary>
    int AccumulatedOreCount { get; }

    /// <summary>
    /// The kind of the ore.
    /// </summary>
    OreKindType OreKind { get; }

    /// <summary>
    /// The position of the mine.
    /// </summary>
    IPosition<int> Position { get; }

    /// <summary>
    /// How much time required to generate ore.
    /// </summary>
    int AccumulateOreInterval { get; }

    /// <summary>
    /// Last time ore generated.
    /// </summary>
    int LastOreGeneratedTick { get; }

    /// <summary>
    /// Picks up some ore.
    /// </summary>
    /// <param name="count">The count of ore to pick up.</param>
    void PickUpOre(int count);

    /// <summary>
    /// Generate ore automaticly.
    /// </summary>
    void GenerateOre(int tick);
}
