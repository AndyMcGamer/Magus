using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Magus.Game
{
    public enum GameMode
    {
        Training,
        Standard
    }

    public class MatchController : NetworkBehaviour
    {
        public static MatchController instance;

        public int roundNumber;
        public GameMode gameMode;

        private void Awake()
        {
            if(instance != null)
            {
                Destroy(gameObject);
                return;
            }
            instance = this;
        }

        public void StartMatch(GameMode mode)
        {
            gameMode = mode;
            roundNumber = 1;
            RoundController.instance.StartRound();
            SetRoundNumber(roundNumber);
            SetGameMode(mode);

            switch (gameMode)
            {
                case GameMode.Training:
                    GlobalPlayerController.instance.SetSkillPoints(1, 999);
                    break;
                case GameMode.Standard:
                    GlobalPlayerController.instance.SetSkillPoints(1, 10);
                    GlobalPlayerController.instance.SetSkillPoints(2, 10);
                    break;
            }
        }

        [ObserversRpc(ExcludeServer = true)]
        private void SetRoundNumber(int roundNumber)
        {
            this.roundNumber = roundNumber;
        }

        [ObserversRpc(ExcludeServer = true)]
        private void SetGameMode(GameMode gameMode)
        {
            this.gameMode = gameMode;
        }
    }
}
