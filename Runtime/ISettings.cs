using BIG.Network.Requests;

namespace BIG
{
    public interface ISettings
    {
        int EntitiesCapacity => 10000;
        uint SteamAppId => 400;
    }

    public interface IServerSettings
    {
        public int DefaultServerPort => 10515;
        public int ServerMaxConnections => 10;
        public string ServerConnectionString => "test";
        public int MsDelayInTheNetworkThread => 15;
        public bool SyncAllClientsToTheNewlyConnectedOne => true;
        public ConfirmationType ServerRequiredConfirmationType => ConfirmationType.None;
    }
}
