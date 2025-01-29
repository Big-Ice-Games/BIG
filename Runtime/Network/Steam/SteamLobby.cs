using Steamworks;

namespace BIG.Network
{
    internal sealed class SteamLobby
    {
        public CSteamID LobbyId { get; private set; }

        protected Callback<LobbyEnter_t> LobbyEnteredCallback;
        protected Callback<LobbyCreated_t> LobbyCreatedCallback;
        protected Callback<LobbyMatchList_t> GetLobbiesCallback;
        protected Callback<GameRichPresenceJoinRequested_t> JoinSomeoneElseLobbyRequestedCallback;
  
        protected Callback<LobbyChatUpdate_t> LobbyChatUpdated;
        protected Callback<GameLobbyJoinRequested_t> LobbyJoinRequestedCallback;
        protected Callback<LobbyDataUpdate_t> GetLobbyDataCallback;
        protected Callback<P2PSessionRequest_t> P2PSessionRequestCallback;
        protected Callback<GameServerChangeRequested_t> GameServerChangeRequestedCallback;
        protected Callback<NumberOfCurrentPlayers_t> NumberOfCurrentPlayersCallback;
    }
}

    //public struct SteamLobbyIAmReadyRequest
    //{
    //    public const byte REQUEST_IDENTIFIER = 170;
    //}

    //public struct SteamLobbyIAmNotReadyRequest
    //{
    //    public const byte REQUEST_IDENTIFIER = 171;
    //}

    //public struct SteamLobbyHostStartingNewMatchRequest
    //{
    //    public const byte REQUEST_IDENTIFIER = 172;

    //    [Index(0)] public int LobbyId;
    //    [Index(1)] public string RegionToken;

    //    public SteamLobbyHostStartingNewMatchRequest(int lobbyId, string regionToken)
    //    {
    //        LobbyId = lobbyId;
    //        RegionToken = regionToken;
    //    }

    //    public static void Register()
    //    {
    //        var structFormatter = new SteamLobbyHostStartingNewMatchRequestFormatter<DefaultResolver>();
    //        Formatter<DefaultResolver, SteamLobbyHostStartingNewMatchRequest>.Register(structFormatter);
    //        Formatter<DefaultResolver, SteamLobbyHostStartingNewMatchRequest?>.Register(new NullableStructFormatter<DefaultResolver, SteamLobbyHostStartingNewMatchRequest>(structFormatter));
    //    }

    //    public class SteamLobbyHostStartingNewMatchRequestFormatter<TTypeResolver> : Formatter<TTypeResolver, SteamLobbyHostStartingNewMatchRequest>
    //        where TTypeResolver : ITypeResolver, new()
    //    {
    //        private readonly Formatter<TTypeResolver, int> _formatter0;
    //        private readonly Formatter<TTypeResolver, string> _formatter1;

    //        public override bool NoUseDirtyTracker => _formatter0.NoUseDirtyTracker;
    //        public SteamLobbyHostStartingNewMatchRequestFormatter()
    //        {
    //            _formatter0 = Formatter<TTypeResolver, int>.Default;
    //            _formatter1 = Formatter<TTypeResolver, string>.Default;
    //        }

    //        public override int? GetLength() => null;
    //        public override int Serialize(ref byte[] bytes, int offset, SteamLobbyHostStartingNewMatchRequest value)
    //        {
    //            //BinaryUtil.EnsureCapacity(ref bytes, offset, 4);
    //            var startOffset = offset;
    //            offset += _formatter0.Serialize(ref bytes, offset, value.LobbyId);
    //            offset += _formatter1.Serialize(ref bytes, offset, value.RegionToken);
    //            return offset - startOffset;
    //        }

    //        public override SteamLobbyHostStartingNewMatchRequest Deserialize(ref byte[] bytes, int offset, DirtyTracker tracker, out int byteSize)
    //        {
    //            byteSize = 0;
    //            int size;
    //            var item0 = _formatter0.Deserialize(ref bytes, offset, tracker, out size);
    //            offset += size;
    //            byteSize += size;
    //            var item1 = _formatter1.Deserialize(ref bytes, offset, tracker, out size);
    //            offset += size;
    //            byteSize += size;

    //            return new SteamLobbyHostStartingNewMatchRequest(item0, item1);
    //        }
    //    }
    //}

    //public struct SteamLobbyKickedFromTeamRequest
    //{
    //    public const byte REQUEST_IDENTIFIER = 173;
    //}

    //public class SteamLobby : SpookedBehaviour
    //{
        // private Dictionary<ulong, SteamFriend> _friends;
        // private List<CSteamID> _steamRoomMembers;
        // private Dictionary<ulong, SteamFriend> _roomMemberDetails;
        // private Dictionary<ulong, bool> _readiness;
        // private readonly HashSet<ulong> _accepted = new HashSet<ulong>();
        //
        // public bool IsReady(ulong steamId) => _readiness[steamId];
        //
        // public bool AreAllPlayersReady()
        // {
        //     foreach (CSteamID roomMember in _steamRoomMembers)
        //     {
        //         if(roomMember.m_SteamID == Game.Game.SteamId) continue;
        //         if (!_readiness[roomMember.m_SteamID])
        //             return false;
        //     }
        //
        //     return true;
        // }
        //
        // /// <summary>
        // /// X ready.
        // /// Y team count.
        // /// </summary>
        // public Vector2 PlayersReady
        // {
        //     get
        //     {
        //         int ready = 0;
        //         foreach (CSteamID roomMember in _steamRoomMembers)
        //         {
        //             if (_readiness[roomMember.m_SteamID])
        //                 ready++;
        //         }
        //
        //         return new Vector2(ready, _steamRoomMembers.Count);
        //     }
        // }
        //
        // public bool AmIReady { get; private set; }
        //
        // public int HostKinguinversId { get; private set; }
        //
        // public bool AmITeamLeader => HostKinguinversId == 0 || HostKinguinversId == Game.Game.KinguinverseId;
        //
        // public List<SteamFriend> Team
        // {
        //     get
        //     {
        //         try
        //         {
        //             List<SteamFriend> team = new List<SteamFriend>(_steamRoomMembers.Count);
        //             for (int i = 0; i < _steamRoomMembers.Count; i++)
        //                 team.Add(_roomMemberDetails[_steamRoomMembers[i].m_SteamID]);
        //
        //             return team;
        //         }
        //         catch (Exception e)
        //         {
        //             this.Log($"Failed to get team: {e}", LogLevel.Error);
        //             return new List<SteamFriend>();
        //         }
        //     }
        // }
        //
        // public int TeamCount => _steamRoomMembers.Count;
        //
        // [SerializeField] private ulong _steamAppId;
        // public CSteamID LobbyId { get; private set; }
        // public int CurrentMembers { get; private set; }
        // public int MaxMembers { get; private set; }
        //
        // [Inject] private KinguinverseWebService _kinguinverseWebService;
        //[Inject] private MainThreadActionsQueue _mainThreadActionsQueue;
        //// [Inject] private SpookedLobbyUtils _spookedLobbyUtils;
        ////
        //// protected Callback<LobbyEnter_t> LobbyEnteredCallback;
        //// protected Callback<LobbyCreated_t> LobbyCreatedCallback;
        //// //protected Callback<LobbyMatchList_t> GetLobbiesCallback;
        //// protected Callback<GameRichPresenceJoinRequested_t> JoinSomeoneElseLobbyRequestedCallback;
        //protected Callback<PersonaStateChange_t> PersonaStateChangedCallback;
        // protected Callback<LobbyChatUpdate_t> LobbyChatUpdated;
        // //protected Callback<getfri>
        //
        // protected Callback<GameLobbyJoinRequested_t> LobbyJoinRequestedCallback;
        // protected Callback<LobbyDataUpdate_t> GetLobbyDataCallback;
        // protected Callback<P2PSessionRequest_t> P2PSessionRequestCallback;
        //
        // //protected Callback<GameServerChangeRequested_t> GameServerChangeRequestedCallback;
        // //protected Callback<NumberOfCurrentPlayers_t> NumberOfCurrentPlayersCallback;
        //
        // public bool Wishlisted()
        // {
        //     return SteamApps.BIsSubscribedApp(new AppId_t((uint)_steamAppId));
        // }
        //
        //protected override void OnAwake()
        //{
        //    base.OnAwake();
        //    //     _friends = new Dictionary<ulong, SteamFriend>();
        //    //     _steamRoomMembers = new List<CSteamID>(6);
        //    //     _roomMemberDetails = new Dictionary<ulong, SteamFriend>();
        //    //     _readiness = new Dictionary<ulong, bool>();
        //    //
        //    //     LobbyEnteredCallback = Callback<LobbyEnter_t>.Create(OnLobbyEnteredCallback);
        //    //     LobbyCreatedCallback = Callback<LobbyCreated_t>.Create(OnLobbyCreatedCallback);
        //    //     JoinSomeoneElseLobbyRequestedCallback = Callback<GameRichPresenceJoinRequested_t>.Create(OnJoinSomeoneElseLobbyRequestedCallback);
        //    PersonaStateChangedCallback = Callback<PersonaStateChange_t>.Create(OnPersonaStateChangedCallback);
        //    //     LobbyChatUpdated = Callback<LobbyChatUpdate_t>.Create(OnLobbyChatUpdated);
        //    //     LobbyJoinRequestedCallback = Callback<GameLobbyJoinRequested_t>.Create(OnLobbyJoinRequestedCallback);
        //    //     P2PSessionRequestCallback = Callback<P2PSessionRequest_t>.Create(OnP2PSessionRequest);
        //}
        //
        // private void OnP2PSessionRequest(P2PSessionRequest_t request)
        // {
        //     CSteamID clientId = request.m_steamIDRemote;
        //     if (_accepted.Contains(clientId.m_SteamID)) return;
        //     SteamNetworking.AcceptP2PSessionWithUser(clientId);
        //     _accepted.Add(clientId.m_SteamID);
        // }
        //
        // public void StopAcceptingConnections()
        // {
        //     foreach (ulong id in _accepted)
        //     {
        //         SteamNetworking.CloseP2PSessionWithUser(new CSteamID(id));
        //     }
        //
        //     _accepted.Clear();
        // }
        //
        //private void OnPersonaStateChangedCallback(PersonaStateChange_t callback)
        //{
        //    _mainThreadActionsQueue.Enqueue(() =>
        //        Publish(new SteamFriendStatusChangeEvent(callback)));

        //    // CSteamID friendId = new CSteamID(callback.m_ulSteamID);
        //    // if (_friends.ContainsKey(callback.m_ulSteamID))
        //    // {
        //    //     bool isOnline = SteamFriends.GetFriendPersonaState(friendId) == EPersonaState.k_EPersonaStateOnline;
        //    //     bool inGame = false;
        //    //     if (SteamFriends.GetFriendGamePlayed(friendId, out FriendGameInfo_t game))
        //    //     {
        //    //         inGame = isOnline && (game.m_gameID.AppID().m_AppId == _steamAppId);
        //    //     }
        //    //
        //    //     _friends[callback.m_ulSteamID].InGame = inGame;
        //    //     _friends[callback.m_ulSteamID].Online = isOnline;
        //    // }
        //    //
        //    //
        //    // _mainThreadActionsQueue.Enqueue(() =>
        //    //     Publish(new SpookedFriendsRefreshedFullEvent(_friends.Select(s => s.Value).ToList())));
        //}
        //
        // public async Task GetFriends()
        // {
        //     int friendsCount = SteamFriends.GetFriendCount(EFriendFlags.k_EFriendFlagImmediate);
        //     List<SteamFriend> friends = new List<SteamFriend>(friendsCount);
        //     for (int i = 0; i < friendsCount; ++i)
        //     {
        //         CSteamID friendSteamID = SteamFriends.GetFriendByIndex(i, EFriendFlags.k_EFriendFlagImmediate);
        //         if (_friends.ContainsKey(friendSteamID.m_SteamID)) continue;
        //
        //         var data = await _kinguinverseWebService.GetAllUserData(friendSteamID.m_SteamID).ConfigureAwait(false);
        //         if (data.IsFailure || data.Value == null) continue;
        //
        //         string nickname = SteamFriends.GetFriendPersonaName(friendSteamID);
        //         bool isOnline = SteamFriends.GetFriendPersonaState(friendSteamID) == EPersonaState.k_EPersonaStateOnline;
        //         bool inGame = false;
        //         if (SteamFriends.GetFriendGamePlayed(friendSteamID, out FriendGameInfo_t game))
        //         {
        //             inGame = isOnline && (game.m_gameID.AppID().m_AppId == _steamAppId);
        //         }
        //
        //         SteamFriend f = new SteamFriend(friendSteamID, nickname, isOnline, inGame,
        //             data.Value.Inventory.GetInventory(),
        //             data.Value.GameUserMetadata.MapGameUserMetadatasToLobbyPlayerProperties(nickname),
        //             data.Value.Banned,
        //             data.Value.UserId);
        //         friends.Add(f);
        //     }
        //
        //     foreach (SteamFriend steamFriend in friends)
        //     {
        //         _friends.Add(steamFriend.SteamId.m_SteamID, steamFriend);
        //     }
        //
        //     _mainThreadActionsQueue.Enqueue(() =>
        //     {
        //         Publish(new SpookedFriendsRefreshedFullEvent(_friends.Select(s => s.Value).ToList()));
        //         if(TeamCount > 1)
        //             Publish(new SpookedTeamRefreshEvent(Team, HostKinguinversId));
        //     });
        // }
        //
        // private async Task<SteamFriend> GetSteamUserDetail(ulong steamId)
        // {
        //     if (_friends.ContainsKey(steamId))
        //         return _friends[steamId];
        //
        //     var data = await _kinguinverseWebService.GetAllUserData(steamId).ConfigureAwait(false);
        //     if (data.IsFailure || data.Value == null) return null;
        //
        //     return new SteamFriend(new CSteamID(steamId), data.Value.Nickname, true, true,
        //         data.Value.Inventory.GetInventory(),
        //         data.Value.GameUserMetadata.MapGameUserMetadatasToLobbyPlayerProperties(data.Value.Nickname),
        //         data.Value.Banned, data.Value.UserId);
        // }
        //
        // private void OnJoinSomeoneElseLobbyRequestedCallback(GameRichPresenceJoinRequested_t callback)
        // {
        //     this.Log($"OnJoinSomeoneElseLobbyRequestedCallback: {callback.m_steamIDFriend.m_SteamID}:{callback.m_rgchConnect}", LogLevel.Info);
        //     if (ulong.TryParse(callback.m_rgchConnect, out ulong lobbyId))
        //     {
        //         Leave();
        //         JoinCustomGame(lobbyId);
        //     }
        // }
        //
        // [RegisterEventHandler(typeof(OnTeamInvitationClickedEvent))]
        // private void OnTeamInvitationClickedEvent(object sender, EventArgs args)
        // {
        //     OnTeamInvitationClickedEvent e = args as OnTeamInvitationClickedEvent;
        //     bool result = InviteFriendToTeam(e.WhomSteamId, LobbyId.m_SteamID.ToString());
        //     if (!result)
        //     {
        //         this.Log("Failed to invite friend to team", LogLevel.Error);
        //     }
        //     else
        //     {
        //         this.Log("Team invitation sent.", LogLevel.Info);
        //     }
        // }
        //
        // [RegisterEventHandler(typeof(OnTeamKickClickedEvent))]
        // private void OnTeamKickClickedEvent(object sender, EventArgs args)
        // {
        //     OnTeamKickClickedEvent e = args as OnTeamKickClickedEvent;
        //     if (e.WhomSteamId == Game.Game.SteamId)
        //     {
        //         Leave();
        //         CreateLobby();
        //     }
        //     else if (Game.Game.IsMyKinguinverseId(HostKinguinversId))
        //     {
        //         SendTeamKick(e.WhomSteamId);
        //     }
        // }
        //
        //
        // public bool InviteFriendToTeam(ulong friendId, string kinguinversId)
        // {
        //     return SteamFriends.InviteUserToGame(new CSteamID(friendId), kinguinversId);
        // }
        //
        // /// <summary>
        // /// We are leaving steam lobby when we are closing the game or we are moving from our own lobby
        // /// to someone's else lobby from invitation.
        // /// </summary>
        // public void Leave()
        // {
        //     this.Log("Leave current lobby..", LogLevel.Info);
        //     SteamMatchmaking.LeaveLobby(LobbyId);
        //     LobbyId = new CSteamID();
        //     _steamRoomMembers = new List<CSteamID>();
        // }
        //
        // /// <summary>
        // /// We can join only specific game from invitation or create our own game.
        // /// </summary>
        // public void JoinCustomGame(ulong lobbyId)
        // {
        //     SteamMatchmaking.JoinLobby(new CSteamID(lobbyId));
        // }
        //
        // private void OnLobbyEnteredCallback(LobbyEnter_t callback)
        // {
        //     this.Log("On lobby entered.");
        //     LobbyId = new CSteamID(callback.m_ulSteamIDLobby);
        //     CurrentMembers = SteamMatchmaking.GetNumLobbyMembers(LobbyId);
        //     MaxMembers = SteamMatchmaking.GetLobbyMemberLimit(LobbyId);
        //     string hostString = SteamMatchmaking.GetLobbyData(LobbyId, "host");
        //     if (int.TryParse(hostString, out int host))
        //     {
        //         HostKinguinversId = host;
        //     }
        //     else
        //     {
        //         this.Log($"Failed to get steam lobby host {hostString ?? ""}.", LogLevel.Error);
        //     }
        //
        //     OnLobbyChatUpdated(new LobbyChatUpdate_t() { m_ulSteamIDLobby = LobbyId.m_SteamID });
        //     // string photonLobbyId = SteamMatchmaking.GetLobbyData(LobbyId, "photon");
        //     //this.Log($"Mamy photon id cza by dołączyć: {photonLobbyId}");
        //
        //     if (!AmITeamLeader && PhotonLobby.Runner != null)
        //     {
        //         if(PhotonLobby.Runner.SessionInfo.Properties.TryGetValue(LobbyValueType.id.ToString(), out var lobbyId))
        //         {
        //             if(lobbyId != HostKinguinversId.ToString())
        //                 _spookedLobbyUtils.JoinLobby(HostKinguinversId.ToString());
        //         }
        //     }
        //
        //     SendImNotReady();
        // }
        //
        // public void CreateLobby()
        // {
        //     SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypePublic, 6);
        // }
        //
        // private void OnLobbyCreatedCallback(LobbyCreated_t callback)
        // {
        //     this.Log("Lobby created.");
        //     LobbyId = new CSteamID(callback.m_ulSteamIDLobby);
        //     SteamMatchmaking.SetLobbyData(LobbyId, "host", Game.Game.KinguinverseId.ToString());
        //     //SteamMatchmaking.SetLobbyData(LobbyId, "photon_lobby_id", );
        //     CurrentMembers = SteamMatchmaking.GetNumLobbyMembers(LobbyId);
        //     MaxMembers = SteamMatchmaking.GetLobbyMemberLimit(LobbyId);
        // }
        //
        // //private async void OnLobbyJoinRequestedCallback(GameLobbyJoinRequested_t callback)
        // //{
        // //    if (callback.m_steamIDLobby.m_SteamID == LobbyId.m_SteamID)
        // //    {
        // //        this.Log("Ignore invitation because we are already in the same game.");
        // //        return;
        // //    }
        //
        //
        // //    //GameEventsManager.Publish(this, new ScopeCleanEvent());
        // //    var customGameJoinedData = await JoinCustomGame(callback.m_steamIDLobby.m_SteamID);
        // //    if (customGameJoinedData == null)
        // //    {
        // //        this.Log("Failed to join other player lobby.", LogLevel.Error);
        // //    }
        // //    else
        // //    {
        // //        this.Log("Joined other player steam lobby.");
        // //        //GameEventsManager.Publish(this, new ScopeCleanEvent());
        // //        this.Log("Joined custom game!!");
        // //        this.Log("OnLobbyJoinRequestedCallback: How to handle joining other photon lobby?");
        // //        // Wyciągnąć z lobby values id photonowego lobby i do niego dołączyć
        // //    }
        // //}
        //
        // private void OnLobbyJoinRequestedCallback(GameLobbyJoinRequested_t callback)
        // {
        //     this.Log("On lobby join requested callback");
        //     //JoinCustomGame(callback.m_steamIDFriend.m_SteamID);
        //     this.Log("Wyciągnąć z lobby values id photonowego lobby i do niego dołączyć");
        // }
        //
        // private List<CSteamID> GetSteamLobbyMembers(ulong lobbyId)
        // {
        //     var lobbySteamId = new CSteamID(lobbyId);
        //     int lobbyMembers = SteamMatchmaking.GetNumLobbyMembers(lobbySteamId);
        //     List<CSteamID> members = new List<CSteamID>(lobbyMembers);
        //     for (int i = 0; i < lobbyMembers; i++)
        //     {
        //         var lobbyMember = SteamMatchmaking.GetLobbyMemberByIndex(lobbySteamId, i);
        //         members.Add(lobbyMember);
        //     }
        //
        //     return members;
        // }
        //
        // private async void OnLobbyChatUpdated(LobbyChatUpdate_t callback)
        // {
        //     _steamRoomMembers = GetSteamLobbyMembers(callback.m_ulSteamIDLobby);
        //     foreach (CSteamID roomMember in _steamRoomMembers)
        //     {
        //         if(!_roomMemberDetails.ContainsKey(roomMember.m_SteamID))
        //             _roomMemberDetails.Add(roomMember.m_SteamID, await GetSteamUserDetail(roomMember.m_SteamID).ConfigureAwait(true));
        //
        //         _readiness.TryAdd(roomMember.m_SteamID, false);
        //     }
        //
        //     bool hostInTeam = false;
        //     foreach (CSteamID member in _steamRoomMembers)
        //     {
        //         if (_roomMemberDetails[member.m_SteamID].UserId == HostKinguinversId)
        //         {
        //             hostInTeam = true;
        //             break;
        //         }
        //     }
        //
        //     if (!hostInTeam)
        //     {
        //         Leave();
        //         CreateLobby();
        //         _spookedLobbyUtils.JoinLobby("");
        //         return;
        //     }
        //
        //     _mainThreadActionsQueue.Enqueue(() => 
        //         Publish(new SpookedTeamRefreshEvent(Team, HostKinguinversId)));
        //     this.Log($"OnLobbyChatUpdated: {_steamRoomMembers.Count}");
        // }
        //
        // public bool IsTeamMember(ulong steamId)
        // {
        //     return _steamRoomMembers.Any(s => s.m_SteamID == steamId);
        // }
        //
        // private bool ReceiveData(out byte[] result, out ulong networkId)
        // {
        //     if (SteamNetworking.IsP2PPacketAvailable(out uint size))
        //     {
        //         result = new byte[size];
        //
        //         if (SteamNetworking.ReadP2PPacket(result, size, out var _, out CSteamID remoteId))
        //         {
        //             networkId = remoteId.m_SteamID;
        //             return true;
        //         }
        //     }
        //
        //     result = null;
        //     networkId = 0;
        //     return false;
        // }
        //
        // public void SendTeamKick(ulong roomMember)
        // {
        //     NetworkRequest networkRequest = new NetworkRequest(ReceiveType.SinglePlayer, SteamLobbyKickedFromTeamRequest.REQUEST_IDENTIFIER, Array.Empty<byte>());
        //     var serialized = networkRequest.Serialize();
        //     SendData(new CSteamID(roomMember), serialized);
        // }
        //
        // public void SendImReady()
        // {
        //     AmIReady = true;
        //     NetworkRequest networkRequest = new NetworkRequest(ReceiveType.SinglePlayer, SteamLobbyIAmReadyRequest.REQUEST_IDENTIFIER, Array.Empty<byte>());
        //     var serialized = networkRequest.Serialize();
        //     foreach (CSteamID roomMember in _steamRoomMembers)
        //     {
        //         SendData(roomMember, serialized);
        //     }
        // }
        //
        // public void SendImNotReady()
        // {
        //     AmIReady = false;
        //     NetworkRequest networkRequest = new NetworkRequest(ReceiveType.SinglePlayer, SteamLobbyIAmNotReadyRequest.REQUEST_IDENTIFIER, Array.Empty<byte>());
        //     var serialized = networkRequest.Serialize();
        //     foreach (CSteamID roomMember in _steamRoomMembers)
        //     {
        //         SendData(roomMember, serialized);
        //     }
        // }
        //
        // public void SendHostStartingMatch(int lobbyId, string regionToken)
        // {
        //     SteamLobbyHostStartingNewMatchRequest request = new SteamLobbyHostStartingNewMatchRequest(lobbyId, regionToken);
        //     NetworkRequest networkRequest = new NetworkRequest(ReceiveType.SinglePlayer, SteamLobbyHostStartingNewMatchRequest.REQUEST_IDENTIFIER, request.Serialize());
        //     var serialized = networkRequest.Serialize();
        //     foreach (CSteamID roomMember in _steamRoomMembers)
        //     {
        //         SendData(roomMember, serialized);
        //     }
        // }
        //
        // private void SendData(CSteamID receiver, byte[] data)
        // {
        //     SteamNetworking.SendP2PPacket(receiver, data, (uint)data.Length, EP2PSend.k_EP2PSendReliable);
        // }
        //
        // private void Update()
        // {
        //     InternalTick();
        // }
        //
        // private void InternalTick()
        // {
        //     try
        //     {
        //         while (ReceiveData(out byte[] result, out ulong networkId))
        //         {
        //             NetworkRequest networkRequest = result.Deserialize<NetworkRequest>();
        //             if (networkRequest.Id == SteamLobbyIAmReadyRequest.REQUEST_IDENTIFIER)
        //             {
        //                 _readiness[networkId] = true;
        //                 Publish(new SpookedTeamRefreshEvent(Team, HostKinguinversId));
        //             }
        //             else if (networkRequest.Id == SteamLobbyIAmNotReadyRequest.REQUEST_IDENTIFIER)
        //             {
        //                 _readiness[networkId] = false;
        //                 Publish(new SpookedTeamRefreshEvent(Team, HostKinguinversId));
        //             }
        //             else if (networkRequest.Id == SteamLobbyHostStartingNewMatchRequest.REQUEST_IDENTIFIER)
        //             {
        //                 var req = networkRequest.Data.Deserialize<SteamLobbyHostStartingNewMatchRequest>();
        //                 
        //                 this.Log("Host starting new match..", LogLevel.Info);
        //                 
        //                 if(!AmITeamLeader)
        //                     _spookedLobbyUtils.JoinAsTeamMember(req.LobbyId.ToString(), req.RegionToken);
        //             }
        //             else if (networkRequest.Id == SteamLobbyKickedFromTeamRequest.REQUEST_IDENTIFIER)
        //             {
        //                 Leave();
        //                 CreateLobby();
        //             }
        //             
        //             this.Log($"Received {networkRequest.Id} from {networkId}", LogLevel.Info);
        //         }
        //     }
        //     catch (Exception e)
        //     {
        //         this.Log($"Exception occur Receiving data from steam lobby member: {e}", LogLevel.Error);
        //     }
        // }
        //
        // protected override void BeforeDispose()
        // {
        //     base.BeforeDispose();
        //     LobbyEnteredCallback?.Dispose();
        //     LobbyCreatedCallback?.Dispose();
        //     JoinSomeoneElseLobbyRequestedCallback?.Dispose();
        //     PersonaStateChangedCallback?.Dispose();
        //     LobbyJoinRequestedCallback?.Dispose();
        //     GetLobbyDataCallback?.Dispose();
        //     LobbyChatUpdated?.Dispose();
        //
        //     StopAcceptingConnections();
        // }
   // }
//}

