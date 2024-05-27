using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using Magus.MatchmakingSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Magus.Multiplayer
{
    public class ConnectionManager : NetworkBehaviour
    {
        public static ConnectionManager instance;

        public readonly SyncDictionary<NetworkConnection, int> playerData = new(new SyncTypeSettings(1f));

        public Dictionary<int, string> playerNames = new();

        private void Awake()
        {
            if(instance != null)
            {
                Destroy(gameObject);
            }
            instance = this;
        }

        public void AddPlayerData(int playerNumber, NetworkConnection conn)
        {
            playerData.Add(conn, playerNumber);
        }

        public void GatherPlayerNames()
        {
            playerNames = new();
            foreach (var player in LobbyManager.instance.Lobby.Players)
            {
                int playerNumber = Int32.Parse(player.Data["PlayerNumber"].Value);
                playerNames.Add(playerNumber, player.Data["PlayerName"].Value);
            }
        }

        public void GatherTrainingPlayer()
        {
            playerNames = new()
            {
                { 1, PlayerInfoManager.instance.PlayerInfo.username }
            };
        }
    }
}
