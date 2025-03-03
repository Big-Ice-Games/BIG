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
	/// Length = 3.
	/// </summary>
	[Serializable]
	public struct Vector3Byte : INetSerializable
	{
        [Preserve] public byte X;
        [Preserve] public byte Y;
        [Preserve] public byte Z;

        [Preserve]
        public Vector3Byte(byte x, byte y, byte z)
		{
			X = x;
			Y = y;
            Z = z;
        }

        [Preserve]
        public Vector3Byte(bool x, bool y, bool z)
		{
			X = x.ToByte();
			Y = y.ToByte();
            Z = z.ToByte();
        }

        public static Vector3Byte Random(Random random)
        {
			return new Vector3Byte((byte)random.Next(byte.MinValue, byte.MaxValue), (byte)random.Next(byte.MinValue, byte.MaxValue), (byte)random.Next(byte.MinValue, byte.MaxValue));
        }

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Vector3Int Normalized()
		{
			int magnitude = (int)Math.Sqrt(X * X + Y * Y + Z * Z);
			return new Vector3Int(X / magnitude, Y / magnitude, Z / magnitude);
		}

		public static bool TryParse(string value, out Vector3Byte result)
		{
			result = new Vector3Byte();
			var values = value.Split('.');
			if (values.Length != 3)
				return false;

			if (!byte.TryParse(values[0], out result.X))
				return false;

			if (!byte.TryParse(values[1], out result.Y))
				return false;

            if (!byte.TryParse(values[2], out result.Z))
                return false;

			result.X = byte.Parse(values[0]);
			result.Y = byte.Parse(values[1]);
            result.Z = byte.Parse(values[2]);
			return true;
		}

		#region Operators
		

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator ==(Vector3Byte vec1, Vector3Byte vec2)
		{
			return vec1.X == vec2.X && vec1.Y == vec2.Y && vec1.Z == vec2.Z;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator !=(Vector3Byte vec1, Vector3Byte vec2)
		{
			return !(vec1 == vec2);
		}

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(Vector3Byte other)
		{
			return X.Equals(other.X) && Y.Equals(other.Y) && Z.Equals(other.Z);
		}

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Equals(object obj)
		{
			return obj is Vector3Byte other && Equals(other);
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override string ToString()
        {
            return $"{X} : {Y} : {Z}";
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
            X = reader.GetByte();
            Y = reader.GetByte();
            Z = reader.GetByte();
        }
        #endregion
    }
}
