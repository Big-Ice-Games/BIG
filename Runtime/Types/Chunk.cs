#region license

// Copyright (c) 2025, Big Ice Games
// All rights reserved.

#endregion

using System;
using System.Runtime.CompilerServices;
using LiteNetLib.Utils;

namespace BIG
{
    [Serializable]
    public struct Chunk : IEquatable<Chunk>, INetSerializable
    {
        [Preserve] public int X;
        [Preserve] public int Y;

        [Preserve]
        public Chunk(int x, int y)
        {
            X = x;
            Y = y;
        }

        [Preserve]
        public Chunk(Vector2Int value)
        {
            X = value.X;
            Y = value.Y;
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator Vector2Int(Chunk chunk) => new Vector2Int(chunk.X, chunk.Y);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator Chunk(Vector2UShort v) => new Chunk(v.X, v.Y);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override string ToString() => $"{X},{Y}";

        #region Serialization
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
    }
}
