using FishNet;
using FishNet.Connection;
using FishNet.Managing.Scened;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using FishNet.Transporting.Multipass;
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
        public event Action<int> OnWinnerLoaded;

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
            }
        }

        public override void OnStartServer()
        {
            base.OnStartServer();
            stoppedConnection = false;
            countdownTimer.Value = Constants.ENDING_COUNTDOWN;
            base.SceneManager.OnLoadEnd += OnLoadEnd;
        }

        private void OnLoadEnd(SceneLoadEndEventArgs args)
        {
            if (!args.QueueData.AsServer) return;
            int[] serverParams = args.QueueData.SceneLoadData.Params.ServerParams.Cast<int>().ToArray();
            winner = serverParams[0];
            SetWinner(winner);
        }

        [ObserversRpc]
        private void SetWinner(int winner)
        {
            this.winner = winner;
            OnWinnerLoaded?.Invoke(winner);
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
                    stoppedConnection = true;
                    ConnectionManager.instance.ForceDisconnectServer(base.TransportManager.GetTransport<Multipass>().ClientTransport.Index, immediate: true);
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
