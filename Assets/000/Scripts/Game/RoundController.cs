using FishNet.Object;
using FishNet.Object.Synchronizing;
using Magus.Global;
using Magus.SceneManagement;
using Magus.UserInterface;
using NaughtyAttributes;
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
        Transition,
        Battle,
        SuddenDeath
    }

    public class RoundController : NetworkBehaviour
    {
        public static RoundController instance;

        private readonly SyncVar<float> stageTimer = new SyncVar<float>();
        public float StageTimer => stageTimer.Value;

        public event Action<float> OnStageTimerChanged;
        private bool changeStageTimer;

        public GameStage gameStage;

        public event Action OnGameStageChanged;

        private WaitForSeconds pointSevenFive = new(0.75f);

        [ReadOnly] public int readyPlayers;
        public event Action<int> OnReadyPlayersChanged;

        public event Action<int> OnRoundWinner;

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
                            SwitchToTransition();
                            break;
                        case GameStage.Battle:
                            StartCoroutine(SwitchToTraining());
                            break;
                        case GameStage.Transition:
                            StartCoroutine(SwitchToBattle());
                            break;
                        case GameStage.SuddenDeath:
                            changeStageTimer = false;
                            SetStageTimer(0.01f);
                            break;
                    }
                }
            }
        }

        [ServerRpc(RequireOwnership = false)]
        public void AddReadyPlayer()
        {
            readyPlayers++;
            SetReadyPlayers(readyPlayers);
            if(readyPlayers == MatchController.instance.MinPlayers)
            {
                JumpToBattlePrep();
            }
        }

        [ObserversRpc(ExcludeServer = false)]
        private void SetReadyPlayers(int players)
        {
            readyPlayers = players;
            OnReadyPlayersChanged?.Invoke(players);
        }

        private void SwitchToTransition()
        {
            changeStageTimer = true;
            SetStageTimer(Constants.TRANSITION_TIME);
            gameStage = GameStage.Transition;
            SetGameStage(gameStage);

            GlobalPlayerController.instance.LockSkills();
        }

        private IEnumerator SwitchToBattle()
        {
            ClientFade();
            yield return pointSevenFive;

            // Will be changed with PlayerStats
            GlobalPlayerController.instance.SetPlayerHealth(1, 100);
            GlobalPlayerController.instance.SetPlayerHealth(2, 100);

            SetStageTimer(Constants.BATTLE_TIME);
            gameStage = GameStage.Battle;
            SetGameStage(gameStage);

            //SceneSwitcher.instance.LoadGlobalNetworkedScene("HealthBars", false, FishNet.Managing.Scened.ReplaceOption.All);
            SceneSwitcher.instance.LoadGlobalNetworkedScene("RoundTimer", false, FishNet.Managing.Scened.ReplaceOption.All);
            SceneSwitcher.instance.LoadGlobalNetworkedScene("BattleScene", true, FishNet.Managing.Scened.ReplaceOption.None);
        }

        [Server]
        public void SetChangeTimer(bool changeTimer)
        {
            changeStageTimer = changeTimer;
        }

        private IEnumerator SwitchToTraining()
        {
            ClientFade();

            readyPlayers = 0;
            SetReadyPlayers(readyPlayers);

            yield return pointSevenFive;

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

        [Server]
        public void DetermineWinner()
        {
            float p1Health = GlobalPlayerController.instance.GetCurrentHealth(1);
            float p2Health = GlobalPlayerController.instance.GetCurrentHealth(2);

            if (p1Health > p2Health)
            {
                MatchController.instance.ChooseWinner(1);
                BroadcastRoundWin(1);
            }
            else if (p1Health < p2Health)
            {
                MatchController.instance.ChooseWinner(2);
                BroadcastRoundWin(2);
            }
        }

        [ObserversRpc]
        private void BroadcastRoundWin(int winner)
        {
            OnRoundWinner?.Invoke(winner);
        }

        public void EndRound()
        {
            StartCoroutine(SwitchToTraining());
        }

        [ObserversRpc]
        private void ClientFade()
        {
            ClientFadeAsync();
        }

        private async void ClientFadeAsync()
        {
            await Fader.instance.FadeIn(easeFunction: DG.Tweening.Ease.OutQuad);
        }

        [Server]
        private void SetStageTimer(float time)
        {
            stageTimer.Value = time;
        }

        [Server]
        public void JumpToBattlePrep()
        {
            if (gameStage != GameStage.Training) return;
            stageTimer.Value = 0.1f;
        }

        [Server]
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

        [ObserversRpc(ExcludeServer = false)]
        private void SetGameStage(GameStage stage)
        {
            gameStage = stage;
            OnGameStageChanged?.Invoke();
        }
    }
}
