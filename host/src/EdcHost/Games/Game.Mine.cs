namespace EdcHost.Games;

partial class Game : IGame
{
    /// <summary>
    /// Generate mines according to game config
    /// </summary>
    /// <param name="diamondMines">Diamond mine list</param>
    /// <param name="goldMines">Gold mine list</param>
    /// <param name="ironMines">Iron mine list</param>
    private void GenerateMines(List<Tuple<int, int>>? diamondMines,
        List<Tuple<int, int>>? goldMines, List<Tuple<int, int>>? ironMines)
    {
        if (diamondMines is not null)
        {
            foreach (Tuple<int, int> minePos in diamondMines)
            {
                var position = new Position<int>(minePos.Item1, minePos.Item2);
                var mine = new Mine(IMine.OreKindType.Diamond, position, 0);
                Mines.Add(mine);
                GameMap.GetChunkAt(position).OwningOreKind = IMine.OreKindType.Diamond;
            }
        }
        if (goldMines is not null)
        {
            foreach (Tuple<int, int> minePos in goldMines)
            {
                var position = new Position<int>(minePos.Item1, minePos.Item2);
                var mine = new Mine(IMine.OreKindType.GoldIngot, position, 0);
                Mines.Add(mine);
                GameMap.GetChunkAt(position).OwningOreKind = IMine.OreKindType.GoldIngot;
            }
        }
        if (ironMines is not null)
        {
            foreach (Tuple<int, int> minePos in ironMines)
            {
                var position = new Position<int>(minePos.Item1, minePos.Item2);
                var mine = new Mine(IMine.OreKindType.IronIngot, position, 0);
                Mines.Add(mine);
                GameMap.GetChunkAt(position).OwningOreKind = IMine.OreKindType.IronIngot;
            }
        }
    }
}
