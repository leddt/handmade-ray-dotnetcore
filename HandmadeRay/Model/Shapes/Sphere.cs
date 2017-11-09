using System;
using System.Numerics;

namespace HandmadeRay.Model.Shapes
{
    public struct Sphere : IShape
    {
        public Material Material;
        public Vector3 Position;
        public float Radius;

        public Hit TestRay(Ray ray)
        {
            var sphereRelativeRayOrigin = ray.Origin - Position;

            var a = Vector3.Dot(ray.Direction, ray.Direction);
            var b = 2 * Vector3.Dot(ray.Direction, sphereRelativeRayOrigin);
            var c = Vector3.Dot(sphereRelativeRayOrigin, sphereRelativeRayOrigin) - Radius * Radius;

            var sqrt = MathF.Sqrt(b * b - 4f * a * c);
            if (MathF.Abs(sqrt) < Constants.TOLERANCE) return Hit.Miss;

            var near = (-b + sqrt) / 2f * a;
            var far = (-b - sqrt) / 2f * a;

            var result = near;
            if (far > 0.001f && far < near)
                result = far;
            
            Hit hit;
            hit.Distance = result;
            hit.Position = result * ray.Origin;
            hit.Normal = Vector3.Normalize(result * ray.Direction + sphereRelativeRayOrigin);
            hit.Material = Material;

            return hit;
        }
    }
}