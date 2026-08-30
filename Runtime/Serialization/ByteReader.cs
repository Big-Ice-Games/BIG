using System;
using System.Buffers.Binary;
using System.Runtime.CompilerServices;
using System.Text;

namespace BIG
{
    /// <summary>
    /// Zero-allocation binary reader over a caller-provided buffer.
    /// Mirror of <see cref="ByteWriter"/> — all multi-byte values are read explicitly as little-endian.
    /// </summary>
    public ref struct ByteReader
    {
        private readonly ReadOnlySpan<byte> _buffer;
        private int _position;

        public ByteReader(ReadOnlySpan<byte> buffer)
        {
            _buffer = buffer;
            _position = 0;
        }

        /// <summary> Number of bytes read so far. </summary>
        public readonly int Position => _position;

        /// <summary> Number of bytes left in the underlying buffer. </summary>
        public readonly int Remaining => _buffer.Length - _position;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ReadBool() => ReadByte() != 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte ReadByte()
        {
            byte value = _buffer[_position];
            _position += sizeof(byte);
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public sbyte ReadSByte() => unchecked((sbyte)ReadByte());

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public short ReadShort()
        {
            short value = BinaryPrimitives.ReadInt16LittleEndian(_buffer.Slice(_position));
            _position += sizeof(short);
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ushort ReadUShort()
        {
            ushort value = BinaryPrimitives.ReadUInt16LittleEndian(_buffer.Slice(_position));
            _position += sizeof(ushort);
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int ReadInt()
        {
            int value = BinaryPrimitives.ReadInt32LittleEndian(_buffer.Slice(_position));
            _position += sizeof(int);
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public uint ReadUInt()
        {
            uint value = BinaryPrimitives.ReadUInt32LittleEndian(_buffer.Slice(_position));
            _position += sizeof(uint);
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long ReadLong()
        {
            long value = BinaryPrimitives.ReadInt64LittleEndian(_buffer.Slice(_position));
            _position += sizeof(long);
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ulong ReadULong()
        {
            ulong value = BinaryPrimitives.ReadUInt64LittleEndian(_buffer.Slice(_position));
            _position += sizeof(ulong);
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float ReadFloat() => BitConverter.Int32BitsToSingle(ReadInt());

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double ReadDouble() => BitConverter.Int64BitsToDouble(ReadLong());

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ReadOnlySpan<byte> ReadBytes(int count)
        {
            var value = _buffer.Slice(_position, count);
            _position += count;
            return value;
        }

        /// <summary>
        /// Reads a UTF-8 string prefixed with a ushort byte count (written by <see cref="ByteWriter.WriteString"/>).
        /// </summary>
        public string ReadString()
        {
            int byteCount = ReadUShort();
            if (byteCount == 0)
                return string.Empty;

            var bytes = _buffer.Slice(_position, byteCount);
            _position += byteCount;
            return Encoding.UTF8.GetString(bytes);
        }
    }
}
