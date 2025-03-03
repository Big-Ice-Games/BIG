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
	/// Length 8.
	/// </summary>
	[Serializable]
	public struct Vector2Int : INetSerializable
	{
        [Preserve] public int X;
        [Preserve] public int Y;

        [Preserve]
        public Vector2Int(int x, int y)
		{
			X = x;
			Y = y;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Vector2Int Normalized()
		{
			int magnitude = (int)Math.Sqrt(X * X + Y * Y);
			return new Vector2Int(X / magnitude, Y / magnitude);
		}

		public static bool TryParse(string value, out Vector2Int result)
		{
			result = new Vector2Int();
			var values = value.Split('.');
			if (values.Length != 2)
				return false;

			if (!int.TryParse(values[0], out result.X))
				return false;

			if (!int.TryParse(values[1], out result.Y))
				return false;
			result.X = int.Parse(values[0]);
			result.Y = int.Parse(values[1]);
			return true;
		}

		#region Operators
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator Vector2(Vector2Int vec) => new Vector2(vec.X, vec.Y);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator Vector2Int(Vector2 vec) => new Vector2Int((int)vec.X, (int)vec.Y);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2Int operator -(Vector2Int vec1, Vector2Int vec2)
		{
			return new Vector2Int(vec1.X - vec2.X, vec1.Y - vec2.Y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2Int operator +(Vector2Int vec1, Vector2Int vec2)
		{
			return new Vector2Int(vec1.X + vec2.X, vec1.Y + vec2.Y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2Int operator *(Vector2Int vec1, Vector2Int vec2)
		{
			return new Vector2Int(vec1.X * vec2.X, vec1.Y * vec2.Y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2Int operator *(Vector2Int vec1, float value)
		{
			return new Vector2Int((int)(vec1.X * value), (int)(vec1.Y * value));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2Int operator /(Vector2Int vec1, float value)
		{
			if (value == 0)
			{
				throw new DivideByZeroException();
			}

			return new Vector2Int((int)(vec1.X / value), (int)(vec1.Y / value));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator ==(Vector2Int vec1, Vector2Int vec2)
		{
			return vec1.X == vec2.X && vec1.Y == vec2.Y;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator !=(Vector2Int vec1, Vector2Int vec2)
		{
			return !(vec1 == vec2);
		}

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(Vector2Int other)
		{
			return X.Equals(other.X) && Y.Equals(other.Y);
		}

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Equals(object obj)
		{
			return obj is Vector2Int other && Equals(other);
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

        #region Serialization
        public void Serialize(NetDataWriter writer)
        {
            writer.Put(X);
            writer.Put(Y);
        }

        public void Deserialize(NetDataReader reader)
        {
            X = reader.GetInt();
            Y = reader.GetInt();
        }
        #endregion
    }
}
