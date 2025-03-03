#region license

// Copyright (c) 2025, Big Ice Games
// All rights reserved.

#endregion

using LiteNetLib.Utils;

// ReSharper disable InvalidXmlDocComment

namespace BIG.Network.Requests
{
    public enum ConfirmationType : byte
    {
        /// <summary>
        /// None mean that for this confirmation type client can send just his nickname through <see cref="ConfirmationRequest"/>.
        /// </summary>
        None,

        /// <summary>
        /// Custom mean that for this confirmation type client should send custom authorization token from custom web service.
        /// </summary>
        Custom,

        /// <summary>
        /// SteamTicket mean that for this confirmation type client should send steam authorization ticket.
        /// <example>
        /// <code>
        /// AuthResponseCallback = Callback<GetAuthSessionTicketResponse_t/>.Create(OnAuthResponseCallback);
        /// SteamUser.GetAuthSessionTicket(_ticket, 1024, out _ticketCounter);
        /// </code>
        /// </example>
        /// </summary>
        SteamTicket
    }

    /// <summary>
    /// <see cref="IServer"/> sends it from <see cref="IServer.OnPeerConnected"/> function.
    /// So after first confirmation when client connected to the server in response server is sending this request with confirmation requirement.
    /// </summary>
    public struct ConfirmationRequireRequest : IRequest
    {
        public const byte REQUEST_ID = 1;
        public ConfirmationType ConfirmationType { get; set; }
        public NetworkRequest ToRequest(int frame) => new NetworkRequest(REQUEST_ID, frame, this.Serialize());
        public static NetworkRequest Create(ConfirmationType type, int frame = 0) => new ConfirmationRequireRequest() { ConfirmationType = type }.ToRequest(frame);
        public void Serialize(NetDataWriter writer)
        {
            writer.Put((byte)ConfirmationType);
        }

        public void Deserialize(NetDataReader reader)
        {
            ConfirmationType = (ConfirmationType)reader.GetByte();
        }
    }
}