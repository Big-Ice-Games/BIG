#region license

// Copyright (c) 2025, Big Ice Games
// All rights reserved.

#endregion

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using BIG.Network.Requests;
using LiteNetLib;
using LiteNetLib.Utils;

namespace BIG.Network
{
    /// <summary>
    /// To make the game authorization platform-agnostic we are injecting this generic handler for authorization.
    /// When server client send <see cref="ConfirmationRequest"/> and we have to validate it - it goes through this handler which can asynchronously confirm player identity
    /// through http or any other way.
    /// </summary>
    public interface IServerAuthorizationHandler
    {
        public struct AuthorizationResponse : INetSerializable
        {
            public bool Authorized;
            public byte[] Data;
            public void Serialize(NetDataWriter writer)
            {
                writer.Put(Authorized);
                writer.PutBytesWithLength(Data);
            }

            public void Deserialize(NetDataReader reader)
            {
                Authorized = reader.GetBool();
                Data = reader.GetBytesWithLength();
            }
        }

        public Task<AuthorizationResponse> Authorize(byte[] data);
    }

    /// <summary>
    /// Wrapper for server client that provides abstract functions used to communicate.
    /// </summary>
    public interface IPeer
    {
        int Id { get; }
        int Latency { get; }
        void SendReliable(byte[] data);
        void SendUnreliable(byte[] data);
    }

    public interface IServer
    {
        Action<IPeer> PeerConnected { get; set; }
        Action<IPeer, byte[]> OnPeerConfirmed { get; set; }
        Action<IPeer, NetworkRequest> NetworkRequestReceived { get; set; }
        Action<IPeer, DisconnectReason> OnPeerDisconnected { get; set; }

        /// <summary>
        /// Start underlying server. It is a part of every game scope.
        /// Every time player is starting a client it also starts server which client is connecting to.
        /// </summary>
        /// <returns>Value indicating whether we successfully started the server.</returns>
        bool Start();

        /// <summary>
        /// Stop the server. It will stop listening and disconnect all the clients.
        /// </summary>
        void Stop();

        /// <summary>
        /// Send request to all confirmed clients
        /// </summary>
        void SendToAllReliable(in NetworkRequest request);

        /// <summary>
        /// Send request to all confirmed clients
        /// </summary>
        void SendToAllUnReliable(in NetworkRequest request);
    }

    [Register(true)]
    internal sealed class Server : INetEventListener, IServer
    {
        private sealed class ServerClient : IPeer
        {
            public int Id => _peer.Id;
            public int Latency => _peer.Ping;
            public IPAddress Address => _peer.Address;

            private readonly NetPeer _peer;

            public byte[] AuthorizationData { get; set; }

            public ServerClient(NetPeer peer)
            {
                _peer = peer;
            }

            public void SendReliable(byte[] data)
            {
                _peer.Send(data, DeliveryMethod.ReliableOrdered);
            }

            public void SendUnreliable(byte[] data)
            {
                _peer.Send(data, DeliveryMethod.Unreliable);
            }
        }

        #region Fields and Properties
        /// <summary>
        /// Server is running on the separated thread since we call <see cref="Start"/> as long as IsWorking is true.
        /// </summary>
        public bool IsWorking { get; private set; }
        private readonly Thread _workingThread;
        private readonly IServerSettings _settings;

        /// <summary>
        /// https://github.com/RevenantX/LiteNetLib/blob/master/LiteNetLib/NetManager.cs
        /// </summary>
        private NetManager _server;

        /// <summary>
        /// When new client is connecting to the server he is assigned to confirmation queue.
        /// Every second all the clients from the confirmation queue are getting update with their position in the queue.
        /// Queue is necessary to load balance incoming traffic. Because it takes time to asynchronously confirm incoming client through <see cref="IServerAuthorizationHandler"/>
        /// we need to keep communication with all the clients that are waiting for confirmation otherwise players which are far in the queue will receive timeouts because lack of the
        /// communication.
        /// </summary>
        private readonly Dictionary<int, ServerClient> _confirmationQueue;

        /// <summary>
        /// All the clients connected to the server and confirmed.
        /// </summary>
        private readonly Dictionary<int, ServerClient> _clients;
        private readonly IServerAuthorizationHandler _authorizationHandler;

