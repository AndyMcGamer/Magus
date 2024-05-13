using FishNet.Connection;
using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Magus.SceneSpecific
{
    public class TrainingPlayerSpawner : NetworkBehaviour
    {
        [SerializeField] private NetworkObject clientPlayer;
        [SerializeField] private NetworkObject serverPlayer;
        [SerializeField] private Vector3 spawnpoint;

        public override void OnStartClient()
        {
            base.OnStartClient();
            //print(UnityEngine.SceneManagement.SceneManager.GetActiveScene().handle);
            SpawnPlayer(base.LocalConnection);
        }

        [ServerRpc(RequireOwnership = false)]
        private void SpawnPlayer(NetworkConnection conn)
        {
            var nob = Instantiate(clientPlayer, spawnpoint, Quaternion.identity);
            ServerManager.Spawn(nob, conn, ServerManager.Clients[conn.ClientId].Scenes.First(x => x.name == "TrainingRoom"));
        }
    }
}
