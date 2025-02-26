#region license

// Copyright (c) 2025, Big Ice Games
// All rights reserved.

#endregion

using LiteNetLib.Utils;

namespace BIG
{
    [Preserve]
    public sealed class Color : INetSerializable
    {
        public static Color Red = new Color(255, 0, 0, 255);
        public static Color Blue = new Color(0, 0, 255, 255);
        public static Color Green = new Color(0, 255, 0, 255);
        public static Color Cyan = new Color(0, 255, 255, 255);
        public static Color Grey = new Color(125, 125, 125, 255);
        public static Color Magenta = new Color(255, 0, 255, 255);
        public static Color Yellow = new Color(255, 200, 20, 255);
        public static Color White = new Color(255, 255, 255, 255);
        public static Color Default = new Color(0, 0, 0, 255);

        [Preserve] public byte R { get; set; }
        [Preserve] public byte G { get; set; }
        [Preserve] public byte B { get; set; }
        [Preserve] public byte Alpha { get; set; }

        [Preserve]
        public Color(byte red, byte green, byte blue, byte alpha)
        {
            R = red;
            G = green;
            B = blue;
            Alpha = alpha;
        }

        [Preserve]
        public Color(Vector4Byte vector4Byte)
        {
            R = vector4Byte.X;
            G = vector4Byte.Y;
            B = vector4Byte.Z;
            Alpha = vector4Byte.W;
        }


        [Preserve]
        public Color(string fromString)
        {
            var rgba = fromString.Split(new char[] { ':' });
            R = byte.Parse(rgba[0]);
            G = byte.Parse(rgba[1]);
            B = byte.Parse(rgba[2]);
            Alpha = byte.Parse(rgba[3]);
        }

        [Preserve]
        public Color() {}

        #region Serialization
        public void Serialize(NetDataWriter writer)
        {
            writer.Put(Red);
            writer.Put(Green);
            writer.Put(Blue);
            writer.Put(Alpha);
        }

        public void Deserialize(NetDataReader reader)
        {
           R = reader.GetByte();
           G = reader.GetByte();
           B = reader.GetByte();
           Alpha = reader.GetByte();
        }
        #endregion
    }
}
