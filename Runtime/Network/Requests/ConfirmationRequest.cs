#region license

// Copyright (c) 2025, Big Ice Games
// All rights reserved.

#endregion

using LiteNetLib.Utils;

namespace BIG.Network.Requests
{
    /// <summary>
    /// When client is connecting to the server - server is asking to confirm using <see cref="ConfirmationRequireRequest"/>.
    /// In response client is sending this request providing generic confirmation data in a form of byte[].
    /// </summary>
    public struct ConfirmationRequest : IRequest
    {
        public const byte REQUEST_ID = 2;
        public byte[] ConfirmationData { get; set; }
        public static NetworkRequest Create(byte[] confirmationData, int frame) => new ConfirmationRequest() { ConfirmationData = confirmationData }.ToRequest(frame);
        public NetworkRequest ToRequest(int frame = 0) => new NetworkRequest(REQUEST_ID, frame, this.Serialize());

        public void Serialize(NetDataWriter writer)
        {
            writer.PutBytesWithLength(ConfirmationData);
        }

        public void Deserialize(NetDataReader reader)
        {
            ConfirmationData = reader.GetBytesWithLength();
        }
    }
}