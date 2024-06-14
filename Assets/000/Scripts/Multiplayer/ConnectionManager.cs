using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using FishNet.Transporting.Multipass;
using Magus.MatchmakingSystem;
using Magus.SceneManagement;
using Magus.UserInterface;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
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

        public override void OnStartClient()
        {
            base.OnStartClient();
            base.ClientManager.OnClientTimeOut += ClientManager_OnClientTimeOut;
        }

        public override void OnStartServer()
        {
            base.OnStartServer();
        }

        public override void OnStopClient()
        {
            base.OnStopClient();
            base.ClientManager.OnClientTimeOut -= ClientManager_OnClientTimeOut;
            //print("Client Stopperini");
        }

        public override void OnStopServer()
        {
            base.OnStopServer();
            //print("Server Stopperini");
        }

        private void ClientManager_OnClientTimeOut()
        {
            Async_ExitFade(false);
            var mp = base.TransportManager.GetTransport<Multipass>();
            ForceDisconnectServer(mp.ClientTransport.Index, base.LocalConnection);
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

        [ServerRpc(RequireOwnership = false)]
        public void ForceDisconnectServer(int transportIndex, NetworkConnection excludeConnection)
        {
            StartCoroutine(ForceDisconnectCoroutine(transportIndex, excludeConnection));
        }

        private IEnumerator ForceDisconnectCoroutine(int transportIndex, NetworkConnection excludeConnection)
        {
            yield return new WaitForSeconds(1.5f);

            foreach (var conn in base.ServerManager.Clients.Values)
            {
                if (conn == excludeConnection) continue;
                ExitFade(conn);
            }
            
            yield return new WaitForSeconds(0.15f);
            var mp = base.TransportManager.GetTransport<Multipass>();
            mp.StopServerConnection(true, transportIndex);
        }

        [TargetRpc]
        private void ExitFade(NetworkConnection conn)
        {
            Async_ExitFade(true);
        }

        [Client]
        private async void Async_ExitFade(bool fromServer)
        {
            await Fader.instance.FadeIn(easeFunction: DG.Tweening.Ease.OutQuad, reset: false);
            HandleForcedDisconnect(fromServer);
        }

        [Client]
        public void ForceDisconnectClient()
        {
            StartCoroutine(Client_ForceDisconnectCoroutine());
        }

        private IEnumerator Client_ForceDisconnectCoroutine()
        {
            Async_ExitFade(false); // Client Fade

            var mp = base.TransportManager.GetTransport<Multipass>();

            ForceDisconnectServer(mp.ClientTransport.Index, base.LocalConnection);

            yield return new WaitForSeconds(0.75f);

            mp.ClientTransport.StopConnection(false);
        }

        private async void HandleForcedDisconnect(bool fromServer)
        {
            if (fromServer)
            {
                if(LobbyManager.instance.Lobby != null)
                {
                    SceneSwitcher.instance.LoadScene("LobbyScene");
                    LobbyManager.instance.ResetPlayerData();
                }
                else
                {
                    SceneSwitcher.instance.LoadScene("MainMenu");
                }
            }
            else
            {
                if(LobbyManager.instance.Lobby != null)
                {
                    await LobbyManager.instance.LeaveLobby();
                }
                SceneSwitcher.instance.LoadScene("MainMenu");
            }
        }
    }
}
