#region license

// Copyright (c) 2025, Big Ice Games
// All rights reserved.

#endregion

using LiteNetLib.Utils;
using System;
using System.Runtime.CompilerServices;

namespace BIG
{
	/// <summary>
	/// Length = 8;
	/// </summary>
	[Serializable]
	public struct Vector2 : INetSerializable
	{
		public static Vector2 Zero = new Vector2();

        [Preserve] public float X;
        [Preserve] public float Y;

        [Preserve]
        public Vector2(float x, float y)
		{
			X = x;
			Y = y;
		}

        [Preserve]
        public Vector2(double x, double y)
        {
            X = (float)x;
            Y = (float)y;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Vector2 Normalized()
		{
			float magnitude = MathF.Sqrt(X * X + Y * Y);
			return new Vector2(X / magnitude, Y / magnitude);
		}

		#region Operators
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2 operator -(Vector2 vec1, Vector2 vec2)
		{
			return new Vector2(vec1.X - vec2.X, vec1.Y - vec2.Y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2 operator +(Vector2 vec1, Vector2 vec2)
		{
			return new Vector2(vec1.X + vec2.X, vec1.Y + vec2.Y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2 operator *(Vector2 vec1, Vector2 vec2)
		{
			return new Vector2(vec1.X * vec2.X, vec1.Y * vec2.Y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2 operator *(Vector2 vec1, float value)
		{
			return new Vector2(vec1.X * value, vec1.Y * value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2 operator /(Vector2 vec1, float value)
		{
			if (value == 0)
			{
				throw new DivideByZeroException();
			}
			return new Vector2(vec1.X / value, vec1.Y / value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator ==(Vector2 vec1, Vector2 vec2)
		{
			return vec1.X == vec2.X && vec1.Y == vec2.Y;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator !=(Vector2 vec1, Vector2 vec2)
		{
			return !(vec1 == vec2);
		}

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(Vector2 other)
		{
			return X.Equals(other.X) && Y.Equals(other.Y);
		}

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Equals(object obj)
		{
			return obj is Vector2 other && Equals(other);
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
        public override string ToString() => $"{X:F}:{Y:F}";

        #region Serialization
        public void Serialize(NetDataWriter writer)
        {
            writer.Put(X);
            writer.Put(Y);
        }

        public void Deserialize(NetDataReader reader)
        {
            X = reader.GetFloat();
            Y = reader.GetFloat();
        }
        #endregion
    }
}
