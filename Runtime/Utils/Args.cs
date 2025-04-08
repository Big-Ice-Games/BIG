// Copyright (c) 2025, Big Ice Games
// All rights reserved.

namespace BIG
{
    public class Args
    {
        /// <summary>
        /// If <see cref="System.Environment.GetCommandLineArgs()"/> contains +connect_lobby argument this function will give you lobby id.
        /// </summary>
        /// <param name="lobbyId">Steam lobby id.</param>
        /// <returns>Value indicating whether there is a lobby player should join.</returns>
        public static bool GetSteamConnectionLobbyId(out ulong lobbyId)
        {
            lobbyId = 0;
            string[] args = System.Environment.GetCommandLineArgs();

            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] != "+connect_lobby" || i + 1 >= args.Length) continue;
                if (ulong.TryParse(args[i + 1], out lobbyId))
                {
                    return true;
                }
            }
            
            return false;
        }
    }
}
