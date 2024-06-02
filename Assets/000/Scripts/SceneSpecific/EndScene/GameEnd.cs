using FishNet;
using FishNet.Connection;
using FishNet.Managing.Scened;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using Magus.Global;
using Magus.Multiplayer;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Magus.SceneSpecific
{
    public class GameEnd : NetworkBehaviour
    {
        private readonly SyncVar<float> countdownTimer = new SyncVar<float>(new SyncTypeSettings(1f));
        public float CountdownTimer => countdownTimer.Value;

        public event Action<float> OnCountdownChanged;
        public event Action<int, int> OnLoadWinner;

        private bool stoppedConnection;

        public int winner;

        private void Awake()
        {
            countdownTimer.OnChange += CountdownChanged;
            OnCountdownChanged?.Invoke(Constants.ENDING_COUNTDOWN);
        }

        private void OnDestroy()
        {
            countdownTimer.OnChange -= CountdownChanged;
            if(base.IsServerInitialized) {
                base.SceneManager.OnLoadEnd -= OnLoadEnd;
                base.SceneManager.OnClientPresenceChangeEnd -= SceneManager_OnClientPresenceChangeEnd;
            }
        }

        public override void OnStartServer()
        {
            base.OnStartServer();
            stoppedConnection = false;
            countdownTimer.Value = Constants.ENDING_COUNTDOWN;
            base.SceneManager.OnLoadEnd += OnLoadEnd;
            base.SceneManager.OnClientPresenceChangeEnd += SceneManager_OnClientPresenceChangeEnd;
        }

        private void SceneManager_OnClientPresenceChangeEnd(ClientPresenceChangeEventArgs args)
        {
            if(args.Scene.name == "EndScene" && args.Added)
            {
                SetWinner(args.Connection, winner);
            }
        }

        private void OnLoadEnd(SceneLoadEndEventArgs args)
        {
            if (!args.QueueData.AsServer) return;
            int[] serverParams = args.QueueData.SceneLoadData.Params.ServerParams.Cast<int>().ToArray();
            winner = serverParams[0];
        }

        [TargetRpc]
        private void SetWinner(NetworkConnection conn, int winner)
        {
            this.winner = winner;
            OnLoadWinner?.Invoke(ConnectionManager.instance.playerData[conn], winner);
        }

        private void CountdownChanged(float prev, float next, bool asServer)
        {
            if (!asServer)
            {
                OnCountdownChanged?.Invoke(next);
            }
            else
            {
                if(next <= 0 && !stoppedConnection)
                {
                    //base.ServerManager.StopConnection(true);
                    stoppedConnection = true;
                }
            }
        }

        private void Update()
        {
            if (base.IsServerInitialized)
            {
                if (countdownTimer.Value > 0 && !stoppedConnection)
                {
                    countdownTimer.Value -= Time.deltaTime;
                }
            }
        }
    }
}
