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
	public struct Vector3Int : INetSerializable
	{
        [Preserve] public int X;
        [Preserve] public int Y;
        [Preserve] public int Z;

        [Preserve]
        public Vector3Int(int x, int y, int z)
		{
			X = x;
			Y = y;
            Z = z;
        }

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Vector3Int Normalized()
		{
			int magnitude = (int)Math.Sqrt(X * X + Y * Y + Z * Z);
			return new Vector3Int(X / magnitude, Y / magnitude, Z / magnitude);
		}

		public static bool TryParse(string value, out Vector3Int result)
		{
			result = new Vector3Int();
			var values = value.Split('.');
			if (values.Length != 3)
				return false;

			if (!int.TryParse(values[0], out result.X))
				return false;

			if (!int.TryParse(values[1], out result.Y))
				return false;

            if (!int.TryParse(values[2], out result.Z))
                return false;

			result.X = int.Parse(values[0]);
			result.Y = int.Parse(values[1]);
            result.Z = int.Parse(values[2]);
			return true;
		}

		#region Operators
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator Vector3(Vector3Int vec) => new Vector3(vec.X, vec.Y, vec.Z);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator Vector3Int(Vector3 vec) => new Vector3Int((int)vec.X, (int)vec.Y, (int)vec.Z);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector3Int operator -(Vector3Int vec1, Vector3Int vec2)
		{
			return new Vector3Int(vec1.X - vec2.X, vec1.Y - vec2.Y, vec1.Z - vec2.Z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector3Int operator +(Vector3Int vec1, Vector3Int vec2)
		{
			return new Vector3Int(vec1.X + vec2.X, vec1.Y + vec2.Y, vec1.Z + vec2.Z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector3Int operator *(Vector3Int vec1, Vector3Int vec2)
		{
			return new Vector3Int(vec1.X * vec2.X, vec1.Y * vec2.Y, vec1.Z * vec2.Z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector3Int operator *(Vector3Int vec1, float value)
		{
			return new Vector3Int((int)(vec1.X * value), (int)(vec1.Y * value), (int)(vec1.Z * value));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector3Int operator /(Vector3Int vec1, float value)
		{
			if (value == 0)
			{
				throw new DivideByZeroException();
			}

			return new Vector3Int((int)(vec1.X / value), (int)(vec1.Y / value), (int)(vec1.Z / value));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator ==(Vector3Int vec1, Vector3Int vec2)
		{
			return vec1.X == vec2.X && vec1.Y == vec2.Y && vec1.Z == vec2.Z;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator !=(Vector3Int vec1, Vector3Int vec2)
		{
			return !(vec1 == vec2);
		}

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(Vector3Int other)
		{
			return X.Equals(other.X) && Y.Equals(other.Y) && Z.Equals(other.Z);
		}

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Equals(object obj)
		{
			return obj is Vector3Int other && Equals(other);
		}

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode()
		{
			unchecked
			{
				return (X.GetHashCode() * 397) ^ Y.GetHashCode() ^ Z.GetHashCode();
			}
		}
        #endregion

        #region Serializable
        public void Serialize(NetDataWriter writer)
        {
            writer.Put(X);
            writer.Put(Y);
            writer.Put(Z);
        }

        public void Deserialize(NetDataReader reader)
        {
            X = reader.GetInt();
            Y = reader.GetInt();
            Z = reader.GetInt();
        }
        #endregion
    }
}
