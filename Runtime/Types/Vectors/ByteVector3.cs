using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace BIG
{
    /// <summary>
    /// Byte vector usable also as an RGB color (see <see cref="R"/>, <see cref="G"/>, <see cref="B"/> aliases).
    /// </summary>
    [Preserve, Serializable]
    [StructLayout(LayoutKind.Sequential)]
    [Newtonsoft.Json.JsonConverter(typeof(JsonConverter))]
    public partial struct ByteVector3 : IEquatable<ByteVector3>, ISerializable
    {
        public const int SERIALIZED_SIZE = sizeof(byte) * 3;

        public static readonly ByteVector3 Zero = new ByteVector3();
        public static readonly ByteVector3 One = new ByteVector3(1, 1, 1);
        public static readonly ByteVector3 White = new ByteVector3(byte.MaxValue, byte.MaxValue, byte.MaxValue);
        public static readonly ByteVector3 Black = new ByteVector3(0, 0, 0);

        public byte X;
        public byte Y;
        public byte Z;

        /// <summary> Color alias of <see cref="X"/>. </summary>
        public byte R
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)] readonly get => X;
            [MethodImpl(MethodImplOptions.AggressiveInlining)] set => X = value;
        }

        /// <summary> Color alias of <see cref="Y"/>. </summary>
        public byte G
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)] readonly get => Y;
            [MethodImpl(MethodImplOptions.AggressiveInlining)] set => Y = value;
        }

        /// <summary> Color alias of <see cref="Z"/>. </summary>
        public byte B
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)] readonly get => Z;
            [MethodImpl(MethodImplOptions.AggressiveInlining)] set => Z = value;
        }

        public ByteVector3(byte x, byte y, byte z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        #region Utils
        public override string ToString() => $"{X}:{Y}:{Z}";

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsZero() => X == 0 && Y == 0 && Z == 0;

        /// <summary>
        /// Component-wise linear interpolation (e.g. color blending). T is clamped to [0, 1].
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ByteVector3 Lerp(in ByteVector3 a, in ByteVector3 b, float t)
        {
            t = Math.Clamp(t, 0f, 1f);
            return new ByteVector3(
                (byte)(a.X + (b.X - a.X) * t + 0.5f),
                (byte)(a.Y + (b.Y - a.Y) * t + 0.5f),
                (byte)(a.Z + (b.Z - a.Z) * t + 0.5f));
        }

        public bool Equals(ByteVector3 other) => X == other.X && Y == other.Y && Z == other.Z;

        public override bool Equals(object? obj) => obj is ByteVector3 other && Equals(other);

        public override int GetHashCode() => X << 16 | Y << 8 | Z;
        #endregion

        #region Operators
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(ByteVector3 vec1, ByteVector3 vec2)
            => vec1.X == vec2.X && vec1.Y == vec2.Y && vec1.Z == vec2.Z;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(ByteVector3 vec1, ByteVector3 vec2) => !(vec1 == vec2);
        #endregion

        #region Serialization
        public int SerializedSize => SERIALIZED_SIZE;

        public void Serialize(ref ByteWriter writer)
        {
            writer.WriteByte(X);
            writer.WriteByte(Y);
            writer.WriteByte(Z);
        }

        public void Deserialize(ref ByteReader reader)
        {
            X = reader.ReadByte();
            Y = reader.ReadByte();
            Z = reader.ReadByte();
        }

        #region Json
        /// <summary>
        /// Custom JSON format: "{x, y, z}" (InvariantCulture). Registered globally in <see cref="BIG.Json"/>.
        /// </summary>
        public sealed class JsonConverter : Newtonsoft.Json.JsonConverter<ByteVector3>
        {
            public override ByteVector3 ReadJson(Newtonsoft.Json.JsonReader reader, Type objectType, ByteVector3 existingValue, bool hasExistingValue, Newtonsoft.Json.JsonSerializer serializer)
            {
                var parts = JsonVectorFormat.Split(reader.Value, 3);
                if (parts != null
                    && JsonVectorFormat.TryByte(parts[0], out byte x)
                    && JsonVectorFormat.TryByte(parts[1], out byte y)
                    && JsonVectorFormat.TryByte(parts[2], out byte z))
                    return new ByteVector3(x, y, z);

                return Zero;
            }

            public override void WriteJson(Newtonsoft.Json.JsonWriter writer, ByteVector3 value, Newtonsoft.Json.JsonSerializer serializer)
                => writer.WriteValue(JsonVectorFormat.Format(value.X, value.Y, value.Z));
        }
        #endregion
        #endregion
    }
}
