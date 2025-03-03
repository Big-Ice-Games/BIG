#region license

// Copyright (c) 2025, Big Ice Games
// All rights reserved.

#endregion

using BIG.Network.Requests;
using LiteNetLib;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace BIG.Network
{
    public interface IClientAuthorizationProvider
    {
        Task<ConfirmationRequest> Authorize();
    }

    public interface IClient
    {
        Action<NetworkRequest> OnNetworkRequestReceived { get; set; }
        Action<ConfirmationResponseRequest> OnConnected { get; set; }
        Action<RemotePlayerConnectedRequest> OnRemotePlayerConnected { get; set; }
        Action<RemotePlayerDisconnectedRequest> OnRemotePlayerDisconnected { get; set; }
        void Connect(string ip);
        void Disconnect();
        void SendReliable(NetworkRequest request);
        void SendUnreliable(NetworkRequest request);
        int Ping { get; }
        bool Connected { get; }
    }

    [Register(false)]
    internal sealed class Client : INetEventListener, IClient
    {
        #region Fields and Properties
        private bool _isRunning = false;
        private NetPeer? _server;
        private readonly NetManager _netManager;
        private readonly Thread _workingThread;
        private readonly IServerSettings _settings;
        private readonly IClientAuthorizationProvider _clientAuthorizationProvider;
        public int Ping { get; private set; }
        public bool Connected { get; private set; }
        #endregion

        public Client(IServerSettings settings, IClientAuthorizationProvider clientAuthorizationProvider)
        {
            this.Log("Client created.");
            _settings = settings;
            _clientAuthorizationProvider = clientAuthorizationProvider;
            _netManager = new NetManager(this)
            {
                AutoRecycle = true,
                IPv6Enabled = false
            };
            _netManager.Start();
            _workingThread = new Thread(Work) { IsBackground = true };
        }

        #region Public API
        public Action<NetworkRequest> OnNetworkRequestReceived { get; set; }
        public Action<ConfirmationResponseRequest> OnConnected { get; set; }
        public Action<RemotePlayerConnectedRequest> OnRemotePlayerConnected { get; set; }
        public Action<RemotePlayerDisconnectedRequest> OnRemotePlayerDisconnected { get; set; }
        public void Connect(string ip)
        {
            this.Log($"Client.Trying to connect: {ip}");
            _netManager.Connect(ip, _settings.DefaultServerPort, _settings.ServerConnectionString);
            _isRunning = true;
            _workingThread.Start();
        }

        public void Disconnect()
        {
            _netManager.DisconnectAll();
            _isRunning = false;
            Connected = false;
        }

        public void SendReliable(NetworkRequest networkRequest)
        {
            if (_server == null) return;
            var serialized = networkRequest.Serialize();
            _server.Send(serialized, DeliveryMethod.ReliableOrdered);
        }

        public void SendUnreliable(NetworkRequest networkRequest)
        {
            if (_server == null) return;
            var serialized = networkRequest.Serialize();
            _server.Send(serialized, DeliveryMethod.Unreliable);
        }
        #endregion

        #region INetEventListener
        void INetEventListener.OnNetworkReceive(NetPeer peer, NetPacketReader reader, byte channelNumber, DeliveryMethod deliveryMethod)
        {
            this.Log("Client.Network request received.");
            NetworkRequest networkRequest = new NetworkRequest();
            networkRequest.Deserialize(reader);

            if (networkRequest.Data == null)
            {
                this.Log("Client.Deserialized network request with null data.", LogLevel.Warning);
                return;
            }

            switch (networkRequest.Id)
            {
                case ConfirmationRequireRequest.REQUEST_ID:
                    {
                        networkRequest.Data.Deserialize(out ConfirmationRequireRequest request);
                        OnConfirmationRequire(peer, in request);
                        break;
                    }
                case ConfirmationResponseRequest.REQUEST_ID:
                    {
                        networkRequest.Data.Deserialize(out ConfirmationResponseRequest confirmationResponseRequest);
                        Connected = confirmationResponseRequest.Accepted;
                        OnConnected?.Invoke(confirmationResponseRequest);
                        break;
                    }
                case RemotePlayerConnectedRequest.REQUEST_ID:
                    {
                        networkRequest.Data.Deserialize(out RemotePlayerConnectedRequest remotePlayerConnectedRequest);
                        OnRemotePlayerConnected?.Invoke(remotePlayerConnectedRequest);
                        break;
                    }
                case RemotePlayerDisconnectedRequest.REQUEST_ID:
                    {
                        networkRequest.Data.Deserialize(out RemotePlayerDisconnectedRequest remotePlayerDisconnected);
                        OnRemotePlayerDisconnected?.Invoke(remotePlayerDisconnected);
                        break;
                    }
                default:
                    OnNetworkRequestReceived?.Invoke(networkRequest);
                    break;
            }
        }

        void INetEventListener.OnPeerConnected(NetPeer peer)
        {
            this.Log($"Client.Connected to server: {peer.Address}");
            _server = peer;
            Connected = true;
        }

        void INetEventListener.OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
        {
            this.Log($"Client.Disconnected from server: {disconnectInfo.Reason}");
            _server = null;
            Connected = false;
        }

        void INetEventListener.OnNetworkError(IPEndPoint endPoint, SocketError socketError)
        {
            this.Log($"Client.Network error: {socketError}", LogLevel.Error);
        }

        void INetEventListener.OnNetworkReceiveUnconnected(IPEndPoint remoteEndPoint, NetPacketReader reader,
            UnconnectedMessageType messageType)
        {
            this.Log("Client.OnNetworkReceiveUnconnected", LogLevel.Warning);
        }

        void INetEventListener.OnNetworkLatencyUpdate(NetPeer peer, int latency)
        {
            Ping = latency;
        }

        void INetEventListener.OnConnectionRequest(ConnectionRequest request)
        {
            request.Reject();
        }

        #endregion

        #region Private
        private async void Work()
        {
            while (_isRunning)
            {
                Update();
                await Task.Delay(_settings.MsDelayInTheNetworkThread);
            }
        }

        private void Update()
        {
            _netManager.PollEvents();
        }

        private void OnConfirmationRequire(NetPeer peer, in ConfirmationRequireRequest request)
        {
            Task.Factory.StartNew(async () => await Confirm());
        }

        private async Task Confirm()
        {
            var confirmation = await _clientAuthorizationProvider.Authorize();
            SendReliable(confirmation.ToRequest());
        }

        #endregion
    }
}
