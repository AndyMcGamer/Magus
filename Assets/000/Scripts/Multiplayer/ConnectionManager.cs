using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Magus.Multiplayer
{
    public class ConnectionManager : NetworkBehaviour
    {
        public static ConnectionManager instance;

        public readonly SyncDictionary<NetworkConnection, int> playerData = new(new SyncTypeSettings(1f));

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
    }
}
