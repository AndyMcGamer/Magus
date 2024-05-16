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

        /// <summary>
        /// Number of rounds needed to win the match
        /// </summary>
        public static readonly int NUM_WINS_NEEDED = 3;

        /// <summary>
        /// Maximum number of rounds in a match
        /// </summary>
        public static readonly int MAX_ROUNDS = 5;

        public static readonly float TRAINING_TIME = 90;
        public static readonly float BATTLE_TIME = 300;
        public static readonly float MAX_TIME = 5999;

        public static readonly float LOADING_INITIAL_TIME = 30;
        public static readonly float LOADING_ACCEL_TIME = 5;

        public static readonly float PROJECTILE_MAX_PASSED_TIME = 0.3f;

        public static readonly string PLAYER_ONE_TAG = "PlayerOne";
        public static readonly string PLAYER_TWO_TAG = "PlayerTwo";
    }
}
