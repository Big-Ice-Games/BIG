#region license

// Copyright (c) 2025, Big Ice Games
// All rights reserved.

#endregion

using LiteNetLib.Utils;

namespace BIG.Network
{
    public interface IRequest : INetSerializable
    {
        NetworkRequest ToRequest();
    }

    /// <summary>
    /// Generic request. 
    /// </summary>
    public struct NetworkRequest : INetSerializable
    {
        public byte Id;
        public int Frame;

        /// <summary>
        /// While request is received we assign this value with sender id.
        /// While we are sending request, we can use this field to push request to certain player.
        /// </summary>
        public ulong Player;
        public byte[] Data;

        public NetworkRequest(byte id, int frame, byte[] data, ulong toWhom = 0)
        {
            Id = id;
            Frame = frame;
            Player = toWhom;
            Data = data;
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(Id);
            writer.Put(Frame);
            writer.Put(Player);
            writer.PutBytesWithLength(Data);
        }

        public void Deserialize(NetDataReader reader)
        {
            Id = reader.GetByte();
            Frame = reader.GetInt();
            Player = reader.GetULong();
            Data = reader.GetBytesWithLength();
        }

        public override string ToString()
        {
            return $"{Id}:{Frame}:{Player}:{Data.Length}";
        }
    }

    public static class NetworkRequestArraySerializer
    {
        public static void Put(this NetDataWriter writer, NetworkRequest[] array)
        {
            writer.Put(array.Length);
            for (int i = 0; i < array.Length; i++)
                array[i].Serialize(writer);
        }

        public static NetworkRequest[] Get(this NetDataReader reader)
        {
            int length = reader.GetInt();
            NetworkRequest[] array = new NetworkRequest[length];

            for (int i = 0; i < length; i++)
            {
                NetworkRequest element = new NetworkRequest();
                element.Deserialize(reader);
                array[i] = element;
            }

            return array;
        }
    }
}
