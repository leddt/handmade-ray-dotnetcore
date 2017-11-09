namespace HandmadeRay.Model.Shapes
{
    public interface IShape
    {
        Hit TestRay(Ray ray);
    }
}