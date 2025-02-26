#region license

// Copyright (c) 2025, Big Ice Games
// All rights reserved.

#endregion

namespace BIG.Network
{
    public enum DisconnectReason
    {
        ConnectionFailed,
        Timeout,
        HostUnreachable,
        NetworkUnreachable,
        RemoteConnectionClose,
        DisconnectPeerCalled,
        ConnectionRejected,
        InvalidProtocol,
        UnknownHost,
        Reconnect,
        PeerToPeerConnection,
        PeerNotFound
    }
}