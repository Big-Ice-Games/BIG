namespace BIG
{
    /// <summary>
    /// Deterministic binary serialization contract used on both server and client side.
    /// All data is written explicitly as little-endian, so serialized bytes are identical on every platform.
    /// Implement directly on structs; call through generic constraint (<see cref="Serializer"/>) to avoid boxing.
    /// </summary>
    public interface ISerializable
    {
        /// <summary>
        /// Exact number of bytes this instance writes during <see cref="Serialize"/>.
        /// </summary>
        int SerializedSize { get; }

        void Serialize(ref ByteWriter writer);
        void Deserialize(ref ByteReader reader);
    }
}
