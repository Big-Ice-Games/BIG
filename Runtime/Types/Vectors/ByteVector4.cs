using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace BIG
{
    /// <summary>
    /// Byte vector usable also as an RGBA color (see <see cref="R"/>, <see cref="G"/>, <see cref="B"/>, <see cref="A"/> aliases).
    /// </summary>
    [Preserve, Serializable]
    [StructLayout(LayoutKind.Sequential)]
    [Newtonsoft.Json.JsonConverter(typeof(JsonConverter))]
    public partial struct ByteVector4 : IEquatable<ByteVector4>, ISerializable
    {
        public const int SERIALIZED_SIZE = sizeof(byte) * 4;

        public static readonly ByteVector4 Zero = new ByteVector4();
        public static readonly ByteVector4 One = new ByteVector4(1, 1, 1, 1);
        public static readonly ByteVector4 White = new ByteVector4(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
        public static readonly ByteVector4 Black = new ByteVector4(0, 0, 0, byte.MaxValue);
        public static readonly ByteVector4 Clear = new ByteVector4(0, 0, 0, 0);

        public byte X;
        public byte Y;
        public byte Z;
        public byte W;

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

        /// <summary> Color alias of <see cref="W"/>. </summary>
        public byte A
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)] readonly get => W;
            [MethodImpl(MethodImplOptions.AggressiveInlining)] set => W = value;
        }

        public ByteVector4(byte x, byte y, byte z, byte w)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }

        #region Utils
        public override string ToString() => $"{X}:{Y}:{Z}:{W}";

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsZero() => X == 0 && Y == 0 && Z == 0 && W == 0;

        /// <summary>
        /// Component-wise linear interpolation (e.g. color blending). T is clamped to [0, 1].
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ByteVector4 Lerp(in ByteVector4 a, in ByteVector4 b, float t)
        {
            t = Math.Clamp(t, 0f, 1f);
            return new ByteVector4(
                (byte)(a.X + (b.X - a.X) * t + 0.5f),
                (byte)(a.Y + (b.Y - a.Y) * t + 0.5f),
                (byte)(a.Z + (b.Z - a.Z) * t + 0.5f),
                (byte)(a.W + (b.W - a.W) * t + 0.5f));
        }

        public bool Equals(ByteVector4 other) => X == other.X && Y == other.Y && Z == other.Z && W == other.W;

        public override bool Equals(object? obj) => obj is ByteVector4 other && Equals(other);

        public override int GetHashCode() => X << 24 | Y << 16 | Z << 8 | W;
        #endregion

        #region Operators
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(ByteVector4 vec1, ByteVector4 vec2)
            => vec1.X == vec2.X && vec1.Y == vec2.Y && vec1.Z == vec2.Z && vec1.W == vec2.W;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(ByteVector4 vec1, ByteVector4 vec2) => !(vec1 == vec2);
        #endregion

        #region Serialization
        public int SerializedSize => SERIALIZED_SIZE;

        public void Serialize(ref ByteWriter writer)
        {
            writer.WriteByte(X);
            writer.WriteByte(Y);
            writer.WriteByte(Z);
            writer.WriteByte(W);
        }

        public void Deserialize(ref ByteReader reader)
        {
            X = reader.ReadByte();
            Y = reader.ReadByte();
            Z = reader.ReadByte();
            W = reader.ReadByte();
        }

        #region Json
        /// <summary>
        /// Custom JSON format: "{x, y, z, w}" (InvariantCulture). Registered globally in <see cref="BIG.Json"/>.
        /// </summary>
        public sealed class JsonConverter : Newtonsoft.Json.JsonConverter<ByteVector4>
        {
            public override ByteVector4 ReadJson(Newtonsoft.Json.JsonReader reader, Type objectType, ByteVector4 existingValue, bool hasExistingValue, Newtonsoft.Json.JsonSerializer serializer)
            {
                var parts = JsonVectorFormat.Split(reader.Value, 4);
                if (parts != null
                    && JsonVectorFormat.TryByte(parts[0], out byte x)
                    && JsonVectorFormat.TryByte(parts[1], out byte y)
                    && JsonVectorFormat.TryByte(parts[2], out byte z)
                    && JsonVectorFormat.TryByte(parts[3], out byte w))
                    return new ByteVector4(x, y, z, w);

                return Zero;
            }

            public override void WriteJson(Newtonsoft.Json.JsonWriter writer, ByteVector4 value, Newtonsoft.Json.JsonSerializer serializer)
                => writer.WriteValue(JsonVectorFormat.Format(value.X, value.Y, value.Z, value.W));
        }
        #endregion
        #endregion
    }
}
