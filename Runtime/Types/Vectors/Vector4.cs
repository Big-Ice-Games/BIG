#region license

// Copyright (c) 2025, Big Ice Games
// All rights reserved.

#endregion

using System;
using System.Runtime.CompilerServices;
using LiteNetLib.Utils;

namespace BIG
{
	/// <summary>
	/// Length 16.
	/// </summary>
	[Serializable]
	public struct Vector4 : INetSerializable
	{
        [Preserve] public float X;
        [Preserve] public float Y;
        [Preserve] public float Z;
        [Preserve] public float W;

        [Preserve]
        public Vector4(float x, float y, float z, float w)
		{
			X = x;
			Y = y;
			Z = z;
			W = w;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Vector4 Normalized()
		{
			float magnitude = (float)Math.Sqrt(X * X + Y * Y + Z * Z + W * W);
			return new Vector4(X / magnitude, Y / magnitude, Z / magnitude, W / magnitude);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Is_XZ_Zero()
		{
			return X == 0 && Z == 0;
		}

		#region Operators
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector4 operator -(Vector4 vec1, Vector4 vec2)
		{
			return new Vector4(vec1.X - vec2.X, vec1.Y - vec2.Y, vec1.Z - vec2.Z, vec1.W - vec2.W);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector4 operator +(Vector4 vec1, Vector4 vec2)
		{
			return new Vector4(vec1.X + vec2.X, vec1.Y + vec2.Y, vec1.Z + vec2.Z, vec1.W + vec2.W);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector4 operator *(Vector4 vec1, Vector4 vec2)
		{
			return new Vector4(vec1.X * vec2.X, vec1.Y * vec2.Y, vec1.Z * vec2.Z, vec1.W * vec2.W);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector4 operator *(Vector4 vec1, float value)
		{
			return new Vector4(vec1.X * value, vec1.Y * value, vec1.Z * value, vec1.W * value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator Vector3(Vector4 vector)
		{
			return new Vector3(vector.X, vector.Y, vector.Z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector4 operator /(Vector4 vec1, float value)
		{
			if (value == 0)
			{
				throw new DivideByZeroException();
			}
			return new Vector4(vec1.X / value, vec1.Y / value, vec1.Z / value, vec1.W / value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator ==(Vector4 vec1, Vector4 vec2)
		{
			return vec1.X == vec2.X && vec1.Y == vec2.Y && vec1.Z == vec2.Z && vec1.W == vec2.W;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator !=(Vector4 vec1, Vector4 vec2)
		{
			return !(vec1 == vec2);
		}

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(Vector4 other)
		{
			return X == other.X && Y == other.Y && Z == other.Z && W == other.W;
		}

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Equals(object obj)
		{
			return obj is Vector4 other && Equals(other);
		}

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode()
		{
			unchecked
			{
				return (X.GetHashCode() * 397) ^ Y.GetHashCode();
			}
		}
        #endregion

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override string ToString()
		{
			return $"{X}:{Y}:{Z}:{W}";
		}

        #region Serializable

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(X);
            writer.Put(Y);
            writer.Put(Z);
            writer.Put(W);
        }

        public void Deserialize(NetDataReader reader)
        {
            X = reader.GetFloat();
            Y = reader.GetFloat();
            Z = reader.GetFloat();
            W = reader.GetFloat();
        }
        #endregion
    }
}
