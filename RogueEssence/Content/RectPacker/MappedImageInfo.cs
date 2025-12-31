namespace RectPacker
{
    /// <summary>
    /// Represents an image that has been mapped to a specific position in a sprite atlas.
    /// Contains the X and Y coordinates within the atlas and a reference to the source image.
    /// </summary>
    public class MappedImageInfo
    {
        public int X { get; private set; }
        public int Y { get; private set; }
        public ImageInfo ImageInfo { get; private set; }

        public MappedImageInfo(int x, int y, ImageInfo imageInfo)
        {
            X = x;
            Y = y;
            ImageInfo = imageInfo;
        }
    }
}
