using System;
using System.Collections.Generic;
using Steamworks;

namespace BIG.Network
{
    internal sealed class SteamNetwork: IDisposable
    {
        private const int CHANNEL = 0;
        private readonly object _sendLocker;
        private readonly object _receiveLocker;
        private readonly Queue<NetworkRequest> _outgoingUnreliableRequests;
        private readonly Queue<NetworkRequest> _outgoingReliableRequests;
        private readonly Queue<NetworkRequest> _incomingRequest;
        private readonly Callback<P2PSessionRequest_t> _sessionRequestCallback;
        private readonly Callback<P2PSessionConnectFail_t> _connectedFailCallback;

        internal SteamNetwork()
        {
            _sendLocker = new object();
            _receiveLocker = new object();
            _outgoingUnreliableRequests = new Queue<NetworkRequest>();
            _outgoingReliableRequests = new Queue<NetworkRequest>();
            _incomingRequest = new Queue<NetworkRequest>();

            _sessionRequestCallback = Callback<P2PSessionRequest_t>.Create(OnP2PSessionRequest);
            _connectedFailCallback = Callback<P2PSessionConnectFail_t>.Create(OnP2PSessionConnectFail);
        }

        private void OnP2PSessionRequest(P2PSessionRequest_t callback) => SteamNetworking.AcceptP2PSessionWithUser(callback.m_steamIDRemote);
        private void OnP2PSessionConnectFail(P2PSessionConnectFail_t callback) => this.Log($"Failed to connect with player {callback.m_steamIDRemote} with error: {callback.m_eP2PSessionError}", Category.Networking,  LogLevel.Error);

        /// <summary>
        /// This function push request to outgoing queue which will be handled by <see cref="Update"/> function.
        /// This function can be called from any thread, any place in any given moment.
        /// </summary>
        /// <param name="request">Requests we want to send.</param>
        /// <param name="reliable">If reliable - otherwise unreliable.</param>
        public void SendData(NetworkRequest request, bool reliable)
        {
            lock (_sendLocker)
            {
                if (reliable)
                    _outgoingReliableRequests.Enqueue(request);
                else
                    _outgoingUnreliableRequests.Enqueue(request);
            }
        }

        /// <summary>
        /// Send outgoing requests.
        /// Receive incoming requests into <see cref="_incomingRequest"/>.
        /// </summary>
        internal void Update()
        {
            #region Send outgoing requests
            void Send(in NetworkRequest request, EP2PSend sendType)
            {
                try
                {
                    var data = request.Serialize();
                    bool success = SteamNetworking.SendP2PPacket(new CSteamID(request.Player), data, (uint)data.Length, sendType, CHANNEL);
                    if(!success)
                        this.Log($"Failed to send request {request} .", Category.Networking, LogLevel.Warning);
                }
                catch
                {
                    this.Log($"Failed to send request {request} .", Category.Networking, LogLevel.Warning);
                }
            }

            lock (_sendLocker)
            {
                while (_outgoingUnreliableRequests.TryDequeue(out NetworkRequest request))
                    Send(in request, EP2PSend.k_EP2PSendUnreliable);


                while (_outgoingReliableRequests.TryDequeue(out NetworkRequest request))
                    Send(in request, EP2PSend.k_EP2PSendReliable);
            }

            #endregion

            #region Receive

            lock (_receiveLocker)
            {
                if (SteamNetworking.IsP2PPacketAvailable(out uint dataSize, CHANNEL))
                {
                    var data = new byte[dataSize];
                    if (SteamNetworking.ReadP2PPacket(data, dataSize, out _, out CSteamID sender, CHANNEL))
                    {
                        data.Deserialize(out NetworkRequest request);
                        // Keep information from whom we received this message.
                        request.Player = sender.m_SteamID;
                        _incomingRequest.Enqueue(request);
                    }
                }
            }
            #endregion
        }

        public void Dispose()
        {
            _sessionRequestCallback.Dispose();
            _connectedFailCallback.Dispose();
            SteamNetworking.CloseP2PSessionWithUser(SteamUser.GetSteamID());
        }
    }
}
