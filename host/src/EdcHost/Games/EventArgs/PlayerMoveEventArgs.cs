namespace EdcHost.Games;

public class PlayerMoveEventArgs : EventArgs
{
    /// <summary>
    /// The player that moved.
    /// </summary>
    public IPlayer Player { get; }
    /// <summary>
    /// The position of the player before the movement.
    /// </summary>
    public IPosition<float> PositionBeforeMovement { get; }
    /// <summary>
    /// The position of the player after the movement.
    /// </summary>
    public IPosition<float> Position { get; }

    public PlayerMoveEventArgs(IPlayer player, IPosition<float> positionBeforeMovement, IPosition<float> position)
    {
        Player = player;
        PositionBeforeMovement = positionBeforeMovement;
        Position = position;
    }
}
