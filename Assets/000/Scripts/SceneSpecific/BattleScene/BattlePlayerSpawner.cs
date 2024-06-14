using DG.Tweening;
using FishNet.Connection;
using FishNet.Object;
using Magus.Game;
using Magus.Global;
using Magus.Multiplayer;
using Magus.UserInterface;
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

        private int playersSpawned;

        public override void OnStartServer()
        {
            base.OnStartServer();
            playersSpawned = 0;
        }

        public override async void OnStartClient()
        {
            base.OnStartClient();
            SpawnPlayer(base.LocalConnection);
            Banner.instance.SetText("Fight");
            await Banner.instance.FadeIn(0.01f, reset: false);
            await Fader.instance.FadeOut(0.75f, easeFunction: Ease.OutSine);
            await Banner.instance.FadeOut(1f, Ease.InQuart);
            CheckPlayers();
        }

        [ServerRpc(RequireOwnership = false)]
        private void SpawnPlayer(NetworkConnection conn)
        {
            spawnedPlayer = Instantiate(clientPlayer, GlobalPlayerController.instance.GetSpawnpoint(conn), Quaternion.identity);
            ServerManager.Spawn(spawnedPlayer, conn, ServerManager.Clients[conn.ClientId].Scenes.First(x => x.name == "BattleScene"));
            playersSpawned++;
        }

        [ServerRpc(RequireOwnership = false)]
        private void CheckPlayers()
        {
            if (playersSpawned == Constants.MAX_PLAYERS)
            {
                RoundController.instance.SetChangeTimer(true);
            }
        }
    }
}
