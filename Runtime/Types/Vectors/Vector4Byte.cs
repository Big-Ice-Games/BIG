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
	public struct Vector4Byte : INetSerializable
	{
        [Preserve] public byte X;
        [Preserve] public byte Y;
        [Preserve] public byte Z;
        [Preserve] public byte W;

        [Preserve]
        public Vector4Byte(byte x, byte y, byte z, byte w)
		{
			X = x;
			Y = y;
			Z = z;
			W = w;
		}

        [Preserve]
        public Vector4Byte(string fromString)
		{
			var xyzw = fromString.Split(new char[] {':'});
			X = byte.Parse(xyzw[0]);
			Y = byte.Parse(xyzw[1]);
			Z = byte.Parse(xyzw[2]);
			W = byte.Parse(xyzw[3]);
		}

		#region Operators
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector4Byte operator -(Vector4Byte vec1, Vector4Byte vec2)
		{
			return new Vector4Byte((byte)(vec1.X - vec2.X), (byte)(vec1.Y - vec2.Y), (byte)(vec1.Z - vec2.Z), (byte)(vec1.W - vec2.W));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector4Byte operator +(Vector4Byte vec1, Vector4Byte vec2)
		{
			return new Vector4Byte((byte)(vec1.X + vec2.X), (byte)(vec1.Y + vec2.Y), (byte)(vec1.Z + vec2.Z), (byte)(vec1.W + vec2.W));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector4Byte operator *(Vector4Byte vec1, Vector4Byte vec2)
		{
			return new Vector4Byte((byte)(vec1.X * vec2.X), (byte)(vec1.Y * vec2.Y), (byte)(vec1.Z * vec2.Z), (byte)(vec1.W * vec2.W));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector4Byte operator *(Vector4Byte vec1, float value)
		{
			return new Vector4Byte((byte)(vec1.X * value), (byte)(vec1.Y * value), (byte)(vec1.Z * value), (byte)(vec1.W * value));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator Vector3(Vector4Byte vector)
		{
			return new Vector3(vector.X, vector.Y, vector.Z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator Vector4(Vector4Byte vector)
		{
			return new Vector4(vector.X, vector.Y, vector.Z, vector.W);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector4Byte operator /(Vector4Byte vec1, float value)
		{
			if (value == 0)
			{
				throw new DivideByZeroException();
			}
			return new Vector4Byte((byte)(vec1.X / value), (byte)(vec1.Y / value), (byte)(vec1.Z / value), (byte)(vec1.W / value));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator ==(Vector4Byte vec1, Vector4Byte vec2)
		{
			return vec1.X == vec2.X && vec1.Y == vec2.Y && vec1.Z == vec2.Z && vec1.W == vec2.W;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator !=(Vector4Byte vec1, Vector4Byte vec2)
		{
			return !(vec1 == vec2);
		}

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(Vector4Byte other)
		{
			return X == other.X && Y == other.Y && Z == other.Z && W == other.W;
		}

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Equals(object obj)
		{
			return obj is Vector4Byte other && Equals(other);
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
            X = reader.GetByte();
            Y = reader.GetByte();
            Z = reader.GetByte();
            W = reader.GetByte();
        }
        #endregion
    }
}
