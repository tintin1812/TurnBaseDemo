using UnityEngine;

namespace Plugins.PathFinding
{
    public class Tile
    {
        public int Row { get; private set; }
        public int Col { get; private set; }
        public int Weight { get; set; }
        public int Cost { get; set; }
        public Tile PrevTile { get; set; }

        public Tile(int row, int col, int weight)
        {
            Row = row;
            Col = col;
            Weight = weight;
        }

        public Vector2 ToVector2()
        {
            return new Vector2(Col, Row);
        }
    }
}