        #endregion

        #region Public API

        public Action<IPeer> PeerConnected { get; set; }
        public Action<IPeer, byte[]> OnPeerConfirmed { get; set; }
        public Action<IPeer, DisconnectReason> OnPeerDisconnected { get; set; }
        public Action<IPeer, NetworkRequest> NetworkRequestReceived { get; set; }
        public bool Start()
        {
            this.Log($"Server: Trying to start server on {_settings.DefaultServerPort} port.");
            try
            {
                _server = new NetManager(this) { AutoRecycle = true };
                _server.Start(_settings.DefaultServerPort);
                IsWorking = true;
                _workingThread.Start();
                this.Log("Server: Started.");
                return true;
            }
            catch (Exception ex)
            {
                this.Log($"Server.Exception occur in BIG.Server.Start()\n{ex}", LogLevel.Error);
                return false;
            }
        }
        public void Stop()
        {
            IsWorking = false;
            _server.Stop(true);
            _workingThread.Abort();
        }
        public void SendToAllReliable(in NetworkRequest request)
        {
            var serialized = request.Serialize();
            foreach (KeyValuePair<int, ServerClient> serverClient in _clients)
            {
                serverClient.Value.SendReliable(serialized);
            }
        }

        /// <summary>
        /// Send reliable request to all the players exceptClientId one.
        /// </summary>
        /// <param name="request">Request.</param>
        /// <param name="exceptClientId">Client id who SHOULD NOT receive this request.</param>
        public void SendToAllReliable(in NetworkRequest request, int exceptClientId)
        {
            var serialized = request.Serialize();
            foreach (KeyValuePair<int, ServerClient> serverClient in _clients)
            {
                if (serverClient.Value.Id == exceptClientId) continue;
                serverClient.Value.SendReliable(serialized);
            }
        }

        public void SendToAllUnReliable(in NetworkRequest request)
        {
            var serialized = request.Serialize();
            foreach (KeyValuePair<int, ServerClient> serverClient in _clients)
            {
                serverClient.Value.SendUnreliable(serialized);
            }
        }
        #endregion

        public Server(IServerSettings settings, IServerAuthorizationHandler authorizationHandler)
        {
            _settings = settings;
            _clients = new Dictionary<int, ServerClient>(_settings.ServerMaxConnections);
            _confirmationQueue = new Dictionary<int, ServerClient>(_settings.ServerMaxConnections);

            _workingThread = new Thread(Work)
            {
                IsBackground = true
            };

            _authorizationHandler = authorizationHandler;
        }

        #region INetEventListener
        void INetEventListener.OnPeerConnected(NetPeer peer)
        {
            if (_confirmationQueue.ContainsKey(peer.Id))
            {
                this.Log($"Server.OnPeerConnected({peer.Id}). Peer already exists in confirmation queue.", LogLevel.Warning);
                return;
            }
            if (_clients.ContainsKey(peer.Id))
            {
                this.Log($"Server.OnPeerConnected({peer.Id}). Peer already exists as confirmed client.", LogLevel.Warning);
                return;
            }

            this.Log($"Server.OnPeerConnected({peer.Address} with id {peer.Id}");
            var client = new ServerClient(peer);
            _confirmationQueue.Add(peer.Id, client);
            NetworkRequest request = ConfirmationRequireRequest.Create(_settings.ServerRequiredConfirmationType);
            peer.Send(request.Serialize(), DeliveryMethod.ReliableOrdered);

            PeerConnected?.Invoke(client);
        }

        void INetEventListener.OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
        {
            if (_clients.ContainsKey(peer.Id))
            {
                this.Log($"Server.Peer {peer.Id} disconnected. Reason: {disconnectInfo.Reason}", LogLevel.Info);
                OnPeerDisconnected?.Invoke(_clients[peer.Id], Enum.Parse<DisconnectReason>(disconnectInfo.Reason.ToString()));
                _clients.Remove(peer.Id);

                var req = RemotePlayerDisconnectedRequest.Create(peer.Id);
                SendToAllReliable(in req);
            }
            else
            {
                this.Log($"Server.OnPeerDisconnected: {peer.Id} {disconnectInfo.Reason} called for not existing client.", LogLevel.Warning);
            }
        }

