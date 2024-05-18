using FishNet.Connection;
using FishNet.Object;
using Magus.Game;
using Magus.Multiplayer;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Magus.SceneSpecific
{
    public class BattlePlayerSpawner : NetworkBehaviour
    {
        [SerializeField] private NetworkObject clientPlayer;

        private NetworkObject spawnedPlayer;

        public override void OnStartClient()
        {
            base.OnStartClient();
            SpawnPlayer(base.LocalConnection);
        }

        [ServerRpc(RequireOwnership = false)]
        private void SpawnPlayer(NetworkConnection conn)
        {
            spawnedPlayer = Instantiate(clientPlayer, GlobalPlayerController.instance.GetSpawnpoint(conn), Quaternion.identity);
            ServerManager.Spawn(spawnedPlayer, conn, ServerManager.Clients[conn.ClientId].Scenes.First(x => x.name == "BattleScene"));
        }
    }
}
