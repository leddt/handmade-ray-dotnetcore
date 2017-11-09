using System;
using System.Diagnostics;
using System.Drawing;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;

using HandmadeRay.Model;
using HandmadeRay.Model.Shapes;

using static HandmadeRay.Constants;
using static HandmadeRay.Utilities;

namespace HandmadeRay
{
    public static class Constants
    {
        public const int RAYS_PER_PIXEL = 32;
        public const int MAX_BOUNCES = 8;
        public const float MIN_HIT_DISTANCE = 0.001f;
        public const float TOLERANCE = 0.0001f;
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            var bitmap = new Bitmap(1920, 1080);

            var world = MakeWorld();
            world.Camera = MakeCamera(new Vector3(0, -10, 1), bitmap.Width, bitmap.Height);

            var sw = new Stopwatch();
            sw.Start();
            Render(world, bitmap);
            sw.Stop();
            
            bitmap.Save("output.bmp");
            
            Console.WriteLine($"\rDone! ({sw.ElapsedMilliseconds} ms)                      ");
            Console.WriteLine($"{world.BoucesComputed:N0} bounces");
            Console.WriteLine($"{world.BoucesComputed / sw.ElapsedMilliseconds} bounces/ms");
            Thread.Sleep(2000);
        }

        private static World MakeWorld()
        {
            return new World {
                Background = {EmitColor = new Vector3(.3f, .4f, .5f)},
                Shapes = new IShape[] {
                    new Model.Shapes.Plane {
                        Material = {ReflectColor = new Vector3(.5f, .5f, .5f)},
                        Normal = Vector3.UnitZ,
                        Distance = 0
                    },
                    new Sphere {
                        Material = {ReflectColor = new Vector3(.7f, .5f, .3f)},
                        Position = Vector3.Zero,
                        Radius = 1
                    },
                    new Sphere {
                        Material = {EmitColor = new Vector3(4f, 0, 0)},
                        Position = new Vector3(3, -2, 0),
                        Radius = 1
                    },
                    new Sphere {
                        Material = {ReflectColor = new Vector3(.2f, .8f, .2f), Scatter = .7f},
                        Position = new Vector3(-2, -1, 2),
                        Radius = 1
                    },
                    new Sphere {
                        Material = {ReflectColor = new Vector3(.4f, .8f, .9f), Scatter = .85f},
                        Position = new Vector3(1, -1, 3),
                        Radius = 1
                    },
                    new Sphere {
                        Material = {ReflectColor = new Vector3(.95f, .95f, .95f), Scatter = 1f},
                        Position = new Vector3(-2, 3, 0),
                        Radius = 2
                    }
                }
            };
        }

        private static Camera MakeCamera(Vector3 position, int bitmapWidth, int bitmapHeight)
        {
            Camera camera;

            camera.Position = position;

            camera.Z = Vector3.Normalize(camera.Position);
            camera.X = Vector3.Normalize(Vector3.Cross(Vector3.UnitZ, camera.Z));
            camera.Y = Vector3.Normalize(Vector3.Cross(camera.Z, camera.X));

            camera.FilmDist = 1;
            camera.FilmWidth = 1;
            camera.FilmHeight = 1;

            camera.BitmapWidth = bitmapWidth;
            camera.BitmapHeight = bitmapHeight;

            var aspectRatio = bitmapWidth / (float)bitmapHeight;
            if (aspectRatio > 1)
                camera.FilmHeight = camera.FilmWidth / aspectRatio;
            else
                camera.FilmWidth = camera.FilmHeight * aspectRatio;

            return camera;
        }

        private static void Render(World world, Bitmap bitmap)
        {
            var results = new Color[bitmap.Width, bitmap.Height];

            var tileWidth = 64;
            var tileHeight = 64;

            var tileCountX = (bitmap.Width + tileWidth - 1) / tileWidth;
            var tileCountY = (bitmap.Height + tileHeight - 1) / tileHeight;
            world.TotalTileCount = tileCountX * tileCountY;

            Console.WriteLine($"{RAYS_PER_PIXEL} rays/px, {MAX_BOUNCES} max bounces");
            Console.WriteLine($"{tileCountX*tileCountY} tiles, {tileWidth}x{tileHeight}px");

            Parallel.For(0, tileCountY, tileY => {
                var minY = tileY * tileHeight;
                var maxY = minY + tileHeight;
                if (maxY > bitmap.Height) maxY = bitmap.Height;

                Parallel.For(0, tileCountX, tileX => {
                    var minX = tileX * tileWidth;
                    var maxX = minX + tileWidth;
                    if (maxX > bitmap.Width) maxX = bitmap.Width;

                    var tile = new Tile(minX, minY, maxX, maxY);

                    RenderTile(world, tile, results);
                });
            });

            for (var y = 0; y < bitmap.Height; y++)
            for (var x = 0; x < bitmap.Width; x++)
                bitmap.SetPixel(x, bitmap.Height - y - 1, results[x, y]);
        }

