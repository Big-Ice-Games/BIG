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
	/// Can be used to values up to 99.99.
	/// </summary>
	[Serializable]
	public struct SmallFloat : INetSerializable
	{
        [Preserve] public short Value;

		[Obsolete("This constructor is only for serialization purpose. Don't use it explicitly!")]
		public SmallFloat(short value)
		{
			Value = value;
		}

        [Preserve]
        public SmallFloat(float value)
		{
			Value = (short)(value * 100);
		}

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator float(SmallFloat value)
		{
			return value.Value / 100f;
		}

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator SmallFloat(float value)
        {
            return new SmallFloat(value);
        }

        #region Serialization
        public void Serialize(NetDataWriter writer)
        {
            writer.Put(Value);
        }

        public void Deserialize(NetDataReader reader)
        {
            Value = reader.GetShort();
        }
        #endregion
    }
}
