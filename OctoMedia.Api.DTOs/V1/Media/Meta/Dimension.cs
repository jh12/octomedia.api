namespace OctoMedia.Api.DTOs.V1.Media.Meta
{
    public class Dimension
    {
        public int Height { get; set; }
        public int Width { get; set; }

        public Dimension()
        {
        }

        public Dimension(int height, int width)
        {
            Height = height;
            Width = width;
        }
    }
}