namespace BIG.Network
{
    public sealed class Steam
    {
        private readonly SteamNetwork _network;
        private readonly SteamLobby _lobby;

        public void Update()
        {
            _network.Update();
        }
    }
}
