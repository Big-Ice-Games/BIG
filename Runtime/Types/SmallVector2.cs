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
	/// Length = 4.
	/// </summary>
	[Serializable]
	public struct SmallVector2 : INetSerializable
	{
        [Preserve] public short X;
        [Preserve] public short Y;

		[Obsolete("This constructor is only for serialization purpose. Don't use it explicitly!")]
		public SmallVector2(short x, short y)
		{
			X = x;
			Y = y;
		}

        [Preserve]
        public SmallVector2(float x, float y)
		{
			X = (short)(x * 100);
			Y = (short)(y * 100);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator Vector2(SmallVector2 vector)
		{
			return new Vector2(vector.X / 100f, vector.Y / 100f);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator SmallVector2(Vector2 vector)
		{
			return new SmallVector2(vector.X, vector.Y);
		}

		#region Operators
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator ==(SmallVector2 vec1, SmallVector2 vec2)
		{
			return vec1.X == vec2.X && vec1.Y == vec2.Y;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator !=(SmallVector2 vec1, SmallVector2 vec2)
		{
			return !(vec1 == vec2);
		}

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(SmallVector2 other)
		{
			return X == other.X && Y == other.Y;
		}

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Equals(object obj)
		{
			return obj is SmallVector2 other && Equals(other);
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
            X = reader.GetShort();
            Y = reader.GetShort();
        }
        #endregion
    }
}
