namespace EdcHost.Games;

public interface IChunk
{
    static IChunk Create(int height, IPosition<int> position)
    {
        return new Chunk(height, position);
    }

    /// <summary>
    /// Whether a block can be placed in the chunk.
    /// </summary>
    bool CanPlaceBlock { get; }
    /// <summary>
    /// Whether a block can be removed from the chunk.
    /// </summary>
    bool CanRemoveBlock { get; }
    /// <summary>
    /// The height of the chunk.
    /// </summary>
    int Height { get; }
    /// <summary>
    /// Whether the chunk is void.
    /// </summary>
    bool IsVoid { get; }
    /// <summary>
    /// The position of the chunk.
    /// </summary>
    IPosition<int> Position { get; }
    /// <summary>
    /// Which kind of ore the chunk owns.
    /// </summary>
    IMine.OreKindType OwningOreKind { get; set; }

    /// <summary>
    /// Places a block in the chunk.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the chunk cannot have a block placed in it.
    /// </exception>
    void PlaceBlock();
    /// <summary>
    /// Removes a block from the chunk.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the chunk cannot have a block removed from it.
    /// </exception>
    void RemoveBlock();
}
