using FishNet.Connection;
using FishNet.Object;
using FishNet.Transporting.Multipass;
using FishNet.Transporting.Yak;
using Magus.MatchmakingSystem;
using Magus.Multiplayer;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Magus.SceneSpecific
{
    public class GameManagerSpawner : NetworkBehaviour
    {
        [SerializeField] private NetworkObject gameManagerObject;

        public override void OnStartServer()
        {
            base.OnStartServer();
            var nob = Instantiate(gameManagerObject, Vector3.zero, Quaternion.identity);
            base.ServerManager.Spawn(nob);
        }

        public override void OnStartClient()
        {
            base.OnStartClient();
            if(base.TransportManager.GetTransport<Multipass>().ClientTransport is Yak)
            {
                AddToConnectionManager(1, base.LocalConnection);
            }
            else
            {
                int playerNumber = Int32.Parse(LobbyManager.instance.LocalPlayer.Data["PlayerNumber"].Value);
                AddToConnectionManager(playerNumber, base.LocalConnection);
            }

        }

        [ServerRpc(RequireOwnership = false)]
        private void AddToConnectionManager(int playerNumber, NetworkConnection conn)
        {
            ConnectionManager.instance.AddPlayerData(playerNumber, conn);
        }
    }
}
