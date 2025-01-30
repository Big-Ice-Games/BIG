namespace BIG.Network
{
    [Register(true)]
    public sealed class Steam
    {
        private readonly BigSteamLobby _lobby;
        private readonly BigSteamFriends _friends;

        internal Steam(BigSteamLobby lobby, BigSteamFriends friends)
        {
            _lobby = lobby;
            _friends = friends;
        }

        public void Initialize()
        {

        }

        public void Update()
        {

        }
    }
}
