namespace EdcHost.Games;

class Chunk : IChunk
{
    const int MaxHeight = 8;

    public bool CanPlaceBlock => (Height < MaxHeight);
    public bool CanRemoveBlock => (Height > 0);
    public int Height { get; private set; }
    public bool IsVoid => (Height == 0);
    public IPosition<int> Position { get; }

    public IMine.OreKindType OwningOreKind { get; set; } = IMine.OreKindType.None;

    /// <summary>
    /// Creates a new chunk.
    /// </summary>
    /// <param name="height">Height of the chunk</param>
    /// <param name="position">Position of the chunk</param>
    public Chunk(int height, IPosition<int> position)
    {
        Height = height;
        Position = position;
    }

    /// <summary>
    /// Places a block in the chunk.
    /// </summary>
    public void PlaceBlock()
    {
        if (CanPlaceBlock == false)
        {
            throw new InvalidOperationException("cannot place block in chunk");
        }

        Height++;
    }

    /// <summary>
    /// Removes a block from the chunk.
    /// </summary>
    public void RemoveBlock()
    {
        if (CanRemoveBlock == false)
        {
            throw new InvalidOperationException("cannot remove block from chunk");
        }

        Height--;
    }
}
