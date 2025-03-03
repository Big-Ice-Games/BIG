#region license

// Copyright (c) 2025, Big Ice Games
// All rights reserved.

#endregion

using LiteNetLib.Utils;

namespace BIG.Network.Requests
{
    /// <summary>
    /// Client receive this request when other client connected to the server
    /// OR
    /// when client connected to the server and server sending details
    /// about all the other already connected clients if <see cref="IServerSettings.SyncAllClientsToTheNewlyConnectedOne"/>
    /// </summary>
    public struct RemotePlayerConnectedRequest : IRequest
    {
        public const byte REQUEST_ID = 4;
        public int Id { get; set; }
        public byte[] Data { get; set; }

        public static NetworkRequest Create(int id, byte[] data) => new RemotePlayerConnectedRequest() { Id = id, Data = data }.ToRequest();
        public NetworkRequest ToRequest(int frame = 0) => new NetworkRequest(REQUEST_ID, frame, this.Serialize());

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(Id);
            writer.PutBytesWithLength(Data);
        }

        public void Deserialize(NetDataReader reader)
        {
            Id = reader.GetInt();
            Data = reader.GetBytesWithLength();
        }
    }
}
