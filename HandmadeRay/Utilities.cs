using System;
using System.Drawing;
using System.Numerics;

namespace HandmadeRay
{
    public static class Utilities
    {
        public static float ExactLinearToSRGB(float linearValue)
        {
            if (linearValue < 0f) linearValue = 0f;
            if (linearValue > 1f) linearValue = 1f;

            return linearValue <= 0.0031308f
                ? linearValue * 12.92f
                : 1.055f * MathF.Pow(linearValue, 1f / 2.4f) - 0.055f;
        }

        public static Color Linear1ToSRGB255(Vector3 linearColor)
        {
            return Color.FromArgb(
                (int)(255f * ExactLinearToSRGB(linearColor.X)),
                (int)(255f * ExactLinearToSRGB(linearColor.Y)),
                (int)(255f * ExactLinearToSRGB(linearColor.Z))
            );
        }

        public static float RandomBilateral(Random rng)
        {
            return (float)rng.NextDouble() * 2 - 1;
        }

        public static Vector3 RandomBilateralVector(Random rng)
        {
            return new Vector3(RandomBilateral(rng), RandomBilateral(rng), RandomBilateral(rng));
        }

        public static Vector3 Hadamard(Vector3 a, Vector3 b)
        {
            return new Vector3(a.X * b.X, a.Y * b.Y, a.Z * b.Z);
        }
    }
}