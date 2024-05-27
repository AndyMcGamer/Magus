using FishNet.Managing.Scened;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using Magus.Global;
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

        public event Action<float> OnCountdownChanged;
        public event Action<int> OnWinnerLoaded;

        private bool stoppedConnection;

        private void Awake()
        {
            countdownTimer.OnChange += CountdownChanged;
            OnCountdownChanged?.Invoke(Constants.ENDING_INITIAL_TIME);
        }

        private void OnDestroy()
        {
            countdownTimer.OnChange -= CountdownChanged;
            if (base.IsServerInitialized)
            {
                base.SceneManager.OnLoadEnd -= OnLoadEnd;
            }
        }

        public override void OnStartServer()
        {
            base.OnStartServer();
            stoppedConnection = false;
            countdownTimer.Value = Constants.ENDING_INITIAL_TIME;
            if (base.IsServerInitialized)
            {
                base.SceneManager.OnLoadEnd += OnLoadEnd;
            }
        }

        private void OnLoadEnd(SceneLoadEndEventArgs args)
        {
            if (!args.QueueData.AsServer) return;
            int[] serverParams = args.QueueData.SceneLoadData.Params.ServerParams.Cast<int>().ToArray();
            OnWinnerLoaded?.Invoke(serverParams[0]);
            ObserverWinnerLoaded(serverParams[0]);
        }

        [ObserversRpc(ExcludeServer = true)]
        private void ObserverWinnerLoaded(int winner)
        {
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
