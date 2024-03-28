namespace EdcHost.Games;

/// <summary>
/// Position represents a position in 2D space.
/// </summary>
/// <typeparam name="T">The type of the position.</typeparam>
struct Position<T> : IPosition<T>
{
    /// <summary>
    /// The X coordinate of the position.
    /// </summary>
    public T X { get; set; }

    /// <summary>
    /// The Y coordinate of the position.
    /// </summary>
    public T Y { get; set; }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="x">The x coordinate</param>
    /// <param name="y">The y coordinate</param>
    public Position(T x, T y)
    {
        X = x;
        Y = y;
    }

}
