namespace Magus.Global
{
    public static class Constants
    {
        /// <summary>
        /// Max number of players in Unity Lobby -- set to 2 since it is a 1v1 <br />
        /// Maybe can increase to support spectators or different gamemodes
        /// </summary>
        public static readonly int MAX_PLAYERS = 2;

        /// <summary>
        /// Unity Lobbies timeout after 30s
        /// </summary>
        public static readonly float LOBBY_HEARTBEAT_TIME = 15f;

        /// <summary>
        /// Rate at which lobby requests update
        /// </summary>
        public static readonly float LOBBY_UPDATE_RATE = 1.01f;

        // Transport Indices for Multipass
        public static readonly int UTP_TRANSPORT_INDEX = 0;
        public static readonly int YAK_TRANSPORT_INDEX = 1;

        // Minimum players needed to go from loading to game
        public static readonly int MIN_PLAYERS_SOLO = 1;
        public static readonly int MIN_PLAYERS_1V1 = 2;
    }
}
