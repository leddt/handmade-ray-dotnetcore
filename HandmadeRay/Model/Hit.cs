using System.Numerics;

namespace HandmadeRay.Model
{
    public struct Hit
    {
        public float Distance;
        public Vector3 Position;
        public Vector3 Normal;
        public Material Material;

        public static readonly Hit Miss = new Hit { Distance = float.MaxValue };
    }
}