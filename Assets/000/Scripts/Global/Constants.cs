
using UnityEngine;

namespace Magus.Global
{
    public static class Constants
    {
        /// <summary>
        /// Max number of players in Unity Lobby -- set to 2 since it is a 1v1 <br />
        /// Maybe can increase to support spectators or different gamemodes
        /// </summary>
        public const int MAX_PLAYERS = 2;

        /// <summary>
        /// Unity Lobbies timeout after 30s
        /// </summary>
        public const float LOBBY_HEARTBEAT_TIME = 15f;

        /// <summary>
        /// Rate at which lobby requests update
        /// </summary>
        public const float LOBBY_UPDATE_RATE = 1.01f;

        // Transport Indices for Multipass
        public const int UTP_TRANSPORT_INDEX = 0;
        public const int YAK_TRANSPORT_INDEX = 1;

        // Minimum players needed to go from loading to game
        public const int MIN_PLAYERS_SOLO = 1;
        public const int MIN_PLAYERS_1V1 = 2;

        /// <summary>
        /// Number of rounds needed to win the match
        /// </summary>
        public const int NUM_WINS_NEEDED = 3;

        /// <summary>
        /// Maximum number of rounds in a match
        /// </summary>
        public const int MAX_ROUNDS = 5;

        public const float TRAINING_TIME = 30; // should be 90
        public const float BATTLE_TIME = 300; // should be 300
        public const float TRANSITION_TIME = 5;
        public const float MAX_TIME = 5999;

        public const float LOADING_INITIAL_TIME = 30;
        public const float LOADING_ACCEL_TIME = 5;

        public const float PROJECTILE_MAX_PASSED_TIME = 0.3f;

        public const string PLAYER_ONE_TAG = "PlayerOne";
        public const string PLAYER_TWO_TAG = "PlayerTwo";

        public static readonly Vector3 PLAYER_ONE_SPAWN = new(-8, 1, -8);
        public static readonly Vector3 PLAYER_TWO_SPAWN = new(8, 1, 8);

        public const int MAX_HOTBAR_SKILLS = 8;

        public static readonly LayerMask PLAYER_ONE_LAYER = LayerMask.NameToLayer(PLAYER_ONE_TAG);
        public static readonly LayerMask PLAYER_TWO_LAYER = LayerMask.NameToLayer(PLAYER_TWO_TAG);

        public const string CLASSIC_ACTION_MAP = "Classic";
        public const string MODERN_ACTION_MAP = "Modern";

        public const float ENDING_COUNTDOWN = 60;

        public static readonly int[] playerAnimations =
        {
            Animator.StringToHash("Movement"),
            Animator.StringToHash("Attack01"),
            Animator.StringToHash("Attack02"),
            Animator.StringToHash("Attack03"),
            Animator.StringToHash("Attack04"),
        };

        public const string SKILL_COOLDOWN = "CD";
        public const string SKILL_NAME = "NAME";
        public const string SKILL_COST = "COST";
        public const string SKILL_DAMAGE = "DMG";
        public const string SKILL_DISTANCE = "DIST";
        public const string SKILL_STAT = "STAT";
        public const string SKILL_AMOUNT = "AMT";

        public const string RESOURCE_NAME = "Mana";
    }
}
