namespace EdcHost.Games;

/// <summary>
/// Position represents a position in 2D space.
/// </summary>
/// <typeparam name="T">The type of the position.</typeparam>
public interface IPosition<T>
{
    static IPosition<T> Create(T x, T y) => new Position<T>(x, y);

    /// <summary>
    /// The X coordinate of the position.
    /// </summary>
    T X { get; set; }

    /// <summary>
    /// The Y coordinate of the position.
    /// </summary>
    T Y { get; set; }
}
