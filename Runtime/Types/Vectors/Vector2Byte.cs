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
	/// Length 2.
	/// </summary>
	[Serializable]
	public struct Vector2Byte : INetSerializable
	{
        [Preserve] public byte X;
        [Preserve] public byte Y;

        [Preserve]
        public Vector2Byte(byte x, byte y)
		{
			X = x;
			Y = y;
		}

        [Preserve]
        public Vector2Byte(int x, int y)
        {
            X = (byte)x;
            Y = (byte)y;
        }

        [Preserve]
        public Vector2Byte(bool x, bool y)
		{
			X = x.ToByte();
			Y = y.ToByte();
		}


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Vector2Int Normalized()
		{
			int magnitude = (int)Math.Sqrt(X * X + Y * Y);
			return new Vector2Int(X / magnitude, Y / magnitude);
		}

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly Chunk ToChunk()
        {
            return new Chunk(X, Y);
        }

        public static bool TryParse(string value, out Vector2Byte result)
		{
			result = new Vector2Byte();
			var values = value.Split('.');
			if (values.Length != 2)
				return false;

			if (!byte.TryParse(values[0], out result.X))
				return false;

			if (!byte.TryParse(values[1], out result.Y))
				return false;
			result.X = byte.Parse(values[0]);
			result.Y = byte.Parse(values[1]);
			return true;
		}

        #region Operators
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(Vector2Byte vec1, Chunk vec2)
        {
            return vec1.X == vec2.X && vec1.Y == vec2.Y;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(Vector2Byte vec1, Chunk vec2)
        {
            return !(vec1 == vec2);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator ==(Vector2Byte vec1, Vector2Byte vec2)
		{
			return vec1.X == vec2.X && vec1.Y == vec2.Y;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator !=(Vector2Byte vec1, Vector2Byte vec2)
		{
			return !(vec1 == vec2);
		}

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(Vector2Byte other)
		{
			return X.Equals(other.X) && Y.Equals(other.Y);
		}

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Equals(object obj)
		{
			return obj is Vector2Byte other && Equals(other);
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
            X = reader.GetByte();
            Y = reader.GetByte();
        }
        #endregion
    }
}
