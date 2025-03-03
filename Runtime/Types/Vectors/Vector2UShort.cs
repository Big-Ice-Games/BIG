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
	/// Length 4.
	/// </summary>
	[Serializable]
	public struct Vector2UShort : INetSerializable
	{
        [Preserve] public ushort X;
        [Preserve] public ushort Y;

        [Preserve]
        public Vector2UShort(ushort x, ushort y)
		{
			X = x;
			Y = y;
		}


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator Vector2(Vector2UShort vector)
		{
			return new Vector2(vector.X, vector.Y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator Vector2UShort(Vector2 vector)
		{
			return new Vector2UShort((ushort)vector.X, (ushort)vector.Y);
		}

		#region Operators
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator ==(Vector2UShort vec1, Vector2UShort vec2)
		{
			return vec1.X == vec2.X && vec1.Y == vec2.Y;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator !=(Vector2UShort vec1, Vector2UShort vec2)
		{
			return !(vec1 == vec2);
		}

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(Vector2UShort other)
		{
			return X == other.X && Y == other.Y;
		}

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Equals(object obj)
		{
			return obj is Vector2UShort other && Equals(other);
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
			return $"{X}:{Y}";
		}

        #region Serialization
        public void Serialize(NetDataWriter writer)
        {
            writer.Put(X);
            writer.Put(Y);
        }

        public void Deserialize(NetDataReader reader)
        {
            X = reader.GetUShort();
            Y = reader.GetUShort();
        }
        #endregion
    }
}
