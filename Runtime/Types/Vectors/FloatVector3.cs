using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace BIG
{
    [Preserve, Serializable]
    [StructLayout(LayoutKind.Sequential)]
    [Newtonsoft.Json.JsonConverter(typeof(JsonConverter))]
    public partial struct FloatVector3 : IEquatable<FloatVector3>, ISerializable
    {
        public const int SERIALIZED_SIZE = sizeof(float) * 3;

        public static readonly FloatVector3 Zero = new FloatVector3(0, 0, 0);
        public static readonly FloatVector3 One = new FloatVector3(1, 1, 1);

        public float X;
        public float Y;
        public float Z;

        public FloatVector3(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        #region Utils
        public override string ToString()
        {
            return $"{X:F}:{Y:F}:{Z:F}";
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Is_XY_Zero() => X == 0 && Y == 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsZero() => X == 0 && Y == 0 && Z == 0;

        /// <summary>
        /// Returns normalized copy of this vector or <see cref="Zero"/> when magnitude is zero.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public FloatVector3 Normalized()
        {
            float sqrMagnitude = X * X + Y * Y + Z * Z;
            if (sqrMagnitude < float.Epsilon) return Zero;
            float magnitude = MathF.Sqrt(sqrMagnitude);
            return new FloatVector3(X / magnitude, Y / magnitude, Z / magnitude);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float SqrMagnitude() => X * X + Y * Y + Z * Z;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float Magnitude() => MathF.Sqrt(X * X + Y * Y + Z * Z);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Dot(in FloatVector3 a, in FloatVector3 b) => a.X * b.X + a.Y * b.Y + a.Z * b.Z;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FloatVector3 Cross(in FloatVector3 a, in FloatVector3 b)
            => new FloatVector3(
                a.Y * b.Z - a.Z * b.Y,
                a.Z * b.X - a.X * b.Z,
                a.X * b.Y - a.Y * b.X);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float SqrDistance(in FloatVector3 a, in FloatVector3 b)
        {
            float dx = a.X - b.X;
            float dy = a.Y - b.Y;
            float dz = a.Z - b.Z;
            return dx * dx + dy * dy + dz * dz;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Distance(in FloatVector3 a, in FloatVector3 b) => MathF.Sqrt(SqrDistance(a, b));

        /// <summary>
        /// Linear interpolation between a and b. T is clamped to [0, 1].
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FloatVector3 Lerp(in FloatVector3 a, in FloatVector3 b, float t)
        {
            t = Math.Clamp(t, 0f, 1f);
            return new FloatVector3(a.X + (b.X - a.X) * t, a.Y + (b.Y - a.Y) * t, a.Z + (b.Z - a.Z) * t);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FloatVector3 LerpUnclamped(in FloatVector3 a, in FloatVector3 b, float t)
            => new FloatVector3(a.X + (b.X - a.X) * t, a.Y + (b.Y - a.Y) * t, a.Z + (b.Z - a.Z) * t);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FloatVector3 Min(in FloatVector3 a, in FloatVector3 b)
            => new FloatVector3(MathF.Min(a.X, b.X), MathF.Min(a.Y, b.Y), MathF.Min(a.Z, b.Z));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FloatVector3 Max(in FloatVector3 a, in FloatVector3 b)
            => new FloatVector3(MathF.Max(a.X, b.X), MathF.Max(a.Y, b.Y), MathF.Max(a.Z, b.Z));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ApproxEquals(in FloatVector3 other, float epsilon = 1e-5f)
            => MathF.Abs(X - other.X) <= epsilon && MathF.Abs(Y - other.Y) <= epsilon && MathF.Abs(Z - other.Z) <= epsilon;

        public bool Equals(FloatVector3 other)
        {
            return X == other.X && Y == other.Y && Z == other.Z;
        }

        public override bool Equals(object obj)
        {
            return obj is FloatVector3 other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = X.GetHashCode();
                hash = hash * 397 ^ Y.GetHashCode();
                hash = hash * 397 ^ Z.GetHashCode();
                return hash;
            }
        }
        #endregion

        #region Operators
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FloatVector3 operator -(FloatVector3 vec) => new FloatVector3(-vec.X, -vec.Y, -vec.Z);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FloatVector3 operator -(FloatVector3 vec1, FloatVector3 vec2)
        {
            return new FloatVector3(vec1.X - vec2.X, vec1.Y - vec2.Y, vec1.Z - vec2.Z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FloatVector3 operator +(FloatVector3 vec1, FloatVector3 vec2)
        {
            return new FloatVector3(vec1.X + vec2.X, vec1.Y + vec2.Y, vec1.Z + vec2.Z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FloatVector3 operator *(FloatVector3 vec1, FloatVector3 vec2)
        {
            return new FloatVector3(vec1.X * vec2.X, vec1.Y * vec2.Y, vec1.Z * vec2.Z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FloatVector3 operator *(FloatVector3 vec1, float value)
        {
            return new FloatVector3(vec1.X * value, vec1.Y * value, vec1.Z * value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FloatVector3 operator /(FloatVector3 vec1, float value)
        {
            if (value == 0)
            {
                throw new DivideByZeroException();
            }
            return new FloatVector3(vec1.X / value, vec1.Y / value, vec1.Z / value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(FloatVector3 vec1, FloatVector3 vec2)
        {
            return vec1.X == vec2.X && vec1.Y == vec2.Y && vec1.Z == vec2.Z;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(FloatVector3 vec1, FloatVector3 vec2)
        {
            return !(vec1 == vec2);
        }
        #endregion

        #region Serialization
        public int SerializedSize => SERIALIZED_SIZE;

        public void Serialize(ref ByteWriter writer)
        {
            writer.WriteFloat(X);
            writer.WriteFloat(Y);
            writer.WriteFloat(Z);
        }

        public void Deserialize(ref ByteReader reader)
        {
            X = reader.ReadFloat();
            Y = reader.ReadFloat();
            Z = reader.ReadFloat();
        }

        #region Json
        /// <summary>
        /// Custom JSON format: "{x, y, z}" (InvariantCulture). Registered globally in <see cref="BIG.Json"/>.
        /// </summary>
        public sealed class JsonConverter : Newtonsoft.Json.JsonConverter<FloatVector3>
        {
            public override FloatVector3 ReadJson(Newtonsoft.Json.JsonReader reader, Type objectType, FloatVector3 existingValue, bool hasExistingValue, Newtonsoft.Json.JsonSerializer serializer)
            {
                var parts = JsonVectorFormat.Split(reader.Value, 3);
                if (parts != null
                    && JsonVectorFormat.TryFloat(parts[0], out float x)
                    && JsonVectorFormat.TryFloat(parts[1], out float y)
                    && JsonVectorFormat.TryFloat(parts[2], out float z))
                    return new FloatVector3(x, y, z);

                return Zero;
            }

            public override void WriteJson(Newtonsoft.Json.JsonWriter writer, FloatVector3 value, Newtonsoft.Json.JsonSerializer serializer)
                => writer.WriteValue(JsonVectorFormat.Format(value.X, value.Y, value.Z));
        }
        #endregion
        #endregion
    }
}
