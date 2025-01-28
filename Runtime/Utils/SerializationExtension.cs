#region license

// Copyright (c) 2025, Big Ice Games
// All rights reserved.

#endregion

using System;
using System.Runtime.CompilerServices;
using LiteNetLib.Utils;

namespace BIG
{
    public static class Serializer
    {
        [ThreadStatic] private static NetDataWriter? _writer;
        [ThreadStatic] private static NetDataReader? _reader;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static NetDataWriter GetWriter()
        {
            _writer ??= new NetDataWriter();
            _writer.Reset();
            return _writer;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static NetDataReader GetReader(this byte[] data)
        {
            _reader ??= new NetDataReader();
            _reader.SetSource(data);
            return _reader;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte[] Serialize(this INetSerializable serializable)
        {
            NetDataWriter writer = GetWriter();
            serializable.Serialize(writer);
            return writer.CopyData(); // Return the serialized data as a byte array
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Deserialize<T>(this byte[] data, out T value) where T : struct, INetSerializable
        {
            NetDataReader reader = GetReader(data);
            value = default(T);
            value.Deserialize(reader);
        }
    }
}