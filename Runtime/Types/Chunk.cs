#region license

// Copyright (c) 2025, Big Ice Games
// All rights reserved.

#endregion

using System;
using System.Runtime.CompilerServices;
using LiteNetLib.Utils;

namespace BIG.Types
{
    /// <summary>
    /// Chunk is used to store entities in 2 dimensional space for optimization purposes like network synchronization.
    /// </summary>
    [Serializable]
    public struct Chunk : IEquatable<Chunk>, INetSerializable
    {
        #region INetSerializable
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

        public int X;
        public int Y;

        public Chunk(int x, int y)
        {
            X = x;
            Y = y;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(Chunk a, Chunk b)
        {
            return a.X == b.X && a.Y == b.Y;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(Chunk a, Chunk b)
        {
            return a.X != b.X || a.Y != b.Y;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(Chunk other)
        {
            return X == other.X && Y == other.Y;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is Chunk chunk && Equals(chunk);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode()
        {
            unchecked
            {
                return X * 397 ^ Y;
            }
        }

        public override string ToString() => $"{X},{Y}";
    }
}