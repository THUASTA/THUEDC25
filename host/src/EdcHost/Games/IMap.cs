namespace EdcHost.Games;

/// <summary>
/// Map represents the map of the game.
/// </summary>
public interface IMap
{
    static IMap Create(IPosition<int>[] spawnPoints)
    {
        return new Map(spawnPoints);
    }

    /// <summary>
    /// The list of chunks.
    /// </summary>
    List<IChunk> Chunks { get; }

    /// <summary>
    /// Gets the chunk at the position.
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    IChunk GetChunkAt(IPosition<int> position);
}
