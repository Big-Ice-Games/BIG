using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace BIG
{
    [Preserve, Serializable]
    [StructLayout(LayoutKind.Sequential)]
    [Newtonsoft.Json.JsonConverter(typeof(JsonConverter))]
    public partial struct ByteVector2 : IEquatable<ByteVector2>, ISerializable
    {
        public const int SERIALIZED_SIZE = sizeof(byte) * 2;

        public static readonly ByteVector2 Zero = new ByteVector2();
        public static readonly ByteVector2 One = new ByteVector2(1, 1);
        public static readonly ByteVector2 Max = new ByteVector2(byte.MaxValue, byte.MaxValue);

        public byte X;
        public byte Y;

        public ByteVector2(byte x, byte y)
        {
            X = x;
            Y = y;
        }

        #region Utils
        public override string ToString() => $"{X}:{Y}";

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsZero() => X == 0 && Y == 0;

        public bool Equals(ByteVector2 other) => X == other.X && Y == other.Y;

        public override bool Equals(object? obj) => obj is ByteVector2 other && Equals(other);

        public override int GetHashCode() => X << 8 | Y;
        #endregion

        #region Operators
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(ByteVector2 vec1, ByteVector2 vec2)
            => vec1.X == vec2.X && vec1.Y == vec2.Y;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(ByteVector2 vec1, ByteVector2 vec2) => !(vec1 == vec2);
        #endregion

        #region Serialization
        public int SerializedSize => SERIALIZED_SIZE;

        public void Serialize(ref ByteWriter writer)
        {
            writer.WriteByte(X);
            writer.WriteByte(Y);
        }

        public void Deserialize(ref ByteReader reader)
        {
            X = reader.ReadByte();
            Y = reader.ReadByte();
        }

        #region Json
        /// <summary>
        /// Custom JSON format: "{x, y}" (InvariantCulture). Registered globally in <see cref="BIG.Json"/>.
        /// </summary>
        public sealed class JsonConverter : Newtonsoft.Json.JsonConverter<ByteVector2>
        {
            public override ByteVector2 ReadJson(Newtonsoft.Json.JsonReader reader, Type objectType, ByteVector2 existingValue, bool hasExistingValue, Newtonsoft.Json.JsonSerializer serializer)
            {
                var parts = JsonVectorFormat.Split(reader.Value, 2);
                if (parts != null
                    && JsonVectorFormat.TryByte(parts[0], out byte x)
                    && JsonVectorFormat.TryByte(parts[1], out byte y))
                    return new ByteVector2(x, y);

                return Zero;
            }

            public override void WriteJson(Newtonsoft.Json.JsonWriter writer, ByteVector2 value, Newtonsoft.Json.JsonSerializer serializer)
                => writer.WriteValue(JsonVectorFormat.Format(value.X, value.Y));
        }
        #endregion
        #endregion
    }
}
