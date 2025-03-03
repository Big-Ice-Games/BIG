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
	/// Length 32.
	/// </summary>
	[Serializable]
	public struct Vector4Double : INetSerializable
	{
        [Preserve] public double X;
        [Preserve] public double Y;
        [Preserve] public double Z;
        [Preserve] public double W;

        [Preserve]
        public Vector4Double(double x, double y, double z, double w)
		{
			X = x;
			Y = y;
			Z = z;
			W = w;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Vector4Double Normalized()
		{
			double magnitude = Math.Sqrt(X * X + Y * Y + Z * Z + W * W);
			return new Vector4Double(X / magnitude, Y / magnitude, Z / magnitude, W / magnitude);
		}

		#region Operators
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector4Double operator -(Vector4Double vec1, Vector4Double vec2)
		{
			return new Vector4Double(vec1.X - vec2.X, vec1.Y - vec2.Y, vec1.Z - vec2.Z, vec1.W - vec2.W);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector4Double operator +(Vector4Double vec1, Vector4Double vec2)
		{
			return new Vector4Double(vec1.X + vec2.X, vec1.Y + vec2.Y, vec1.Z + vec2.Z, vec1.W + vec2.W);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector4Double operator *(Vector4Double vec1, Vector4Double vec2)
		{
			return new Vector4Double(vec1.X * vec2.X, vec1.Y * vec2.Y, vec1.Z * vec2.Z, vec1.W * vec2.W);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector4Double operator *(Vector4Double vec1, float value)
		{
			return new Vector4Double(vec1.X * value, vec1.Y * value, vec1.Z * value, vec1.W * value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector4Double operator /(Vector4Double vec1, float value)
		{
			if (value == 0)
			{
				throw new DivideByZeroException();
			}
			return new Vector4Double(vec1.X / value, vec1.Y / value, vec1.Z / value, vec1.W / value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator ==(Vector4Double vec1, Vector4Double vec2)
		{
			return vec1.X == vec2.X && vec1.Y == vec2.Y && vec1.Z == vec2.Z && vec1.W == vec2.W;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator !=(Vector4Double vec1, Vector4Double vec2)
		{
			return !(vec1 == vec2);
		}

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(Vector4Double other)
		{
			return X == other.X && Y == other.Y && Z == other.Z && W == other.W;
		}

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Equals(object obj)
		{
			return obj is Vector4Double other && Equals(other);
		}

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode()
		{
			unchecked
			{
				return (X.GetHashCode() * 397) ^ Y.GetHashCode();
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator Vector4(Vector4Double vector)
		{
			return new Vector4((float)vector.X, (float)vector.Y, (float)vector.Z, (float)vector.W);
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
            X = reader.GetDouble();
            Y = reader.GetDouble();
            Z = reader.GetDouble();
            W = reader.GetDouble();
        }
        #endregion
    }
}
