namespace BIG.Network
{
    [Register(true)]
    public sealed class Steam
    {
        private readonly BigSteamNetwork _network;
        private readonly BigSteamLobby _lobby;
        private readonly BigSteamFriends _friends;

        internal Steam(BigSteamNetwork network, BigSteamLobby lobby, BigSteamFriends friends)
        {
            _network = network;
            _lobby = lobby;
            _friends = friends;
        }

        public void Initialize()
        {

        }

        public void Update()
        {
            _network.Update();
        }
    }
}
