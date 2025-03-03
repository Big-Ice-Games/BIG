#region license

// Copyright (c) 2025, Big Ice Games
// All rights reserved.

#endregion

using LiteNetLib.Utils;

namespace BIG.Network.Requests
{
    /// <summary>
    /// After server connected a new client it sends <see cref="ConfirmationRequireRequest"/>.
    /// In response client is sending <see cref="ConfirmationRequest"/>.
    /// And after server check the client through <see cref="IServerAuthorizationHandler"/> it will send him this request with information about his id and
    /// value indicating whether he was accepted or not.
    /// </summary>
    public struct ConfirmationResponseRequest : IRequest
    {
        public const byte REQUEST_ID = 3;
        public int Id;
        public bool Accepted;
        public byte[] Data;

        public static NetworkRequest Create(int id, bool accepted, byte[] data, int frame = 0) => new ConfirmationResponseRequest() { Id = id, Accepted = accepted, Data = data }.ToRequest(frame);
        public NetworkRequest ToRequest(int frame) => new NetworkRequest(REQUEST_ID, frame, this.Serialize());
        public void Serialize(NetDataWriter writer)
        {
            writer.Put(Id);
            writer.Put(Accepted);
            writer.PutBytesWithLength(Data);
        }

        public void Deserialize(NetDataReader reader)
        {
            Id = reader.GetInt();
            Accepted = reader.GetBool();
            Data = reader.GetBytesWithLength();
        }
    }
}
