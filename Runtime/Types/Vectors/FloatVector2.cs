using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace BIG
{
    [Preserve, Serializable]
    [StructLayout(LayoutKind.Sequential)]
    [Newtonsoft.Json.JsonConverter(typeof(JsonConverter))]
    public partial struct FloatVector2 : IEquatable<FloatVector2>, ISerializable
    {
        public const int SERIALIZED_SIZE = sizeof(float) * 2;

        public static readonly FloatVector2 Zero = new FloatVector2(0f, 0f);
        public static readonly FloatVector2 One = new FloatVector2(1f, 1f);

        public float X;
        public float Y;

        public FloatVector2(float x, float y)
        {
            X = x;
            Y = y;
        }

        #region Utils
        public override string ToString() => $"{X:F}:{Y:F}";

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsZero() => X == 0 && Y == 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float SqrMagnitude() => X * X + Y * Y;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float Magnitude() => MathF.Sqrt(X * X + Y * Y);

        /// <summary>
        /// Returns normalized copy of this vector or <see cref="Zero"/> when magnitude is zero.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public FloatVector2 Normalized()
        {
            float sqrMagnitude = X * X + Y * Y;
            if (sqrMagnitude < float.Epsilon) return Zero;
            float magnitude = MathF.Sqrt(sqrMagnitude);
            return new FloatVector2(X / magnitude, Y / magnitude);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Dot(in FloatVector2 a, in FloatVector2 b) => a.X * b.X + a.Y * b.Y;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float SqrDistance(in FloatVector2 a, in FloatVector2 b)
        {
            float dx = a.X - b.X;
            float dy = a.Y - b.Y;
            return dx * dx + dy * dy;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Distance(in FloatVector2 a, in FloatVector2 b) => MathF.Sqrt(SqrDistance(a, b));

        /// <summary>
        /// Linear interpolation between a and b. T is clamped to [0, 1].
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FloatVector2 Lerp(in FloatVector2 a, in FloatVector2 b, float t)
        {
            t = Math.Clamp(t, 0f, 1f);
            return new FloatVector2(a.X + (b.X - a.X) * t, a.Y + (b.Y - a.Y) * t);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FloatVector2 LerpUnclamped(in FloatVector2 a, in FloatVector2 b, float t)
            => new FloatVector2(a.X + (b.X - a.X) * t, a.Y + (b.Y - a.Y) * t);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FloatVector2 Min(in FloatVector2 a, in FloatVector2 b)
            => new FloatVector2(MathF.Min(a.X, b.X), MathF.Min(a.Y, b.Y));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FloatVector2 Max(in FloatVector2 a, in FloatVector2 b)
            => new FloatVector2(MathF.Max(a.X, b.X), MathF.Max(a.Y, b.Y));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ApproxEquals(in FloatVector2 other, float epsilon = 1e-5f)
            => MathF.Abs(X - other.X) <= epsilon && MathF.Abs(Y - other.Y) <= epsilon;

        public bool Equals(FloatVector2 other) => X == other.X && Y == other.Y;

        public override bool Equals(object? obj) => obj is FloatVector2 other && Equals(other);

        public override int GetHashCode()
        {
            unchecked
            {
                return X.GetHashCode() * 397 ^ Y.GetHashCode();
            }
        }
        #endregion

        #region Operators
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FloatVector2 operator -(FloatVector2 vec) => new FloatVector2(-vec.X, -vec.Y);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FloatVector2 operator -(FloatVector2 vec1, FloatVector2 vec2)
            => new FloatVector2(vec1.X - vec2.X, vec1.Y - vec2.Y);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FloatVector2 operator +(FloatVector2 vec1, FloatVector2 vec2)
            => new FloatVector2(vec1.X + vec2.X, vec1.Y + vec2.Y);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FloatVector2 operator *(FloatVector2 vec1, FloatVector2 vec2)
            => new FloatVector2(vec1.X * vec2.X, vec1.Y * vec2.Y);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FloatVector2 operator *(FloatVector2 vec1, float value)
            => new FloatVector2(vec1.X * value, vec1.Y * value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FloatVector2 operator /(FloatVector2 vec1, float value)
        {
            if (value == 0)
            {
                throw new DivideByZeroException();
            }
            return new FloatVector2(vec1.X / value, vec1.Y / value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(FloatVector2 vec1, FloatVector2 vec2)
            => vec1.X == vec2.X && vec1.Y == vec2.Y;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(FloatVector2 vec1, FloatVector2 vec2) => !(vec1 == vec2);
        #endregion

        #region Serialization
        public int SerializedSize => SERIALIZED_SIZE;

        public void Serialize(ref ByteWriter writer)
        {
            writer.WriteFloat(X);
            writer.WriteFloat(Y);
        }

        public void Deserialize(ref ByteReader reader)
        {
            X = reader.ReadFloat();
            Y = reader.ReadFloat();
        }

        #region Json
        /// <summary>
        /// Custom JSON format: "{x, y}" (InvariantCulture). Registered globally in <see cref="BIG.Json"/>.
        /// </summary>
        public sealed class JsonConverter : Newtonsoft.Json.JsonConverter<FloatVector2>
        {
            public override FloatVector2 ReadJson(Newtonsoft.Json.JsonReader reader, Type objectType, FloatVector2 existingValue, bool hasExistingValue, Newtonsoft.Json.JsonSerializer serializer)
            {
                var parts = JsonVectorFormat.Split(reader.Value, 2);
                if (parts != null
                    && JsonVectorFormat.TryFloat(parts[0], out float x)
                    && JsonVectorFormat.TryFloat(parts[1], out float y))
                    return new FloatVector2(x, y);

                return Zero;
            }

            public override void WriteJson(Newtonsoft.Json.JsonWriter writer, FloatVector2 value, Newtonsoft.Json.JsonSerializer serializer)
                => writer.WriteValue(JsonVectorFormat.Format(value.X, value.Y));
        }
        #endregion
        #endregion
    }
}
