namespace EdcHost.Games;

public class PlayerTradeEventArgs : EventArgs
{
    public IPlayer Player { get; }
    public IPlayer.CommodityKindType CommodityKind { get; }

    public PlayerTradeEventArgs(IPlayer player, IPlayer.CommodityKindType commodityKind)
    {
        Player = player;
        CommodityKind = commodityKind;
    }
}