        private static void RenderTile(World world, Tile tile, Color[,] results)
        {
            var bitmapWidth = world.Camera.BitmapWidth;
            var bitmapHeight = world.Camera.BitmapHeight;

            var rng = new Random();
            var halfPixelWidth = .5f / bitmapWidth;
            var halfPixelHeight = .5f / bitmapHeight;
            var camera = world.Camera;
            
            var filmCenter = camera.Position - camera.FilmDist * camera.Z;

            var bounces = 0;

            for (var y = tile.MinY; y < tile.MaxY; y++)
            {
                var filmY = -1f + 2f * (y / (float) bitmapHeight);

                for (var x = tile.MinX; x < tile.MaxX; x++)
                {
                    var filmX = -1f + 2f * (x / (float) bitmapWidth);

                    var color = Vector3.Zero;
                    var contrib = 1f / RAYS_PER_PIXEL;
                    for (var rayIndex = 0; rayIndex < RAYS_PER_PIXEL; rayIndex++)
                    {
                        var offX = filmX + RandomBilateral(rng) * halfPixelWidth;
                        var offY = filmY + RandomBilateral(rng) * halfPixelHeight;

                        var filmPosition = filmCenter + offX * camera.FilmWidth / 2f * camera.X +
                                           offY * camera.FilmHeight / 2f * camera.Y;

                        var ray = new Ray {
                            Origin = camera.Position,
                            Direction = Vector3.Normalize(filmPosition - camera.Position)
                        };

                        var castResult = RayCast(world, ray, rng);

                        color += contrib * castResult.Color;
                        bounces += castResult.Bounces;
                    }

                    results[x, y] = Linear1ToSRGB255(color);
                }
            }

            Interlocked.Add(ref world.BoucesComputed, bounces);
            Interlocked.Increment(ref world.TilesRendered);

            Console.Write($"\rRendering... {world.TilesRendered}/{world.TotalTileCount} ({world.TilesRendered * 100 / world.TotalTileCount}%)");
        }

        private static CastResult RayCast(World world, Ray ray, Random rng)
        {
            var result = new CastResult();
            var attenuation = Vector3.One;

            for (var bounceCount = 0; bounceCount < MAX_BOUNCES; bounceCount++)
            {
                result.Bounces++;

                var bestHit = Hit.Miss;

                foreach (var shape in world.Shapes)
                {
                    var hit = shape.TestRay(ray);
                    if (hit.Distance > MIN_HIT_DISTANCE && hit.Distance < bestHit.Distance)
                    {
                        bestHit = hit;
                    }
                }

                if (bestHit.Distance < float.MaxValue)
                {
                    result.Color += Hadamard(attenuation, bestHit.Material.EmitColor);

                    var cosAtten = Vector3.Dot(-ray.Direction, bestHit.Normal);
                    if (cosAtten < 0) cosAtten = 0;

                    attenuation = Hadamard(attenuation, cosAtten * bestHit.Material.ReflectColor);

                    var pureBounce = ray.Direction - 2 * Vector3.Dot(ray.Direction, bestHit.Normal) * bestHit.Normal;
                    var randomBounce = Vector3.Normalize(bestHit.Normal + RandomBilateralVector(rng));

                    ray = new Ray {
                        Origin = ray.Origin + bestHit.Distance* ray.Direction,
                        Direction = Vector3.Normalize(Vector3.Lerp(randomBounce, pureBounce, bestHit.Material.Scatter))
                    };
                }
                else
                {
                    result.Color += Hadamard(attenuation, world.Background.EmitColor);
                    break;
                }
            }

            return result;
        }
    }
}
