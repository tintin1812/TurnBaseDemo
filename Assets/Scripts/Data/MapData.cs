namespace Data
{
    public struct Pet
    {
    }

    public class MapData
    {
        private int[,] _map;

        public MapData(int col, int row)
        {
            _map = new int[col, row];
        }
    }
}