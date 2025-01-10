namespace BIG.Types
{
    [Preserve]
    public sealed class Color
    {
        public static Color Default = new Color(0, 0, 0, 255);
        [Preserve] public byte Red { get; set; }
        [Preserve] public byte Green { get; set; }
        [Preserve] public byte Blue { get; set; }
        [Preserve] public byte Alpha { get; set; }

        [Preserve]
        public Color(byte red, byte green, byte blue, byte alpha)
        {
            Red = red;
            Green = green;
            Blue = blue;
            Alpha = alpha;
        }

        [Preserve]
        public Color() {}
    }
}
