﻿using HandmadeRay.Model.Shapes;

namespace HandmadeRay.Model
{
    public class World
    {
        public Material Background;
        public IShape[] Shapes;
        public Camera Camera;
        public Options Options;

        public long BoucesComputed;
        public int TilesRendered;
        public int TotalTileCount;
    }
}