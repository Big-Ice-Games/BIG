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
        public byte[] Data;

        public NetworkRequest(byte id, byte[] data)
        {
            Id = id;
            Data = data;
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(Id);
            writer.PutBytesWithLength(Data);
        }

        public void Deserialize(NetDataReader reader)
        {
            Id = reader.GetByte();
            Data = reader.GetBytesWithLength();
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
