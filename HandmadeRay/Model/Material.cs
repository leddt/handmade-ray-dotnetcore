using System.Numerics;

namespace HandmadeRay.Model
{
    public struct Material
    {
        public Vector3 EmitColor;
        public Vector3 ReflectColor;
        public float Scatter;
    }
}