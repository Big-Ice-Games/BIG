using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace BIG
{
    /// <summary>
    /// Rotation quaternion. All angles are in RADIANS.
    /// Euler convention matches Unity: rotation order Z (roll), then X (pitch), then Y (yaw).
    /// </summary>
    [Preserve, Serializable]
    [StructLayout(LayoutKind.Sequential)]
    [Newtonsoft.Json.JsonConverter(typeof(JsonConverter))]
    public partial struct FloatQuaternion : IEquatable<FloatQuaternion>, ISerializable
    {
        public const int SERIALIZED_SIZE = sizeof(float) * 4;

        public static readonly FloatQuaternion Identity = new FloatQuaternion(0f, 0f, 0f, 1f);

        public float X;
        public float Y;
        public float Z;
        public float W;

        public FloatQuaternion(float x, float y, float z, float w)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }

        #region Utils
        public override string ToString() => $"{X:F}:{Y:F}:{Z:F}:{W:F}";

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Dot(in FloatQuaternion a, in FloatQuaternion b)
            => a.X * b.X + a.Y * b.Y + a.Z * b.Z + a.W * b.W;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float SqrMagnitude() => X * X + Y * Y + Z * Z + W * W;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float Magnitude() => MathF.Sqrt(X * X + Y * Y + Z * Z + W * W);

        /// <summary>
        /// Returns normalized copy of this quaternion or <see cref="Identity"/> when magnitude is zero.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public FloatQuaternion Normalized()
        {
            float sqrMagnitude = SqrMagnitude();
            if (sqrMagnitude < float.Epsilon) return Identity;
            float inverseMagnitude = 1f / MathF.Sqrt(sqrMagnitude);
            return new FloatQuaternion(X * inverseMagnitude, Y * inverseMagnitude, Z * inverseMagnitude, W * inverseMagnitude);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public FloatQuaternion Conjugate() => new FloatQuaternion(-X, -Y, -Z, W);

        /// <summary>
        /// Returns inverse rotation or <see cref="Identity"/> when magnitude is zero.
        /// For unit quaternions <see cref="Conjugate"/> is equivalent and cheaper.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public FloatQuaternion Inverse()
        {
            float sqrMagnitude = SqrMagnitude();
            if (sqrMagnitude < float.Epsilon) return Identity;
            float inverse = 1f / sqrMagnitude;
            return new FloatQuaternion(-X * inverse, -Y * inverse, -Z * inverse, W * inverse);
        }

        /// <summary>
        /// Rotation around a NORMALIZED axis by angle in radians.
        /// </summary>
        public static FloatQuaternion FromAxisAngle(in FloatVector3 axis, float angleRadians)
        {
            float half = angleRadians * 0.5f;
            float sin = MathF.Sin(half);
            return new FloatQuaternion(axis.X * sin, axis.Y * sin, axis.Z * sin, MathF.Cos(half));
        }

        /// <summary>
        /// Euler angles (radians) to quaternion. Rotation order matches Unity: Z (roll), then X (pitch), then Y (yaw).
        /// </summary>
        public static FloatQuaternion FromEuler(float xRadians, float yRadians, float zRadians)
        {
            float hx = xRadians * 0.5f;
            float hy = yRadians * 0.5f;
            float hz = zRadians * 0.5f;
            float sx = MathF.Sin(hx), cx = MathF.Cos(hx);
            float sy = MathF.Sin(hy), cy = MathF.Cos(hy);
            float sz = MathF.Sin(hz), cz = MathF.Cos(hz);

            return new FloatQuaternion(
                sx * cy * cz + cx * sy * sz,
                cx * sy * cz - sx * cy * sz,
                cx * cy * sz - sx * sy * cz,
                cx * cy * cz + sx * sy * sz);
        }

        /// <summary>
        /// Euler angles (radians) to quaternion.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FloatQuaternion FromEuler(in FloatVector3 eulerRadians)
            => FromEuler(eulerRadians.X, eulerRadians.Y, eulerRadians.Z);

        /// <summary>
        /// Normalized linear interpolation — cheap, good enough for small angle differences (e.g. network snapshots).
        /// T is clamped to [0, 1].
        /// </summary>
        public static FloatQuaternion Nlerp(in FloatQuaternion a, in FloatQuaternion b, float t)
        {
            t = Math.Clamp(t, 0f, 1f);
            // Take the shortest path.
            float sign = Dot(a, b) < 0f ? -1f : 1f;
            var result = new FloatQuaternion(
                a.X + (b.X * sign - a.X) * t,
                a.Y + (b.Y * sign - a.Y) * t,
                a.Z + (b.Z * sign - a.Z) * t,
                a.W + (b.W * sign - a.W) * t);
            return result.Normalized();
        }

        /// <summary>
        /// Spherical linear interpolation with constant angular velocity. T is clamped to [0, 1].
        /// </summary>
        public static FloatQuaternion Slerp(in FloatQuaternion a, in FloatQuaternion b, float t)
        {
            t = Math.Clamp(t, 0f, 1f);

            float dot = Dot(a, b);
            // Take the shortest path.
            float sign = 1f;
            if (dot < 0f)
            {
                dot = -dot;
                sign = -1f;
            }

            // Quaternions are nearly parallel — fall back to nlerp to avoid division by ~zero.
            if (dot > 0.9995f)
                return Nlerp(a, b, t);

            float theta = MathF.Acos(dot);
            float sinTheta = MathF.Sin(theta);
            float wa = MathF.Sin((1f - t) * theta) / sinTheta;
            float wb = MathF.Sin(t * theta) / sinTheta * sign;

            return new FloatQuaternion(
                a.X * wa + b.X * wb,
                a.Y * wa + b.Y * wb,
                a.Z * wa + b.Z * wb,
                a.W * wa + b.W * wb);
        }

        /// <summary>
        /// True when both quaternions represent nearly the same rotation (q and -q are the same rotation).
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ApproxEquals(in FloatQuaternion other, float epsilon = 1e-5f)
            => MathF.Abs(Dot(this, other)) >= 1f - epsilon;

        public bool Equals(FloatQuaternion other)
            => X == other.X && Y == other.Y && Z == other.Z && W == other.W;

        public override bool Equals(object? obj) => obj is FloatQuaternion other && Equals(other);

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = X.GetHashCode();
                hash = hash * 397 ^ Y.GetHashCode();
                hash = hash * 397 ^ Z.GetHashCode();
                hash = hash * 397 ^ W.GetHashCode();
                return hash;
            }
        }
        #endregion

        #region Operators
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FloatQuaternion operator *(FloatQuaternion a, FloatQuaternion b)
        {
            return new FloatQuaternion(
                a.W * b.X + a.X * b.W + a.Y * b.Z - a.Z * b.Y,
                a.W * b.Y + a.Y * b.W + a.Z * b.X - a.X * b.Z,
                a.W * b.Z + a.Z * b.W + a.X * b.Y - a.Y * b.X,
                a.W * b.W - a.X * b.X - a.Y * b.Y - a.Z * b.Z);
        }

        /// <summary>
        /// Rotate vector by this quaternion (quaternion must be normalized).
        /// </summary>
        public static FloatVector3 operator *(FloatQuaternion rotation, FloatVector3 vector)
        {
            // v' = v + 2 * cross(q.xyz, cross(q.xyz, v) + w * v)
            float tx = 2f * (rotation.Y * vector.Z - rotation.Z * vector.Y);
            float ty = 2f * (rotation.Z * vector.X - rotation.X * vector.Z);
            float tz = 2f * (rotation.X * vector.Y - rotation.Y * vector.X);

            return new FloatVector3(
                vector.X + rotation.W * tx + (rotation.Y * tz - rotation.Z * ty),
                vector.Y + rotation.W * ty + (rotation.Z * tx - rotation.X * tz),
                vector.Z + rotation.W * tz + (rotation.X * ty - rotation.Y * tx));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(FloatQuaternion a, FloatQuaternion b)
            => a.X == b.X && a.Y == b.Y && a.Z == b.Z && a.W == b.W;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(FloatQuaternion a, FloatQuaternion b) => !(a == b);
        #endregion

        #region Serialization
        public int SerializedSize => SERIALIZED_SIZE;

        public void Serialize(ref ByteWriter writer)
        {
            writer.WriteFloat(X);
            writer.WriteFloat(Y);
            writer.WriteFloat(Z);
            writer.WriteFloat(W);
        }

        public void Deserialize(ref ByteReader reader)
        {
            X = reader.ReadFloat();
            Y = reader.ReadFloat();
            Z = reader.ReadFloat();
            W = reader.ReadFloat();
        }

        #region Json
        /// <summary>
        /// Custom JSON format: "{x, y, z, w}" (InvariantCulture). Registered globally in <see cref="BIG.Json"/>.
        /// Broken input falls back to <see cref="Identity"/>.
        /// </summary>
        public sealed class JsonConverter : Newtonsoft.Json.JsonConverter<FloatQuaternion>
        {
            public override FloatQuaternion ReadJson(Newtonsoft.Json.JsonReader reader, Type objectType, FloatQuaternion existingValue, bool hasExistingValue, Newtonsoft.Json.JsonSerializer serializer)
            {
                var parts = JsonVectorFormat.Split(reader.Value, 4);
                if (parts != null
                    && JsonVectorFormat.TryFloat(parts[0], out float x)
                    && JsonVectorFormat.TryFloat(parts[1], out float y)
                    && JsonVectorFormat.TryFloat(parts[2], out float z)
                    && JsonVectorFormat.TryFloat(parts[3], out float w))
                    return new FloatQuaternion(x, y, z, w);

                return Identity;
            }

            public override void WriteJson(Newtonsoft.Json.JsonWriter writer, FloatQuaternion value, Newtonsoft.Json.JsonSerializer serializer)
                => writer.WriteValue(JsonVectorFormat.Format(value.X, value.Y, value.Z, value.W));
        }
        #endregion
        #endregion
    }
}
