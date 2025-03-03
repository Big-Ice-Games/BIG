#region license

// Copyright (c) 2025, Big Ice Games
// All rights reserved.

#endregion

using LiteNetLib.Utils;

namespace BIG.Network.Requests
{
    /// <summary>
    /// Client receiving this request while another client disconnected from the same server.
    /// </summary>
    public struct RemotePlayerDisconnectedRequest : IRequest
    {
        public const byte REQUEST_ID = 5;
        public int Id { get; set; }
        public static NetworkRequest Create(int id) => new RemotePlayerDisconnectedRequest() { Id = id }.ToRequest();
        public NetworkRequest ToRequest(int frame = 0) => new NetworkRequest(REQUEST_ID, frame, this.Serialize());

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(Id);
        }

        public void Deserialize(NetDataReader reader)
        {
            Id = reader.GetInt();
        }
    }
}
