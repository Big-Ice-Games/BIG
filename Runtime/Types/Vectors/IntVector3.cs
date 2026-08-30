using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace BIG
{
    [Preserve, Serializable]
    [StructLayout(LayoutKind.Sequential)]
    [Newtonsoft.Json.JsonConverter(typeof(JsonConverter))]
    public partial struct IntVector3 : IEquatable<IntVector3>, ISerializable
    {
        public const int SERIALIZED_SIZE = sizeof(int) * 3;

        public static readonly IntVector3 Zero = new IntVector3();
        public static readonly IntVector3 One = new IntVector3(1, 1, 1);
        public static readonly IntVector3 MinusOne = new IntVector3(-1, -1, -1);

        public int X;
        public int Y;
        public int Z;

        public IntVector3(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        #region Utils
        public override string ToString() => $"{X}:{Y}:{Z}";

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsZero() => X == 0 && Y == 0 && Z == 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int SqrMagnitude() => X * X + Y * Y + Z * Z;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float Magnitude() => MathF.Sqrt(X * X + Y * Y + Z * Z);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Dot(in IntVector3 a, in IntVector3 b) => a.X * b.X + a.Y * b.Y + a.Z * b.Z;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SqrDistance(in IntVector3 a, in IntVector3 b)
        {
            int dx = a.X - b.X;
            int dy = a.Y - b.Y;
            int dz = a.Z - b.Z;
            return dx * dx + dy * dy + dz * dz;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Distance(in IntVector3 a, in IntVector3 b) => MathF.Sqrt(SqrDistance(a, b));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IntVector3 Min(in IntVector3 a, in IntVector3 b)
            => new IntVector3(Math.Min(a.X, b.X), Math.Min(a.Y, b.Y), Math.Min(a.Z, b.Z));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IntVector3 Max(in IntVector3 a, in IntVector3 b)
            => new IntVector3(Math.Max(a.X, b.X), Math.Max(a.Y, b.Y), Math.Max(a.Z, b.Z));

        public bool Equals(IntVector3 other) => X == other.X && Y == other.Y && Z == other.Z;

        public override bool Equals(object? obj) => obj is IntVector3 other && Equals(other);

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = X;
                hash = hash * 397 ^ Y;
                hash = hash * 397 ^ Z;
                return hash;
            }
        }
        #endregion

        #region Operators
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IntVector3 operator -(IntVector3 vec) => new IntVector3(-vec.X, -vec.Y, -vec.Z);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IntVector3 operator -(IntVector3 vec1, IntVector3 vec2)
            => new IntVector3(vec1.X - vec2.X, vec1.Y - vec2.Y, vec1.Z - vec2.Z);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IntVector3 operator +(IntVector3 vec1, IntVector3 vec2)
            => new IntVector3(vec1.X + vec2.X, vec1.Y + vec2.Y, vec1.Z + vec2.Z);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IntVector3 operator *(IntVector3 vec1, IntVector3 vec2)
            => new IntVector3(vec1.X * vec2.X, vec1.Y * vec2.Y, vec1.Z * vec2.Z);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IntVector3 operator *(IntVector3 vec1, int value)
            => new IntVector3(vec1.X * value, vec1.Y * value, vec1.Z * value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IntVector3 operator /(IntVector3 vec1, int value)
        {
            if (value == 0)
            {
                throw new DivideByZeroException();
            }
            return new IntVector3(vec1.X / value, vec1.Y / value, vec1.Z / value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(IntVector3 vec1, IntVector3 vec2)
            => vec1.X == vec2.X && vec1.Y == vec2.Y && vec1.Z == vec2.Z;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(IntVector3 vec1, IntVector3 vec2) => !(vec1 == vec2);
        #endregion

        #region Serialization
        public int SerializedSize => SERIALIZED_SIZE;

        public void Serialize(ref ByteWriter writer)
        {
            writer.WriteInt(X);
            writer.WriteInt(Y);
            writer.WriteInt(Z);
        }

        public void Deserialize(ref ByteReader reader)
        {
            X = reader.ReadInt();
            Y = reader.ReadInt();
            Z = reader.ReadInt();
        }

        #region Json
        /// <summary>
        /// Custom JSON format: "{x, y, z}" (InvariantCulture). Registered globally in <see cref="BIG.Json"/>.
        /// </summary>
        public sealed class JsonConverter : Newtonsoft.Json.JsonConverter<IntVector3>
        {
            public override IntVector3 ReadJson(Newtonsoft.Json.JsonReader reader, Type objectType, IntVector3 existingValue, bool hasExistingValue, Newtonsoft.Json.JsonSerializer serializer)
            {
                var parts = JsonVectorFormat.Split(reader.Value, 3);
                if (parts != null
                    && JsonVectorFormat.TryInt(parts[0], out int x)
                    && JsonVectorFormat.TryInt(parts[1], out int y)
                    && JsonVectorFormat.TryInt(parts[2], out int z))
                    return new IntVector3(x, y, z);

                return Zero;
            }

            public override void WriteJson(Newtonsoft.Json.JsonWriter writer, IntVector3 value, Newtonsoft.Json.JsonSerializer serializer)
                => writer.WriteValue(JsonVectorFormat.Format(value.X, value.Y, value.Z));
        }
        #endregion
        #endregion
    }
}
