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
	/// Length 6.
	/// </summary>
	[Serializable]
	public struct Vector3UShort : INetSerializable
	{
        [Preserve] public ushort X;
        [Preserve] public ushort Y;
        [Preserve] public ushort Z;

        [Preserve]
        public Vector3UShort(ushort x, ushort y, ushort z)
		{
			X = x;
			Y = y;
			Z = z;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator Vector3(Vector3UShort vector)
		{
			return new Vector3(vector.X, vector.Y, vector.Z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator Vector3UShort(Vector3 vector)
		{
			return new Vector3UShort((ushort)vector.X, (ushort)vector.Y, (ushort)vector.Z);
		}

		#region Operators
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator ==(Vector3UShort vec1, Vector3UShort vec2)
		{
			return vec1.X == vec2.X && vec1.Y == vec2.Y && vec1.Z == vec2.Z;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator !=(Vector3UShort vec1, Vector3UShort vec2)
		{
			return !(vec1 == vec2);
		}

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(Vector3UShort other)
		{
			return X == other.X && Y == other.Y && Z == other.Z;
		}

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Equals(object obj)
		{
			return obj is Vector3UShort other && Equals(other);
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

        #region Serializable
        public void Serialize(NetDataWriter writer)
        {
            writer.Put(X);
            writer.Put(Y);
            writer.Put(Z);
        }

        public void Deserialize(NetDataReader reader)
        {
            X = reader.GetUShort();
            Y = reader.GetUShort();
            Z = reader.GetUShort();
        }
        #endregion
    }
}
