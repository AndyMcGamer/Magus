using FishNet.Object;
using FishNet.Object.Synchronizing;
using Magus.Global;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Magus.Game
{
    public enum GameStage
    {
        Training,
        Battle,
        SuddenDeath
    }

    public class RoundController : NetworkBehaviour
    {
        public static RoundController instance;

        private readonly SyncVar<float> stageTimer = new SyncVar<float>();
        public event Action<float> OnStageTimerChanged;
        private bool changeStageTimer;

        public GameStage gameStage;

        private void Awake()
        {
            if(instance != null)
            {
                Destroy(gameObject);
                return;
            }
            instance = this;
            stageTimer.OnChange += StageTimer_OnChange;
            OnStageTimerChanged?.Invoke(stageTimer.Value);
        }

        private void OnDestroy()
        {
            stageTimer.OnChange -= StageTimer_OnChange;
        }

        private void StageTimer_OnChange(float prev, float next, bool asServer)
        {
            if(!asServer)
            {
                OnStageTimerChanged?.Invoke(next);
            }
            else
            {
                if(next <= 0)
                {
                    switch (gameStage)
                    {
                        case GameStage.Training:
                        case GameStage.Battle:
                            changeStageTimer = true;
                            break;
                        case GameStage.SuddenDeath:
                            changeStageTimer = false;
                            break;
                    }
                }
            }
        }

        [Server]
        private void SetStageTimer(float time)
        {
            stageTimer.Value = time;
        }

        public void StartRound()
        {
            switch(MatchController.instance.gameMode)
            {
                case GameMode.Training:
                    changeStageTimer = false;
                    SetStageTimer(Constants.MAX_TIME);
                    break;
                case GameMode.Standard:
                    changeStageTimer = true;
                    SetStageTimer(Constants.TRAINING_TIME);
                    break;
            }
        }

        private void Update()
        {
            if (base.IsServerInitialized)
            {
                if(stageTimer.Value > 0 && changeStageTimer)
                {
                    stageTimer.Value -= Time.deltaTime;
                }
            }
        }
    }
}