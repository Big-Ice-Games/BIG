using System;
using System.Buffers.Binary;
using System.Runtime.CompilerServices;
using System.Text;

namespace BIG
{
    /// <summary>
    /// Zero-allocation binary writer over a caller-provided buffer.
    /// All multi-byte values are written explicitly as little-endian — output is byte-identical on every platform.
    /// </summary>
    public ref struct ByteWriter
    {
        private readonly Span<byte> _buffer;
        private int _position;

        public ByteWriter(Span<byte> buffer)
        {
            _buffer = buffer;
            _position = 0;
        }

        /// <summary> Number of bytes written so far. </summary>
        public readonly int Position => _position;

        /// <summary> Number of bytes left in the underlying buffer. </summary>
        public readonly int Remaining => _buffer.Length - _position;

        /// <summary> Slice of the underlying buffer containing everything written so far. </summary>
        public readonly Span<byte> Written => _buffer.Slice(0, _position);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteBool(bool value) => WriteByte(value ? (byte)1 : (byte)0);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteByte(byte value)
        {
            _buffer[_position] = value;
            _position += sizeof(byte);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteSByte(sbyte value) => WriteByte(unchecked((byte)value));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteShort(short value)
        {
            BinaryPrimitives.WriteInt16LittleEndian(_buffer.Slice(_position), value);
            _position += sizeof(short);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteUShort(ushort value)
        {
            BinaryPrimitives.WriteUInt16LittleEndian(_buffer.Slice(_position), value);
            _position += sizeof(ushort);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteInt(int value)
        {
            BinaryPrimitives.WriteInt32LittleEndian(_buffer.Slice(_position), value);
            _position += sizeof(int);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteUInt(uint value)
        {
            BinaryPrimitives.WriteUInt32LittleEndian(_buffer.Slice(_position), value);
            _position += sizeof(uint);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteLong(long value)
        {
            BinaryPrimitives.WriteInt64LittleEndian(_buffer.Slice(_position), value);
            _position += sizeof(long);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteULong(ulong value)
        {
            BinaryPrimitives.WriteUInt64LittleEndian(_buffer.Slice(_position), value);
            _position += sizeof(ulong);
        }

        /// <summary>
        /// Writes the raw IEEE 754 bits — deterministic, no culture and no rounding involved.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteFloat(float value) => WriteInt(BitConverter.SingleToInt32Bits(value));

        /// <summary>
        /// Writes the raw IEEE 754 bits — deterministic, no culture and no rounding involved.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteDouble(double value) => WriteLong(BitConverter.DoubleToInt64Bits(value));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteBytes(ReadOnlySpan<byte> value)
        {
            value.CopyTo(_buffer.Slice(_position));
            _position += value.Length;
        }

        /// <summary>
        /// Writes UTF-8 bytes prefixed with a ushort byte count. Null and empty string are both written as empty.
        /// </summary>
        public void WriteString(string? value)
        {
            if (string.IsNullOrEmpty(value))
            {
                WriteUShort(0);
                return;
            }

            int byteCount = Encoding.UTF8.GetByteCount(value);
            if (byteCount > ushort.MaxValue)
                throw new ArgumentException($"Serialized string is too long: {byteCount} bytes (max {ushort.MaxValue}).");

            WriteUShort((ushort)byteCount);
            Encoding.UTF8.GetBytes(value.AsSpan(), _buffer.Slice(_position, byteCount));
            _position += byteCount;
        }
    }
}
