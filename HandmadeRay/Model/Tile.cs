namespace HandmadeRay.Model
{
    public struct Tile
    {
        public int MinX;
        public int MinY;
        public int MaxX;
        public int MaxY;

        public Tile(int minX, int minY, int maxX, int maxY)
        {
            MinX = minX;
            MinY = minY;
            MaxX = maxX;
            MaxY = maxY;
        }
    }
}