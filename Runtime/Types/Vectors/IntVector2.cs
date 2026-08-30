using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace BIG
{
    [Serializable, Preserve]
    [StructLayout(LayoutKind.Sequential)]
    [Newtonsoft.Json.JsonConverter(typeof(JsonConverter))]
    public partial struct IntVector2 : IEquatable<IntVector2>, ISerializable
    {
        public const int SERIALIZED_SIZE = sizeof(int) * 2;

        public static readonly IntVector2 Zero = new IntVector2();
        public static readonly IntVector2 One = new IntVector2(1, 1);
        public static readonly IntVector2 MinusOne = new IntVector2(-1, -1);

        public int X;
        public int Y;

        public IntVector2(int x, int y)
        {
            X = x;
            Y = y;
        }

        #region Utils
        public override string ToString() => $"{X}:{Y}";

        /// <summary>
        /// Returns normalized copy of this vector (components truncated to ints) or <see cref="Zero"/> when magnitude is zero.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IntVector2 Normalized()
        {
            int sqrMagnitude = X * X + Y * Y;
            if (sqrMagnitude == 0) return Zero;
            float magnitude = MathF.Sqrt(sqrMagnitude);
            return new IntVector2((int)(X / magnitude), (int)(Y / magnitude));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsZero() => X == 0 && Y == 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int SqrMagnitude() => X * X + Y * Y;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float Magnitude() => MathF.Sqrt(X * X + Y * Y);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Dot(in IntVector2 a, in IntVector2 b) => a.X * b.X + a.Y * b.Y;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SqrDistance(in IntVector2 a, in IntVector2 b)
        {
            int dx = a.X - b.X;
            int dy = a.Y - b.Y;
            return dx * dx + dy * dy;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Distance(in IntVector2 a, in IntVector2 b) => MathF.Sqrt(SqrDistance(a, b));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IntVector2 Min(in IntVector2 a, in IntVector2 b)
            => new IntVector2(Math.Min(a.X, b.X), Math.Min(a.Y, b.Y));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IntVector2 Max(in IntVector2 a, in IntVector2 b)
            => new IntVector2(Math.Max(a.X, b.X), Math.Max(a.Y, b.Y));

        #endregion

        #region Operators
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IntVector2 operator -(IntVector2 vec) => new IntVector2(-vec.X, -vec.Y);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IntVector2 operator -(IntVector2 vec1, IntVector2 intVector2)
        {
            return new IntVector2(vec1.X - intVector2.X, vec1.Y - intVector2.Y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IntVector2 operator +(IntVector2 vec1, IntVector2 intVector2)
        {
            return new IntVector2(vec1.X + intVector2.X, vec1.Y + intVector2.Y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IntVector2 operator *(IntVector2 vec1, IntVector2 intVector2)
        {
            return new IntVector2(vec1.X * intVector2.X, vec1.Y * intVector2.Y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IntVector2 operator *(IntVector2 vec1, int value)
        {
            return new IntVector2(vec1.X * value, vec1.Y * value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IntVector2 operator /(IntVector2 vec1, int value)
        {
            if (value == 0)
            {
                throw new DivideByZeroException();
            }
            return new IntVector2(vec1.X / value, vec1.Y / value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(IntVector2 vec1, IntVector2 intVector2)
        {
            return vec1.X == intVector2.X && vec1.Y == intVector2.Y;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(IntVector2 vec1, IntVector2 intVector2)
        {
            return !(vec1 == intVector2);
        }

        public bool Equals(IntVector2 other)
        {
            return X.Equals(other.X) && Y.Equals(other.Y);
        }

        public override bool Equals(object? obj)
        {
            return obj is IntVector2 other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return X.GetHashCode() * 397 ^ Y.GetHashCode();
            }
        }
        #endregion

        #region Serialization
        public int SerializedSize => SERIALIZED_SIZE;

        public void Serialize(ref ByteWriter writer)
        {
            writer.WriteInt(X);
            writer.WriteInt(Y);
        }

        public void Deserialize(ref ByteReader reader)
        {
            X = reader.ReadInt();
            Y = reader.ReadInt();
        }

        #region Json
        /// <summary>
        /// Custom JSON format: "{x, y}" (InvariantCulture). Registered globally in <see cref="BIG.Json"/>.
        /// </summary>
        public sealed class JsonConverter : Newtonsoft.Json.JsonConverter<IntVector2>
        {
            public override IntVector2 ReadJson(Newtonsoft.Json.JsonReader reader, Type objectType, IntVector2 existingValue, bool hasExistingValue, Newtonsoft.Json.JsonSerializer serializer)
            {
                var parts = JsonVectorFormat.Split(reader.Value, 2);
                if (parts != null
                    && JsonVectorFormat.TryInt(parts[0], out int x)
                    && JsonVectorFormat.TryInt(parts[1], out int y))
                    return new IntVector2(x, y);

                return Zero;
            }

            public override void WriteJson(Newtonsoft.Json.JsonWriter writer, IntVector2 value, Newtonsoft.Json.JsonSerializer serializer)
                => writer.WriteValue(JsonVectorFormat.Format(value.X, value.Y));
        }
        #endregion
        #endregion
    }
}
