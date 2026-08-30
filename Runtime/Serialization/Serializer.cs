using System;

namespace BIG
{
    /// <summary>
    /// Entry point for deterministic binary serialization used on both server and client side.
    /// Works through generic constraints, so serializing structs implementing <see cref="ISerializable"/> never boxes.
    /// </summary>
    public static class Serializer
    {
        /// <summary>
        /// Serialize value into the given buffer.
        /// </summary>
        /// <returns>Number of bytes written.</returns>
        public static int Serialize<T>(T value, Span<byte> buffer) where T : ISerializable
        {
            var writer = new ByteWriter(buffer);
            value.Serialize(ref writer);
            return writer.Position;
        }

        /// <summary>
        /// Serialize value into a new, exactly-sized array. Allocates — prefer the Span overload on hot paths.
        /// </summary>
        public static byte[] SerializeToArray<T>(T value) where T : ISerializable
        {
            var result = new byte[value.SerializedSize];
            var writer = new ByteWriter(result);
            value.Serialize(ref writer);
            return result;
        }

        /// <summary>
        /// Serialize value into a buffer rented from <see cref="System.Buffers.ArrayPool{T}"/> — zero GC pressure.
        /// The caller MUST dispose the returned buffer (returns the array to the pool):
        /// <code>
        /// using var buffer = Serializer.SerializePooled(state);
        /// Send(buffer.Span);
        /// </code>
        /// Use this for buffers that outlive the current scope (queued sends) or have variable sizes;
        /// for small fixed-size messages a stackalloc + the Span overload is even cheaper.
        /// </summary>
        public static PooledBuffer SerializePooled<T>(T value) where T : ISerializable
        {
            var array = System.Buffers.ArrayPool<byte>.Shared.Rent(value.SerializedSize);
            var writer = new ByteWriter(array);
            value.Serialize(ref writer);
            return new PooledBuffer(array, writer.Position);
        }

        /// <summary>
        /// Deserialize a new instance of T from the given buffer.
        /// </summary>
        public static T Deserialize<T>(ReadOnlySpan<byte> buffer) where T : ISerializable, new()
        {
            var reader = new ByteReader(buffer);
            var result = new T();
            result.Deserialize(ref reader);
            return result;
        }

        /// <summary>
        /// Deserialize from the given buffer into an existing instance — no allocation at all for structs.
        /// </summary>
        public static void DeserializeInto<T>(ref T target, ReadOnlySpan<byte> buffer) where T : ISerializable
        {
            var reader = new ByteReader(buffer);
            target.Deserialize(ref reader);
        }
    }
}