        void INetEventListener.OnNetworkError(IPEndPoint endPoint, SocketError socketError)
        {
            this.Log($"Server.Network error {socketError}", LogLevel.Warning);
        }

        void INetEventListener.OnNetworkReceive(NetPeer peer, NetPacketReader reader, byte channelNumber, DeliveryMethod deliveryMethod)
        {
            NetworkRequest networkRequest = new NetworkRequest();
            networkRequest.Deserialize(reader);

            if (networkRequest.Data == null)
            {
                this.Log("Server.Deserialized network request with null data.", LogLevel.Warning);
                return;
            }

            switch (networkRequest.Id)
            {
                case ConfirmationRequest.REQUEST_ID:
                    {
                        networkRequest.Data.Deserialize(out ConfirmationRequest request);
                        OnConfirmationRequest(peer, request);
                        break;
                    }

                default:
                    NetworkRequestReceived?.Invoke(_clients[peer.Id], networkRequest);
                    break;
            }
        }

        void INetEventListener.OnNetworkReceiveUnconnected(IPEndPoint remoteEndPoint, NetPacketReader reader, UnconnectedMessageType messageType)
        {
            this.Log("Server.OnNetworkReceiveUnconnected");
        }
        void INetEventListener.OnNetworkLatencyUpdate(NetPeer peer, int latency) { }
        void INetEventListener.OnConnectionRequest(ConnectionRequest request)
        {
            this.Log($"Server.OnConnectionRequest({request.RemoteEndPoint}", LogLevel.Info);
            if (_server.ConnectedPeersCount < _settings.ServerMaxConnections)
                request.AcceptIfKey(_settings.ServerConnectionString);
            else
                request.Reject();
        }

        #endregion
        private async void Work()
        {
            while (IsWorking)
            {
                Update();
                await Task.Delay(_settings.MsDelayInTheNetworkThread);
            }
        }
        private void Update()
        {
            _server.PollEvents();
        }
        private async void OnConfirmationRequest(NetPeer peer, ConfirmationRequest request)
        {
            if (_confirmationQueue.TryGetValue(peer.Id, out ServerClient serverClient))
            {
                var authorize = await _authorizationHandler.Authorize(request.ConfirmationData);
                if (authorize.Authorized)
                {
                    serverClient.AuthorizationData = authorize.Data;
                    var req = ConfirmationResponseRequest.Create(peer.Id, true, authorize.Data);
                    serverClient.SendReliable(req.Serialize());
                    OnPeerConfirmed?.Invoke(serverClient, authorize.Data);
                    this.Log($"Server.Player {peer.Id}:{peer.Address} confirmed.", LogLevel.Info);

                    // Move client from the confirmation queue into regular clients list.
                    {
                        _clients.TryAdd(peer.Id, serverClient);
                        _confirmationQueue.Remove(peer.Id);
                    }

                    NetworkRequest nr = RemotePlayerConnectedRequest.Create(serverClient.Id, serverClient.AuthorizationData);
                    SendToAllReliable(nr, serverClient.Id);

                    if (_settings.SyncAllClientsToTheNewlyConnectedOne)
                    {
                        foreach (KeyValuePair<int, ServerClient> c in _clients)
                        {
                            // For every other client besides the one who just connected
                            if (c.Key == serverClient.Id) continue;
                            // Grab description of this player and his id
                            NetworkRequest nr_ = RemotePlayerConnectedRequest.Create(c.Value.Id, c.Value.AuthorizationData);
                            // Send it to the newly connected client
                            serverClient.SendReliable(nr_.Serialize());
                        }
                    }
                }
                else
                {
                    serverClient.SendReliable(ConfirmationResponseRequest.Create(-1, false, Array.Empty<byte>()).Serialize());
                    _confirmationQueue.Remove(peer.Id);
                    this.Log($"Server.Player {peer.Id}:{peer.Address} NOT confirmed and removed.", LogLevel.Info);
                }
            }
            else
            {
                this.Log($"Server.OnConfirmationRequest received from peer {peer.Id} not presented in clients list.", LogLevel.Warning);
            }
        }
    }
}
