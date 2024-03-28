using System.Collections.Concurrent;

namespace EdcHost.Games;

partial class Game : IGame
{
    const int TicksBeforeRespawn = 300;

    /// <summary>
    /// Maximum count of same type of items a player can hold.
    /// </summary>
    public const int MaximumItemCount = 128;

    /// <summary>
    /// The damage which will kill a player instantly.
    /// </summary>
    const int InstantDeathDamage = 114514;

    /// <summary>
    /// This means a player hasn't done something (for example, Attack) yet
    /// after game started.
    /// </summary>
    const int Never = -10000;

    /// <summary>
    /// Number of players.
    /// </summary>
    public int PlayerNum { get; }

    /// <summary>
    /// All players.
    /// </summary>
    public List<IPlayer> Players { get; private set; }

    /// <summary>
    /// Whether all beds are destroyed or not.
    /// </summary>
    bool _isAllBedsDestroyed;

    readonly List<int?> _playerDeathTickList;
    readonly List<int> _playerLastAttackTickList;

    readonly ConcurrentQueue<EventArgs> _playerEventQueue;

    int AttackTickInterval(IPlayer player)
    {
        return (int)(Math.Max(8.5 - 0.25 * player.ActionPoints, 0.5) * TicksPerSecondExpected);
    }

}
