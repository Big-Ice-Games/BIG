using System;
using System.Collections.Generic;
using System.Linq;
using Steamworks;

namespace BIG.Network
{
    public sealed class SteamFriend
    {
        public CSteamID SteamId;
        public string Nickname;
        public bool IsOnline;
        public bool InTheGame;
    }

    internal sealed class BigSteamFriends
    {
        private readonly ISettings _settings;
        public List<SteamFriend> Friends { get; private set; }
        public Action<List<SteamFriend>> FriendsChanged { get; private set; }

        private Callback<PersonaStateChange_t> _personaStateChangedCallback;

        public BigSteamFriends(ISettings settings)
        {
            _settings = settings;
            Friends = new List<SteamFriend>();
            _personaStateChangedCallback = Callback<PersonaStateChange_t>.Create(OnPersonaStateChangedCallback);
        }

        private void OnPersonaStateChangedCallback(PersonaStateChange_t callback)
        {
            for (int i = 0; i < Friends.Count; i++)
            {
                if (Friends[i].SteamId.m_SteamID == callback.m_ulSteamID)
                {
                    RefreshFriend(Friends[i]);
                    break;
                }
            }

            FriendsChanged?.Invoke(Friends);
        }

        private void RefreshFriend(SteamFriend friend)
        {
            friend.Nickname = SteamFriends.GetFriendPersonaName(friend.SteamId);
            friend.IsOnline = SteamFriends.GetFriendPersonaState(friend.SteamId) == EPersonaState.k_EPersonaStateOnline;
            if (SteamFriends.GetFriendGamePlayed(friend.SteamId, out FriendGameInfo_t game))
            {
                friend.InTheGame = friend.IsOnline && (game.m_gameID.AppID().m_AppId == _settings.SteamAppId);
            }
        }

        /// <summary>
        /// Refresh <see cref="Friends"/> property.
        /// </summary>
        private void GetFriends()
        {
            int friendsCount = SteamFriends.GetFriendCount(EFriendFlags.k_EFriendFlagImmediate);
            List<SteamFriend> friends = new List<SteamFriend>(friendsCount);
            for (int i = 0; i < friendsCount; ++i)
            {
                CSteamID friendSteamId = SteamFriends.GetFriendByIndex(i, EFriendFlags.k_EFriendFlagImmediate);

                // If this friend is already on the list we can skip.
                if (Friends.Any(f => f.SteamId.m_SteamID == friendSteamId.m_SteamID))
                    continue;

                SteamFriend newFriend = new SteamFriend() { SteamId = friendSteamId };
                RefreshFriend(newFriend);
                friends.Add(newFriend);
            }
        }
    }
}
