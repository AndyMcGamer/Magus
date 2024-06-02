using FishNet.Object;
using FishNet.Object.Synchronizing;
using Magus.Global;
using Magus.SceneManagement;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
                            SwitchToBattle();
                            break;
                        case GameStage.Battle:
                            SwitchToTraining();
                            break;
                        case GameStage.SuddenDeath:
                            changeStageTimer = false;
                            SetStageTimer(0.01f);
                            break;
                    }
                }
            }
        }

        private void SwitchToBattle()
        {
            changeStageTimer = true;
            GlobalPlayerController.instance.SetPlayerHealth(1, 100);
            GlobalPlayerController.instance.SetPlayerHealth(2, 100);
            SetStageTimer(Constants.BATTLE_TIME);
            gameStage = GameStage.Battle;
            SetGameStage(gameStage);
            SceneSwitcher.instance.LoadGlobalNetworkedScene("HealthBars", false, FishNet.Managing.Scened.ReplaceOption.All);
            SceneSwitcher.instance.LoadGlobalNetworkedScene("RoundTimer", false, FishNet.Managing.Scened.ReplaceOption.None);
            SceneSwitcher.instance.LoadGlobalNetworkedScene("BattleScene", true, FishNet.Managing.Scened.ReplaceOption.None);
        }

        private void SwitchToTraining()
        {
            changeStageTimer = false;
            float p1Health = GlobalPlayerController.instance.GetCurrentHealth(1);
            float p2Health = GlobalPlayerController.instance.GetCurrentHealth(2);
            if(p1Health > p2Health)
            {
                MatchController.instance.EndRound(1);
            }
            else if(p1Health < p2Health)
            {
                MatchController.instance.EndRound(2);
            }
            else
            {
                MatchController.instance.EndRound(0);
                print("RoundEnded Tie");
            }
        }

        [ServerRpc(RequireOwnership = false)]
        public void EndRound()
        {
            SwitchToTraining();
        }


        [Server]
        private void SetStageTimer(float time)
        {
            stageTimer.Value = time;
        }

        public void StartRound()
        {
            gameStage = GameStage.Training;
            SetGameStage(gameStage);
            GlobalPlayerController.instance.SetPlayerHealth(1, 100);
            GlobalPlayerController.instance.SetPlayerHealth(2, 100);
            switch (MatchController.instance.gameMode)
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

        [ObserversRpc(ExcludeServer = true)]
        private void SetGameStage(GameStage stage)
        {
            gameStage = stage;
        }
    }
}
