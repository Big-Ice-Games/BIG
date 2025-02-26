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
	/// Length 6.
	/// </summary>
	[Serializable]
	public struct SmallVector3 : INetSerializable
	{
        [Preserve] public short X;
        [Preserve] public short Y;
        [Preserve] public short Z;

		[Obsolete("This constructor is only for serialization purpose. Don't use it explicitly!")]
		public SmallVector3(short x, short y, short z)
		{
			X = x;
			Y = y;
			Z = z;
		}

        [Preserve]
        public SmallVector3(float x, float y, float z)
		{
			X = (short)(x * 100);
			Y = (short)(y * 100);
			Z = (short)(z * 100);
		}

        [Preserve]
        public SmallVector3(Vector3 vector)
		{
			X = (short)(vector.X * 100);
			Y = (short)(vector.Y * 100);
			Z = (short)(vector.Z * 100);
		}

		#region Operators
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator Vector3(SmallVector3 vector)
		{
			return new Vector3(vector.X / 100f, vector.Y / 100f, vector.Z / 100f);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator SmallVector3(Vector3 vector)
		{
			return new SmallVector3(vector.X, vector.Y, vector.Z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator ==(SmallVector3 vec1, SmallVector3 vec2)
		{
			return vec1.X == vec2.X && vec1.Y == vec2.Y && vec1.Z == vec2.Z;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator !=(SmallVector3 vec1, SmallVector3 vec2)
		{
			return !(vec1 == vec2);
		}

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(SmallVector3 other)
		{
			return X == other.X && Y == other.Y && Z == other.Z;
		}

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Equals(object obj)
		{
			return obj is SmallVector3 other && Equals(other);
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
			return $"{X}:{Y}:{Z}";
		}

        #region Serialization
        public void Serialize(NetDataWriter writer)
        {
            writer.Put(X);
            writer.Put(Y);
			writer.Put(Z);
        }

        public void Deserialize(NetDataReader reader)
        {
            X = reader.GetShort();
            Y = reader.GetShort();
            Z = reader.GetShort();
        }
        #endregion
    }
}
