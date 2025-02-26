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
	/// Length 12.
	/// </summary>
	[Serializable]
    public struct Vector3 : INetSerializable
    {
        [Preserve] public float X;
        [Preserve] public float Y;
        [Preserve] public float Z;

        [Preserve]
        public Vector3(float x, float y, float z)
		{
			X = x;
			Y = y;
			Z = z;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Is_XY_Zero()
		{
			return X == 0 && Y == 0;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Vector3 Normalized()
		{
			float magnitude = (float)Math.Sqrt(X * X + Y * Y + Z * Z);
			return new Vector3(X / magnitude, Y / magnitude, Z / magnitude);
		}

		#region Operators
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector3 operator -(Vector3 vec1, Vector3 vec2)
		{
			return new Vector3(vec1.X - vec2.X, vec1.Y - vec2.Y, vec1.Z - vec2.Z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector3 operator +(Vector3 vec1, Vector3 vec2)
		{
			return new Vector3(vec1.X + vec2.X, vec1.Y + vec2.Y, vec1.Z + vec2.Z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector3 operator *(Vector3 vec1, Vector3 vec2)
		{
			return new Vector3(vec1.X * vec2.X, vec1.Y * vec2.Y, vec1.Z * vec2.Z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector3 operator *(Vector3 vec1, float value)
		{
			return new Vector3(vec1.X * value, vec1.Y * value, vec1.Z * value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector3 operator /(Vector3 vec1, float value)
		{
			if (value == 0)
			{
				throw new DivideByZeroException();
			}
			return new Vector3(vec1.X / value, vec1.Y / value, vec1.Z / value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator ==(Vector3 vec1, Vector3 vec2)
		{
			return vec1.X == vec2.X && vec1.Y == vec2.Y && vec1.Z == vec2.Z;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator !=(Vector3 vec1, Vector3 vec2)
		{
			return !(vec1 == vec2);
		}

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(Vector3 other)
		{
			return X == other.X && Y == other.Y && Z == other.Z;
		}

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Equals(object obj)
		{
			return obj is Vector3 other && Equals(other);
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
			return $"{X:F}:{Y:F}:{Z:F}";
		}

        #region Serializable
		public void Serialize(NetDataWriter writer)
        {
            writer.Put(X);
			writer.Put(Y);
			writer.Put(Z);
        }

        public void Deserialize(NetDataReader reader)
        {
            X = reader.GetFloat();
			Y = reader.GetFloat();
			Z = reader.GetFloat();
        }
        #endregion
    }
}
