using System;
using System.Numerics;

namespace HandmadeRay.Model.Shapes
{
    public struct Plane : IShape
    {
        public Material Material;
        public Vector3 Normal;
        public float Distance;

        public Hit TestRay(Ray ray)
        {
            var denominator = Vector3.Dot(Normal, ray.Direction);
            if (MathF.Abs(denominator) < Constants.TOLERANCE) return Hit.Miss;

            var hitDistance = (-Distance - Vector3.Dot(Normal, ray.Origin)) / denominator;

            Hit hit;
            hit.Distance = hitDistance;
            hit.Position = ray.Origin + hitDistance * ray.Direction;
            hit.Normal = Normal;
            hit.Material = Material;

            return hit;
        }
    }
}