using FishNet.Object;
using Magus.MatchmakingSystem;
using Magus.Multiplayer;
using Magus.UserInterface;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Magus.SceneSpecific
{
    public class EndSceneUI : NetworkBehaviour
    {
        [SerializeField] private GameEnd gameEnd;

        [SerializeField] private TextMeshProUGUI outcomeText;

        private bool quitting;

        public override void OnStartClient()
        {
            base.OnStartClient();
            gameEnd.OnCountdownChanged += GameEnd_OnCountdownChanged;
            gameEnd.OnWinnerLoaded += LoadWinner;
        }

        public override void OnStopClient()
        {
            base.OnStopClient();
            gameEnd.OnCountdownChanged -= GameEnd_OnCountdownChanged;
            gameEnd.OnWinnerLoaded -= LoadWinner;
        }

        private async void Enter()
        {
            await Fader.instance.FadeOut(1.5f, DG.Tweening.Ease.InSine);
        }

        private void LoadWinner(int winnerNumber)
        {
            Enter();
            int playerNumber = ConnectionManager.instance.playerData[base.LocalConnection];   
            if(playerNumber == winnerNumber) 
            {
                outcomeText.text = "You Won";
            }
            else
            {
                outcomeText.text = "You Lost";
            }
        }

        private void GameEnd_OnCountdownChanged(float timer)
        {
            print(timer);
            if (timer <= 0)
            {
                QuitToLobby();
            }
        }

        public void QuitToLobby()
        {
            if (quitting) return;
            quitting = true;

            ConnectionManager.instance.End_BackToLobby();
        }

        public void QuitToMenu()
        {
            if (quitting) return;
            quitting = true;

            ConnectionManager.instance.End_BackToMenu();
        }
    }
}
