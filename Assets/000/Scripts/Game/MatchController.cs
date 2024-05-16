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
        }

        [ObserversRpc]
        private void SetRoundNumber(int roundNumber)
        {
            this.roundNumber = roundNumber;
        }

        [ObserversRpc]
        private void SetGameMode(GameMode gameMode)
        {
            this.gameMode = gameMode;
        }
    }
}
