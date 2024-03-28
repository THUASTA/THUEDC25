namespace EdcHost.Games;

partial class Game : IGame
{
    /// <summary>
    /// Convert a float position to an int position.
    /// </summary>
    /// <param name="floatPosition">The float position</param>
    /// <returns>The int position</returns>
    IPosition<int> ToIntPosition(IPosition<float> floatPosition)
    {
        int x = (floatPosition.X < -1e-6) ? -(1 + (int)(-floatPosition.X)) : (int)floatPosition.X;
        int y = (floatPosition.Y < -1e-6) ? -(1 + (int)(-floatPosition.Y)) : (int)floatPosition.Y;
        return new Position<int>(x, y);
    }

    IPosition<float> ToFloatPosition(IPosition<int> intPosition)
    {
        float offset = 0.5f;
        return new Position<float>(offset + (float)intPosition.X, offset + (float)intPosition.Y);
    }

    /// <summary>
    /// Whether the position is a valid position or not.
    /// </summary>
    /// <param name="position">The position</param>
    /// <returns>True if valid, false otherwise</returns>
    bool IsValidPosition(IPosition<int> position)
    {
        return (position.X >= 0 && position.X < 8 && position.Y >= 0 && position.Y < 8);
    }

    /// <summary>
    /// Whether two positions is adjacent or not.
    /// </summary>
    /// <param name="position1">First position</param>
    /// <param name="position2">Second position</param>
    /// <returns>True if adjacant, false otherwise</returns>
    bool IsAdjacent(IPosition<int> position1, IPosition<int> position2)
    {
        return (Math.Abs(position1.X - position2.X) <= 1
            && Math.Abs(position1.Y - position2.Y) <= 1);
    }

    /// <summary>
    /// Whether two positions is same or not.
    /// </summary>
    /// <param name="position1">First position</param>
    /// <param name="position2">Second position</param>
    /// <returns>True if same, fale otherwise</returns>
    bool IsSamePosition(IPosition<int> position1, IPosition<int> position2)
    {
        return (position1.X == position2.X && position1.Y == position2.Y);
    }

    /// <summary>
    /// Eucilid distance between two positions.
    /// </summary>
    /// <param name="position1">First position</param>
    /// <param name="position2">Second position</param>
    /// <returns>THe eucilid distance</returns>
    double EucilidDistance(IPosition<float> position1, IPosition<float> position2)
    {
        return Math.Sqrt((position1.X - position2.X) * (position1.X - position2.X)
            + (position1.Y - position2.Y) * (position1.Y - position2.Y));
    }
}
