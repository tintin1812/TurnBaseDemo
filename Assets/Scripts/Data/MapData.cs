namespace Data
{
    public class MapData
    {
        public enum TitleType
        {
            Empty,
            Wall,
            Attacker,
            Defender
        }

        private int[] Map { get; set; }
        public int Rows { get; private set; }
        public int Cols { get; private set; }

        public MapData(int col, int row)
        {
            Map = new int[col * row];
        }

        public MapData(short[,] tiles)
        {
            Cols = tiles.GetLength(1);
            Rows = tiles.GetLength(0);
            Map = new int[Cols * Rows];
            for (var col = 0; col < Cols; col++)
            {
                for (var row = 0; row < Rows; row++)
                {
                    Map[GetTileIndex(row, col)] = tiles[row, col];
                }
            }
        }

        public int GetTileIndex(int row, int col)
        {
            return row * Cols + col;
        }

        public TitleType GetTile(int row, int col)
        {
            return (TitleType)Map[GetTileIndex(row, col)];
        }
    }
}