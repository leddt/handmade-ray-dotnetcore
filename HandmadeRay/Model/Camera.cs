using System.Numerics;

namespace HandmadeRay.Model
{
    public struct Camera
    {
        public Vector3 Position;

        public Vector3 Z;
        public Vector3 X;
        public Vector3 Y;

        public float FilmDist;
        public float FilmWidth;
        public float FilmHeight;

        public int BitmapWidth;
        public int BitmapHeight;
    }
}